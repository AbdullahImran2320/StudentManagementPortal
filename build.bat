@echo off
setlocal enabledelayedexpansion

echo ================================================
echo   Student Management Portal - Build Script
echo ================================================

set "ROOT=%~dp0"
set "BACKEND_DIR=%ROOT%Backend"
set "FRONTEND_DIR=%ROOT%Frontend"
set "PUBLISH_DIR=%ROOT%publish\StudentManagementPortal"

echo.
echo [1/5] Cleaning previous build output...
if exist "%BACKEND_DIR%\wwwroot" rmdir /s /q "%BACKEND_DIR%\wwwroot"
if exist "%PUBLISH_DIR%" rmdir /s /q "%PUBLISH_DIR%"

echo.
echo [2/5] Installing frontend dependencies...
cd /d "%FRONTEND_DIR%"
call npm install
if errorlevel 1 goto :error

echo.
echo [3/5] Building Angular app (production configuration)...
call npx ng build --configuration production
if errorlevel 1 goto :error

if not exist "%FRONTEND_DIR%\dist\student-app\browser" (
    echo ERROR: Expected Angular output not found at dist\student-app\browser.
    echo Check the actual output folder printed above and update build.bat if it differs.
    goto :error
)

echo.
echo [4/5] Copying Angular build into Backend\wwwroot...
mkdir "%BACKEND_DIR%\wwwroot"
xcopy "%FRONTEND_DIR%\dist\student-app\browser\*" "%BACKEND_DIR%\wwwroot\" /E /I /Y
if errorlevel 1 goto :error

echo.
echo [5/5] Publishing self-contained backend executable (win-x64)...
cd /d "%BACKEND_DIR%"
dotnet publish -c Release -r win-x64 --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -p:EnableCompressionInSingleFile=true ^
    -o "%PUBLISH_DIR%"
if errorlevel 1 goto :error

echo.
echo Copying launcher script into publish output...
copy /Y "%ROOT%LaunchStudentManagementPortal.ps1" "%PUBLISH_DIR%\" >nul

echo.
echo ================================================
echo   Build complete: %PUBLISH_DIR%
echo   Next: run compile-installer.bat
echo ================================================
endlocal
exit /b 0

:error
echo.
echo *** BUILD FAILED ***
endlocal
exit /b 1
