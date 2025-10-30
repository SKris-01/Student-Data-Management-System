# Student Management System Startup Script for VS Code
# Run this in VS Code terminal: .\start-project.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Student Management System Startup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Function to write colored output
function Write-Status {
    param([string]$Message, [string]$Status = "Info")
    switch ($Status) {
        "Success" { Write-Host "✅ $Message" -ForegroundColor Green }
        "Error" { Write-Host "❌ $Message" -ForegroundColor Red }
        "Warning" { Write-Host "⚠️ $Message" -ForegroundColor Yellow }
        "Info" { Write-Host "ℹ️ $Message" -ForegroundColor Blue }
        default { Write-Host $Message }
    }
}

# Step 1: Check .NET SDK
Write-Host "[1/6] Checking .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Status ".NET SDK found: $dotnetVersion" "Success"
} catch {
    Write-Status ".NET SDK not found! Please install .NET 8 SDK" "Error"
    exit 1
}

# Step 2: Check SQL Server
Write-Host "`n[2/6] Checking SQL Server..." -ForegroundColor Yellow
$sqlServerFound = $false

try {
    $service = Get-Service MSSQLSERVER -ErrorAction Stop
    Write-Status "SQL Server Status: $($service.Status)" "Success"
    $sqlServerFound = $true
    
    if ($service.Status -ne "Running") {
        Write-Status "Starting SQL Server..." "Info"
        Start-Service MSSQLSERVER -ErrorAction Stop
        Write-Status "SQL Server started successfully" "Success"
    }
} catch {
    try {
        $service = Get-Service 'MSSQL$SQLEXPRESS' -ErrorAction Stop
        Write-Status "SQL Server Express Status: $($service.Status)" "Success"
        $sqlServerFound = $true
        
        if ($service.Status -ne "Running") {
            Write-Status "Starting SQL Server Express..." "Info"
            Start-Service 'MSSQL$SQLEXPRESS' -ErrorAction Stop
            Write-Status "SQL Server Express started successfully" "Success"
        }
    } catch {
        Write-Status "No SQL Server found - will try LocalDB" "Warning"
    }
}

# Step 3: Navigate to project directory
Write-Host "`n[3/6] Setting up project directory..." -ForegroundColor Yellow
$projectPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $projectPath
Write-Status "Current directory: $(Get-Location)" "Info"

# Step 4: Kill existing processes
Write-Host "`n[4/6] Cleaning up processes..." -ForegroundColor Yellow
try {
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force
    Get-Process -Name "StudentManagementSystem" -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Status "Existing processes terminated" "Success"
} catch {
    Write-Status "No existing processes to terminate" "Info"
}

# Step 5: Build project
Write-Host "`n[5/6] Building project..." -ForegroundColor Yellow

Write-Status "Cleaning previous build..." "Info"
& dotnet clean | Out-Null

Write-Status "Restoring packages..." "Info"
$restoreResult = & dotnet restore --force 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Status "Package restore failed, clearing cache..." "Warning"
    & dotnet nuget locals all --clear | Out-Null
    & dotnet restore --force | Out-Null
}

Write-Status "Building project..." "Info"
$buildResult = & dotnet build 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Status "Build successful" "Success"
} else {
    Write-Status "Build failed, attempting cleanup..." "Warning"
    
    # Remove bin and obj folders
    if (Test-Path "bin") { Remove-Item -Recurse -Force "bin" }
    if (Test-Path "obj") { Remove-Item -Recurse -Force "obj" }
    
    Write-Status "Rebuilding after cleanup..." "Info"
    & dotnet restore --force | Out-Null
    $buildResult = & dotnet build 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Status "Build successful after cleanup" "Success"
    } else {
        Write-Status "Build still failing" "Error"
        Write-Host "`nBuild errors:" -ForegroundColor Red
        Write-Host $buildResult -ForegroundColor Red
        Write-Host "`nCommon solutions:" -ForegroundColor Yellow
        Write-Host "1. Run PowerShell as Administrator" -ForegroundColor White
        Write-Host "2. Close Visual Studio if open" -ForegroundColor White
        Write-Host "3. Ensure SQL Server is running" -ForegroundColor White
        Write-Host "4. Check Windows Defender/Antivirus" -ForegroundColor White
        Read-Host "Press Enter to exit"
        exit 1
    }
}

# Step 6: Start application
Write-Host "`n[6/6] Starting application..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Status "Application starting on: http://localhost:5000" "Success"
Write-Host ""
Write-Host "📋 Available URLs:" -ForegroundColor Cyan
Write-Host "   🏠 Home Page: http://localhost:5000" -ForegroundColor White
Write-Host "   📊 Admin Dashboard: http://localhost:5000/Student/AdminDashboard" -ForegroundColor White
Write-Host "   ➕ Add Student: http://localhost:5000/Student/CreateStudent" -ForegroundColor White
Write-Host "   🔧 Test Database: http://localhost:5000/Student/TestDataPersistence" -ForegroundColor White
Write-Host ""
Write-Host "🔑 Login Credentials:" -ForegroundColor Cyan
Write-Host "   Username: admin" -ForegroundColor White
Write-Host "   Password: Admin123!" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Start the application
& dotnet run --urls "http://localhost:5000"
