using System.Globalization;
using System.Text;

namespace LabFileCollector;

public static class FileTransferService
{
    public static List<PlannedFile> BuildPlan(
        IEnumerable<string> sourceRoots,
        string destinationRoot,
        string pcName,
        bool includeAllFiles,
        IEnumerable<string> allowedExtensions,
        Action<string>? status = null,
        CancellationToken cancellationToken = default)
    {
        var plan = new List<PlannedFile>();
        var extSet = new HashSet<string>(
            allowedExtensions
                .Select(NormalizeExtension)
                .Where(x => !string.IsNullOrWhiteSpace(x)),
            StringComparer.OrdinalIgnoreCase);

        var pcDestinationRoot = Path.Combine(destinationRoot, pcName);

        foreach (var sourceRootRaw in sourceRoots.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                status?.Invoke("Η σάρωση σταμάτησε από τον χρήστη.");
                break;
            }

            var sourceRoot = Environment.ExpandEnvironmentVariables(sourceRootRaw.Trim());
            if (!Directory.Exists(sourceRoot))
            {
                status?.Invoke($"Παράλειψη: δεν βρέθηκε το path {sourceRoot}");
                continue;
            }

            var sourceLabel = GetSafeFolderName(new DirectoryInfo(sourceRoot).Name);
            if (string.IsNullOrWhiteSpace(sourceLabel))
                sourceLabel = "Source";

            status?.Invoke($"Σάρωση πηγής: {sourceRoot}");
            var scannedInThisSource = 0;

            foreach (var file in SafeEnumerateFiles(sourceRoot, status, cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    var info = new FileInfo(file);
                    if (!info.Exists) continue;

                    if (!includeAllFiles)
                    {
                        var ext = info.Extension;
                        if (!extSet.Contains(ext)) continue;
                    }

                    var relative = Path.GetRelativePath(sourceRoot, file);
                    var relativeFolder = Path.GetDirectoryName(relative) ?? string.Empty;
                    var isLooseFile = string.IsNullOrWhiteSpace(relativeFolder) || relativeFolder == ".";
                    var destination = Path.Combine(pcDestinationRoot, sourceLabel, relative);

                    plan.Add(new PlannedFile
                    {
                        SourcePath = file,
                        DestinationPath = destination,
                        SourceRoot = sourceRoot,
                        SourceLabel = sourceLabel,
                        RelativePath = relative,
                        RelativeFolder = isLooseFile ? string.Empty : relativeFolder,
                        IsLooseFile = isLooseFile,
                        SizeBytes = info.Length
                    });

                    scannedInThisSource++;
                    if (scannedInThisSource % 250 == 0)
                        status?.Invoke($"Σάρωση {sourceLabel}: βρέθηκαν μέχρι τώρα {scannedInThisSource} αρχεία που ταιριάζουν.");
                }
                catch (Exception ex)
                {
                    status?.Invoke($"Παράλειψη αρχείου: {file} | {ex.Message}");
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                status?.Invoke("Η σάρωση σταμάτησε από τον χρήστη.");
                break;
            }
        }

        return plan;
    }

    private static IEnumerable<string> SafeEnumerateFiles(string root, Action<string>? status, CancellationToken cancellationToken)
    {
        var pending = new Stack<string>();
        pending.Push(root);

        while (pending.Count > 0)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var current = pending.Pop();

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(current);
            }
            catch (Exception ex)
            {
                status?.Invoke($"Αδυναμία ανάγνωσης αρχείων: {current} | {ex.Message}");
                continue;
            }

            foreach (var file in files)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return file;
            }

            IEnumerable<string> dirs;
            try
            {
                dirs = Directory.EnumerateDirectories(current);
            }
            catch (Exception ex)
            {
                status?.Invoke($"Αδυναμία ανάγνωσης υποφακέλων: {current} | {ex.Message}");
                continue;
            }

            foreach (var dir in dirs)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                try
                {
                    var attributes = File.GetAttributes(dir);
                    if ((attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    {
                        status?.Invoke($"Παράλειψη junction/symlink: {dir}");
                        continue;
                    }
                }
                catch
                {
                    // Αν δεν διαβάζονται attributes, άσε το επόμενο enumeration να το χειριστεί.
                }

                pending.Push(dir);
            }
        }
    }

    public static async Task<TransferResult> ExecuteAsync(
        IReadOnlyList<PlannedFile> plan,
        TransferMode mode,
        IProgress<TransferProgress>? progress,
        CancellationToken cancellationToken)
    {
        var result = new TransferResult();
        var total = plan.Count;

        if (mode == TransferMode.ScanOnly)
        {
            result.Skipped = total;
            for (var i = 0; i < plan.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    result.Canceled = true;
                    break;
                }

                var item = plan[i];
                result.LogRows.Add(LogRow(item, item.DestinationPath, "SCAN_ONLY", "OK"));
                progress?.Report(new TransferProgress(i + 1, total, item.SourcePath, "SCAN"));
            }
            return result;
        }

        for (var i = 0; i < plan.Count; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result.Canceled = true;
                break;
            }

            var item = plan[i];
            var finalDestination = GetNonConflictingPath(item.DestinationPath);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(finalDestination)!);

                await using (var source = new FileStream(item.SourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024 * 128, useAsync: true))
                await using (var destination = new FileStream(finalDestination, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1024 * 128, useAsync: true))
                {
                    await source.CopyToAsync(destination, cancellationToken);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    TryDeletePartialDestination(finalDestination);
                    result.Canceled = true;
                    result.LogRows.Add(LogRow(item, finalDestination, "CANCELED", "User canceled after copy before commit"));
                    progress?.Report(new TransferProgress(i + 1, total, item.SourcePath, "CANCELED"));
                    break;
                }

                var copiedInfo = new FileInfo(finalDestination);
                if (copiedInfo.Length != item.SizeBytes)
                    throw new IOException("Το αρχείο αντιγράφηκε αλλά το μέγεθος δεν ταιριάζει.");

                result.SuccessfulSourcePaths.Add(item.SourcePath);

                if (mode == TransferMode.Move)
                {
                    File.Delete(item.SourcePath);
                    result.Moved++;
                    result.Operations.Add(new TransferOperation(item.SourcePath, finalDestination, "MOVED", item.SizeBytes));
                    result.LogRows.Add(LogRow(item, finalDestination, "MOVED", "OK"));
                }
                else
                {
                    result.Copied++;
                    result.Operations.Add(new TransferOperation(item.SourcePath, finalDestination, "COPIED", item.SizeBytes));
                    result.LogRows.Add(LogRow(item, finalDestination, "COPIED", "OK"));
                }

                progress?.Report(new TransferProgress(i + 1, total, item.SourcePath, "OK"));
            }
            catch (OperationCanceledException)
            {
                TryDeletePartialDestination(finalDestination);
                result.Canceled = true;
                result.LogRows.Add(LogRow(item, finalDestination, "CANCELED", "User canceled"));
                progress?.Report(new TransferProgress(i + 1, total, item.SourcePath, "CANCELED"));
                break;
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.LogRows.Add(LogRow(item, finalDestination, "FAILED", ex.Message));
                progress?.Report(new TransferProgress(i + 1, total, item.SourcePath, "FAILED"));
            }
        }

        return result;
    }

    public static async Task<UndoResult> UndoAsync(
        IReadOnlyList<TransferOperation> operations,
        IProgress<TransferProgress>? progress,
        CancellationToken cancellationToken)
    {
        var result = new UndoResult();
        var ordered = operations.Reverse().ToList();
        var total = ordered.Count;

        for (var i = 0; i < ordered.Count; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                result.Canceled = true;
                break;
            }

            var op = ordered[i];

            try
            {
                if (string.Equals(op.Action, "COPIED", StringComparison.OrdinalIgnoreCase))
                {
                    if (!File.Exists(op.DestinationPath))
                    {
                        result.Skipped++;
                        result.LogRows.Add(UndoLogRow(op, "UNDO_COPY_DELETE", "SKIPPED_DESTINATION_NOT_FOUND"));
                    }
                    else if (new FileInfo(op.DestinationPath).Length != op.SizeBytes)
                    {
                        result.Failed++;
                        result.LogRows.Add(UndoLogRow(op, "UNDO_COPY_DELETE", "FAILED_DESTINATION_SIZE_CHANGED"));
                    }
                    else
                    {
                        File.Delete(op.DestinationPath);
                        result.DeletedCopiedFiles++;
                        result.LogRows.Add(UndoLogRow(op, "UNDO_COPY_DELETE", "OK"));
                        DeleteEmptyParentsSafely(Path.GetDirectoryName(op.DestinationPath));
                    }
                }
                else if (string.Equals(op.Action, "MOVED", StringComparison.OrdinalIgnoreCase))
                {
                    if (!File.Exists(op.DestinationPath))
                    {
                        result.Failed++;
                        result.LogRows.Add(UndoLogRow(op, "UNDO_MOVE_BACK", "FAILED_DESTINATION_NOT_FOUND"));
                    }
                    else if (File.Exists(op.SourcePath))
                    {
                        result.Failed++;
                        result.LogRows.Add(UndoLogRow(op, "UNDO_MOVE_BACK", "FAILED_SOURCE_ALREADY_EXISTS"));
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(op.SourcePath)!);
                        File.Move(op.DestinationPath, op.SourcePath);
                        result.MovedBackFiles++;
                        result.LogRows.Add(UndoLogRow(op, "UNDO_MOVE_BACK", "OK"));
                        DeleteEmptyParentsSafely(Path.GetDirectoryName(op.DestinationPath));
                    }
                }
                else
                {
                    result.Skipped++;
                    result.LogRows.Add(UndoLogRow(op, "UNDO", "SKIPPED_UNKNOWN_ACTION"));
                }

                progress?.Report(new TransferProgress(i + 1, total, op.DestinationPath, "UNDO"));
                await Task.Yield();
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.LogRows.Add(UndoLogRow(op, "UNDO_FAILED", ex.Message));
                progress?.Report(new TransferProgress(i + 1, total, op.DestinationPath, "UNDO_FAILED"));
            }
        }

        return result;
    }

    public static string WriteCsvLog(string destinationRoot, string pcName, TransferResult result)
    {
        var logsRoot = Path.Combine(destinationRoot, "_logs");
        Directory.CreateDirectory(logsRoot);

        var fileName = $"transfer-log-{pcName}-{DateTime.Now:yyyyMMdd-HHmmss}.csv";
        var path = Path.Combine(logsRoot, fileName);

        var sb = new StringBuilder();
        sb.AppendLine("DateTime;Source;RelativePath;Placement;Destination;Action;StatusOrError");
        foreach (var row in result.LogRows)
            sb.AppendLine(row);

        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        return path;
    }

    public static string WriteUndoCsvLog(string destinationRoot, string pcName, UndoResult result)
    {
        var logsRoot = Path.Combine(destinationRoot, "_logs");
        Directory.CreateDirectory(logsRoot);

        var fileName = $"undo-log-{pcName}-{DateTime.Now:yyyyMMdd-HHmmss}.csv";
        var path = Path.Combine(logsRoot, fileName);

        var sb = new StringBuilder();
        sb.AppendLine("DateTime;OriginalSource;Destination;OriginalAction;UndoAction;StatusOrError");
        foreach (var row in result.LogRows)
            sb.AppendLine(row);

        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        return path;
    }

    public static string HumanBytes(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB", "TB"];
        double size = bytes;
        var unit = 0;
        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }
        return $"{size:0.##} {units[unit]}";
    }

    public static long? TryGetAvailableBytes(string destinationRoot)
    {
        try
        {
            var fullPath = Path.GetFullPath(destinationRoot);
            var root = Path.GetPathRoot(fullPath);
            if (string.IsNullOrWhiteSpace(root)) return null;
            return new DriveInfo(root).AvailableFreeSpace;
        }
        catch
        {
            return null;
        }
    }

    private static string GetNonConflictingPath(string path)
    {
        if (!File.Exists(path)) return path;

        var dir = Path.GetDirectoryName(path)!;
        var name = Path.GetFileNameWithoutExtension(path);
        var ext = Path.GetExtension(path);
        var counter = 1;

        string candidate;
        do
        {
            candidate = Path.Combine(dir, $"{name} ({counter}){ext}");
            counter++;
        } while (File.Exists(candidate));

        return candidate;
    }

    private static void DeleteEmptyParentsSafely(string? startDirectory)
    {
        var current = startDirectory;
        while (!string.IsNullOrWhiteSpace(current))
        {
            try
            {
                if (!Directory.Exists(current)) return;
                if (Directory.EnumerateFileSystemEntries(current).Any()) return;
                Directory.Delete(current, recursive: false);
                current = Path.GetDirectoryName(current);
            }
            catch
            {
                return;
            }
        }
    }

    private static void TryDeletePartialDestination(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Αν δεν μπορεί να σβηστεί προσωρινό/μερικό αρχείο, δεν ρισκάρουμε δεύτερη αλλαγή εδώ.
        }
    }

    private static string NormalizeExtension(string extension)
    {
        extension = extension.Trim();
        if (string.IsNullOrWhiteSpace(extension)) return string.Empty;
        return extension.StartsWith('.') ? extension : "." + extension;
    }

    private static string GetSafeFolderName(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name.Trim();
    }

    private static string CsvEscape(string value)
    {
        value = value.Replace("\r", " ").Replace("\n", " ");
        if (value.Contains(';') || value.Contains('"'))
            value = '"' + value.Replace("\"", "\"\"") + '"';
        return value;
    }

    private static string LogRow(PlannedFile item, string finalDestination, string action, string statusOrError)
    {
        return string.Join(';',
            CsvEscape(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
            CsvEscape(item.SourcePath),
            CsvEscape(item.RelativePath),
            CsvEscape(item.PlacementLabel),
            CsvEscape(finalDestination),
            CsvEscape(action),
            CsvEscape(statusOrError));
    }

    private static string UndoLogRow(TransferOperation operation, string undoAction, string statusOrError)
    {
        return string.Join(';',
            CsvEscape(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
            CsvEscape(operation.SourcePath),
            CsvEscape(operation.DestinationPath),
            CsvEscape(operation.Action),
            CsvEscape(undoAction),
            CsvEscape(statusOrError));
    }
}

public sealed record TransferProgress(int Current, int Total, string CurrentFile, string Status);

public sealed record TransferOperation(string SourcePath, string DestinationPath, string Action, long SizeBytes);

public sealed class TransferResult
{
    public int Copied { get; set; }
    public int Moved { get; set; }
    public int Failed { get; set; }
    public int Skipped { get; set; }
    public bool Canceled { get; set; }
    public List<string> LogRows { get; } = new();
    public List<string> SuccessfulSourcePaths { get; } = new();
    public List<TransferOperation> Operations { get; } = new();
}

public sealed class UndoResult
{
    public int DeletedCopiedFiles { get; set; }
    public int MovedBackFiles { get; set; }
    public int Failed { get; set; }
    public int Skipped { get; set; }
    public bool Canceled { get; set; }
    public List<string> LogRows { get; } = new();
}
