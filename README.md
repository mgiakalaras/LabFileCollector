# School Lab File Collector

Windows Forms / .NET 8 tool for collecting student files from lab PCs into a network or local destination folder.

## Main workflow

1. The app detects the Windows computer name automatically.
2. Choose or confirm the destination root path, for example:
   - `\\192.168.1.37\2026`
   - `\\192.168.1.37\shared\Εργασίες Μαθητών\Πληροφορική Γ Λυκείου\2026`
3. Select source paths, usually under `C:\Users\Student`.
4. Choose file extensions or select all files.
5. Scan first.
6. Review the preview table.
7. Copy or move only when ready.

## Folder behavior

The app scans recursively and preserves folder structure.

Example source:

```text
C:\Users\Student\Desktop\Β1\Μαθητής Α\askisi.glo
```

Example destination:

```text
\\192.168.1.37\2026\EDULABS\Desktop\Β1\Μαθητής Α\askisi.glo
```

Loose files are marked as `Χύμα`; files inside folders are marked as `Σε φάκελο`.

## Default extensions

```text
.doc;.docx;.xls;.xlsx;.ppt;.pptx;.pdf;.txt;.py;.html;.css;.js;.sb3;.glo;.psg
```

## UI themes

Version 1.1 includes:

- Σύστημα / Windows
- Φωτεινό / Aero
- Σκούρο / Aero
- Notepad++ Dark
- Nord Dark
- Dracula

The Windows/Aero modes use the Windows accent color when possible.

## System information sidebar

The right panel shows live system information:

- computer type / architecture
- Windows computer name, operating system and logged-in user
- processor and CPU usage as a mini chart
- RAM usage as a mini chart
- disk used/free space as mini charts per local drive
- active LAN/Wi‑Fi adapters, IPv4 and link speed as mini chart cards

## Safety features

- Scan does not change files.
- Stop asks for confirmation.
- Move deletes the source only after successful copy/size validation.
- Undo last copy/move is available for the current session.
- Downloads cleanup only touches files that were successfully processed from the current plan.
- Windows Update cleanup uses supported DISM component cleanup instead of manually deleting WinSxS.

## Build

Open `LabFileCollector.sln` in Visual Studio 2022 and run:

```text
Clean Solution
Rebuild Solution
F5
```

## Publish portable EXE

```powershell
dotnet publish .\LabFileCollector.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:EnableCompressionInSingleFile=true
```
