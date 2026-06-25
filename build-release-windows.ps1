# Run this inside the project folder from PowerShell.
# Creates a portable single-file exe for Windows x64.

dotnet publish .\LabFileCollector.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  /p:PublishSingleFile=true `
  /p:EnableCompressionInSingleFile=true
