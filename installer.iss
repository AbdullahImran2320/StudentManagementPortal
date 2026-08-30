#define MyAppName "Student Management Portal"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Abdullah Imran"
#define MyAppURL "https://github.com/AbdullahImran2320/StudentManagementPortal"
#define MyAppExeName "StudentAPI.exe"
#define MyLauncher "LaunchStudentManagementPortal.ps1"

[Setup]
AppId={{4A6F0D2E-7C1B-4E5A-9F3D-STUMGMTPORTAL}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=setup
OutputBaseFilename=StudentManagementPortalSetup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional shortcuts:"

[Files]
Source: "publish\StudentManagementPortal\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "powershell.exe"; \
    Parameters: "-ExecutionPolicy Bypass -WindowStyle Hidden -File ""{app}\{#MyLauncher}"""; \
    WorkingDir: "{app}"; IconFilename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "powershell.exe"; \
    Parameters: "-ExecutionPolicy Bypass -WindowStyle Hidden -File ""{app}\{#MyLauncher}"""; \
    WorkingDir: "{app}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "powershell.exe"; \
    Parameters: "-ExecutionPolicy Bypass -WindowStyle Hidden -File ""{app}\{#MyLauncher}"""; \
    WorkingDir: "{app}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Code]
function IsLocalDBInstalled(): Boolean;
begin
  Result :=
    RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions') or
    RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server Local DB\Installed Versions');
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
  if not IsLocalDBInstalled() then
  begin
    if MsgBox('SQL Server LocalDB was not detected on this computer.' + #13#10 +
               #13#10 +
               'Student Management Portal needs LocalDB (or SQL Server) to store data.' + #13#10 +
               'You can install it later from https://aka.ms/localdbdotnetcore' + #13#10 +
               #13#10 +
               'Continue installing anyway?',
               mbConfirmation, MB_YESNO) = IDNO then
      Result := False;
  end;
end;
