@echo off
setlocal

set "ROOT=%~dp0"
set "ISCC=C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

if not exist "%ISCC%" (
    set "ISCC=C:\Program Files\Inno Setup 6\ISCC.exe"
)

if not exist "%ISCC%" (
    echo ERROR: Inno Setup 6 Compiler ^(ISCC.exe^) was not found.
    echo Download and install Inno Setup from https://jrsoftware.org/isdl.php
    exit /b 1
)

if not exist "%ROOT%publish\StudentManagementPortal\StudentAPI.exe" (
    echo ERROR: Build output not found. Run build.bat first.
    exit /b 1
)

echo Compiling installer with Inno Setup...
"%ISCC%" "%ROOT%installer.iss"
if errorlevel 1 (
    echo.
    echo *** INSTALLER COMPILATION FAILED ***
    exit /b 1
)

echo.
echo ================================================
echo   Installer created in the setup\ folder.
echo ================================================
endlocal
exit /b 0
