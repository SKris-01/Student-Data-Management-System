@echo off
setlocal enabledelayedexpansion

echo ========================================
echo   Student Management System Startup
echo ========================================
echo.

echo [1/6] System Diagnostics...
echo Current user: %USERNAME%
echo Current directory: %CD%
echo.

echo [2/6] Checking .NET SDK...
dotnet --version >nul 2>&1
if %errorlevel%==0 (
    echo ✓ .NET SDK found:
    dotnet --version
) else (
    echo ❌ .NET SDK not found! Please install .NET 8 SDK
    pause
    exit /b 1
)

echo.
echo [3/6] Checking SQL Server status...
powershell -Command "try { $service = Get-Service MSSQLSERVER -ErrorAction Stop; Write-Host '✓ SQL Server Status:' $service.Status } catch { Write-Host '❌ SQL Server not found - trying alternatives...'; try { $service = Get-Service 'MSSQL$SQLEXPRESS' -ErrorAction Stop; Write-Host '✓ SQL Server Express Status:' $service.Status } catch { Write-Host '⚠️ No SQL Server found - will try LocalDB' } }"

echo.
echo [4/6] Navigating to project directory...
cd /d "%~dp0"
echo Current directory: %CD%

echo.
echo [5/6] Preparing project...
echo Killing any running processes...
taskkill /F /IM dotnet.exe >nul 2>&1
taskkill /F /IM StudentManagementSystem.exe >nul 2>&1

echo Cleaning previous build...
dotnet clean >nul 2>&1

echo Restoring packages...
dotnet restore --force
if %errorlevel% neq 0 (
    echo ❌ Package restore failed
    echo Clearing NuGet cache...
    dotnet nuget locals all --clear
    dotnet restore --force
)

echo Building project...
dotnet build
if %errorlevel%==0 (
    echo ✓ Build successful
) else (
    echo ❌ Build failed - attempting fixes...

    echo Removing bin and obj folders...
    if exist bin rmdir /s /q bin
    if exist obj rmdir /s /q obj

    echo Restoring and building again...
    dotnet restore --force
    dotnet build

    if %errorlevel%==0 (
        echo ✓ Build successful after cleanup
    ) else (
        echo ❌ Build still failing - check errors above
        echo.
        echo Common solutions:
        echo 1. Run as Administrator
        echo 2. Close Visual Studio if open
        echo 3. Check SQL Server is running
        echo 4. Restart computer if needed
        pause
        exit /b 1
    )
)

echo.
echo [6/6] Starting application...
echo ========================================
echo ✓ Application starting on: http://localhost:5000
echo ✓ Home Page: http://localhost:5000
echo ✓ Admin Dashboard: http://localhost:5000/Student/AdminDashboard
echo ✓ Add Student: http://localhost:5000/Student/CreateStudent
echo ✓ Test Database: http://localhost:5000/Student/TestDataPersistence
echo.
echo 📋 Login Credentials:
echo    Username: admin
echo    Password: Admin123!
echo.
echo Press Ctrl+C to stop the application
echo ========================================
echo.

dotnet run --urls "http://localhost:5000"
