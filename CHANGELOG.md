# CHANGELOG

## v1.1.0

### System information sidebar
- Converted the sidebar status blocks into mini chart cards for CPU, RAM, disks and LAN/Wi‑Fi link speed.
- Added operating system information from Windows registry / runtime fallback.
- Added currently logged-in Windows user, so the lab operator can quickly see which account is active.
- Disk cards show used percentage plus free/total capacity.
- Network cards show adapter name, IPv4, status and link speed.


## v1.0.0

### UI / Polishing
- Added a more modern button control with rounded, flat Windows/Fluent-like rendering.
- Added extra theme presets:
  - Σύστημα / Windows
  - Φωτεινό / Aero
  - Σκούρο / Aero
  - Notepad++ Dark
  - Nord Dark
  - Dracula
- Existing Windows accent-color support remains active for the Windows/Aero themes.
- Added best-effort native scrollbar theming using Explorer / DarkMode_Explorer for ListView, TextBox, CheckedListBox and ComboBox controls.
  - Note: full rounded scrollbar styling is limited in classic WinForms native controls without replacing the controls completely.

### System information sidebar
- Added a right-side system information panel.
- Displays:
  - computer type / architecture
  - Windows computer name
  - CPU model and usage percentage
  - RAM used / total / free
  - drive usage and free space
  - active LAN/Wi‑Fi adapters, IPv4 and link speed
- Refreshes automatically every 2.5 seconds.

### Existing behavior retained
- Recursive scan with folder structure preservation.
- Χύμα / Σε φάκελο preview classification.
- Stop confirmation and safe scan cancellation.
- Undo last copy/move.
- Help / About forms.
- .glo and .psg in the default extensions.

## v0.9.0
- Added Windows/Aero-inspired theme support and Windows accent color usage.

## v0.8.1
- Fixed Stop handling to avoid OperationCanceledException during scan.

## v0.8.0
- Added Stop and Undo last transfer.

## v0.7.1
- Fixed unescaped path examples in Help/MainForm strings.

## v0.7.0
- Added recursive folder-preserving scan and Χύμα / Σε φάκελο classification.

