# Launches Student Management Portal and opens it in the default browser.
# Installed alongside StudentAPI.exe by the Inno Setup installer.

$AppDir  = Split-Path -Parent $MyInvocation.MyCommand.Path
$ExeName = "StudentAPI"
$ExePath = Join-Path $AppDir "$ExeName.exe"
$Url     = "http://localhost:5080"

$proc = Get-Process -Name $ExeName -ErrorAction SilentlyContinue
if (-not $proc) {
    Start-Process -FilePath $ExePath -WorkingDirectory $AppDir -WindowStyle Hidden
}

# Wait up to ~10 seconds for the server to come up before opening the browser
for ($i = 0; $i -lt 20; $i++) {
    try {
        Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 1 | Out-Null
        break
    } catch {
        Start-Sleep -Milliseconds 500
    }
}

Start-Process $Url
