@echo off
setlocal enabledelayedexpansion

:: ArtSpark IIS Deployment Batch Script
:: Quick deployment script for IIS publishing

echo =======================================
echo    ArtSpark IIS Deployment Script
echo =======================================

:: Set paths
set PROJECT_PATH=%~dp0
set PROJECT_FILE=%PROJECT_PATH%WebSpark.ArtSpark.Demo.csproj
set PUBLISH_PATH=C:\PublishedWebsites\ArtSpark
set PACKAGE_PATH=C:\PublishedWebsites\Packages

:: Create package directory if it doesn't exist
if not exist "%PACKAGE_PATH%" (
    mkdir "%PACKAGE_PATH%"
    echo Created package directory: %PACKAGE_PATH%
)

:: Get current timestamp
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /value') do set datetime=%%I
set TIMESTAMP=%datetime:~0,8%_%datetime:~8,6%

echo.
echo Building and publishing application...
echo Project: %PROJECT_FILE%
echo Target: %PUBLISH_PATH%

:: Build and publish
dotnet publish "%PROJECT_FILE%" ^
    --configuration Release ^
    --output "%PUBLISH_PATH%" ^
    --self-contained false ^
    --runtime win-x64 ^
    --verbosity minimal

if errorlevel 1 (
    echo ERROR: Publish failed!
    pause
    exit /b 1
)

echo.
echo SUCCESS: Application published to %PUBLISH_PATH%

:: Create version info file
echo { > "%PUBLISH_PATH%\version.json"
echo   "BuildDate": "%date% %time%", >> "%PUBLISH_PATH%\version.json"
echo   "PublishedBy": "%USERNAME%", >> "%PUBLISH_PATH%\version.json"
echo   "MachineName": "%COMPUTERNAME%" >> "%PUBLISH_PATH%\version.json"
echo } >> "%PUBLISH_PATH%\version.json"

:: Create package
set PACKAGE_NAME=ArtSpark_%TIMESTAMP%.zip
set PACKAGE_FULL_PATH=%PACKAGE_PATH%\%PACKAGE_NAME%

echo.
echo Creating deployment package: %PACKAGE_NAME%

:: Use PowerShell to create zip file
powershell -Command "Compress-Archive -Path '%PUBLISH_PATH%\*' -DestinationPath '%PACKAGE_FULL_PATH%' -Force"

if errorlevel 1 (
    echo ERROR: Package creation failed!
    pause
    exit /b 1
)

echo.
echo SUCCESS: Package created at %PACKAGE_FULL_PATH%

:: Create deployment instructions
set INSTRUCTIONS_FILE=%PACKAGE_PATH%\ArtSpark_%TIMESTAMP%_Instructions.txt
echo ArtSpark Deployment Package > "%INSTRUCTIONS_FILE%"
echo ========================== >> "%INSTRUCTIONS_FILE%"
echo. >> "%INSTRUCTIONS_FILE%"
echo Package: %PACKAGE_NAME% >> "%INSTRUCTIONS_FILE%"
echo Created: %date% %time% >> "%INSTRUCTIONS_FILE%"
echo Created by: %USERNAME% >> "%INSTRUCTIONS_FILE%"
echo. >> "%INSTRUCTIONS_FILE%"
echo DEPLOYMENT STEPS: >> "%INSTRUCTIONS_FILE%"
echo 1. Stop IIS Application Pool >> "%INSTRUCTIONS_FILE%"
echo 2. Backup current website (optional) >> "%INSTRUCTIONS_FILE%"
echo 3. Extract zip to IIS website directory >> "%INSTRUCTIONS_FILE%"
echo 4. Start IIS Application Pool >> "%INSTRUCTIONS_FILE%"
echo 5. Test the website >> "%INSTRUCTIONS_FILE%"
echo. >> "%INSTRUCTIONS_FILE%"
echo REQUIREMENTS: >> "%INSTRUCTIONS_FILE%"
echo - .NET 9.0 Runtime installed >> "%INSTRUCTIONS_FILE%"
echo - IIS with ASP.NET Core Module >> "%INSTRUCTIONS_FILE%"
echo - Application Pool set to "No Managed Code" >> "%INSTRUCTIONS_FILE%"

echo.
echo =======================================
echo           Deployment Summary
echo =======================================
echo Published to: %PUBLISH_PATH%
echo Package: %PACKAGE_FULL_PATH%
echo Instructions: %INSTRUCTIONS_FILE%
echo.
echo Ready for server deployment!
echo =======================================

:: Open package folder
start "" explorer /select,"%PACKAGE_FULL_PATH%"

pause
