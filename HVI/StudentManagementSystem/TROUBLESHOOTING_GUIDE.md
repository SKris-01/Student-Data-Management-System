# 🛠️ Troubleshooting Guide - Student Management System

## 🚨 **Common Issues & Solutions**

### **Issue 1: "Access Denied" when starting SQL Server**

**Problem**: `net start MSSQLSERVER` shows "Access denied"

**Solutions**:
```powershell
# Solution A: Run as Administrator
# Right-click PowerShell/Command Prompt → "Run as Administrator"
net start MSSQLSERVER

# Solution B: Use PowerShell (Recommended)
Start-Service MSSQLSERVER

# Solution C: Check if already running
Get-Service MSSQLSERVER
```

**Verification**:
```powershell
Get-Service | Where-Object {$_.Name -like "*SQL*"}
# Should show MSSQLSERVER as "Running"
```

---

### **Issue 2: Build Not Completing**

**Problem**: `dotnet build` hangs or fails

**Solutions**:
```powershell
# Step 1: Kill any running dotnet processes
taskkill /F /IM dotnet.exe

# Step 2: Clean the project
dotnet clean

# Step 3: Restore packages
dotnet restore --force

# Step 4: Build again
dotnet build
```

**If still failing**:
```powershell
# Clear NuGet cache
dotnet nuget locals all --clear

# Remove bin and obj folders
Remove-Item -Recurse -Force bin, obj -ErrorAction SilentlyContinue

# Restore and build
dotnet restore
dotnet build
```

---

### **Issue 3: Batch File Not Working**

**Problem**: `start-project.bat` doesn't run or shows nothing

**Solutions**:

**Method A: Run from Command Prompt**
```cmd
cd "c:\Users\HP\Downloads\HVI\StudentManagementSystem"
start-project.bat
```

**Method B: Run as Administrator**
```cmd
# Right-click start-project.bat → "Run as Administrator"
```

**Method C: Manual Execution**
```powershell
cd "c:\Users\HP\Downloads\HVI\StudentManagementSystem"
powershell -ExecutionPolicy Bypass -File .\start-project.bat
```

---

### **Issue 4: Port 5000 Already in Use**

**Problem**: Application can't start on port 5000

**Solutions**:
```powershell
# Check what's using port 5000
netstat -ano | findstr :5000

# Kill the process (replace PID with actual process ID)
taskkill /F /PID [PID_NUMBER]

# Or use a different port
dotnet run --urls "http://localhost:5001"
```

---

### **Issue 5: Database Connection Errors**

**Problem**: 500 Internal Server Error or database connection issues

**Solutions**:
```powershell
# Check SQL Server is running
Get-Service MSSQLSERVER

# Test database connection
# Visit: http://localhost:5000/Student/TestDataPersistence

# If connection fails, try different connection string
# Edit appsettings.json:
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentManagementDB;Trusted_Connection=true;TrustServerCertificate=true"
```

---

## 🚀 **Complete Startup Process (Step-by-Step)**

### **Method 1: Automated (Recommended)**
```cmd
# Navigate to project folder
cd "c:\Users\HP\Downloads\HVI\StudentManagementSystem"

# Double-click start-project.bat
# OR run from command line:
start-project.bat
```

### **Method 2: Manual (If automated fails)**
```powershell
# 1. Open PowerShell as Administrator
# 2. Navigate to project
cd "c:\Users\HP\Downloads\HVI\StudentManagementSystem"

# 3. Check SQL Server
Get-Service MSSQLSERVER | Start-Service

# 4. Clean and build
dotnet clean
dotnet restore
dotnet build

# 5. Run application
dotnet run --urls "http://localhost:5000"
```

### **Method 3: Visual Studio**
```
1. Open Visual Studio as Administrator
2. Open HVI.sln
3. Set StudentManagementSystem as startup project
4. Press F5 or click Start
```

---

## ✅ **Verification Steps**

### **1. Check SQL Server Status**
```powershell
Get-Service MSSQLSERVER
# Status should be "Running"
```

### **2. Check Application is Running**
```powershell
netstat -an | findstr :5000
# Should show: TCP 127.0.0.1:5000 LISTENING
```

### **3. Test Database Connection**
Visit: `http://localhost:5000/Student/TestDataPersistence`
Should show:
```json
{
  "Success": true,
  "Message": "Database connection successful",
  "StudentCount": [number],
  "DepartmentCount": 5
}
```

### **4. Test Main Pages**
- Home: `http://localhost:5000`
- Admin Dashboard: `http://localhost:5000/Student/AdminDashboard`
- Add Student: `http://localhost:5000/Student/CreateStudent`

---

## 🔧 **Emergency Recovery**

If nothing works, try this complete reset:

```powershell
# 1. Kill all processes
taskkill /F /IM dotnet.exe
taskkill /F /IM StudentManagementSystem.exe

# 2. Navigate to project
cd "c:\Users\HP\Downloads\HVI\StudentManagementSystem"

# 3. Complete clean
Remove-Item -Recurse -Force bin, obj -ErrorAction SilentlyContinue
dotnet nuget locals all --clear

# 4. Fresh build
dotnet restore --force
dotnet build

# 5. Start application
dotnet run --urls "http://localhost:5000"
```

---

## 📞 **Quick Diagnostic Commands**

```powershell
# Check .NET version
dotnet --version

# Check SQL Server services
Get-Service | Where-Object {$_.Name -like "*SQL*"}

# Check running processes
tasklist | findstr dotnet

# Check port usage
netstat -an | findstr :5000

# Check project files
dir "c:\Users\HP\Downloads\HVI\StudentManagementSystem"
```

---

## 🎯 **Success Indicators**

✅ **Everything is working when:**
- SQL Server shows "Running" status
- `dotnet build` completes with "Build succeeded"
- Port 5000 shows "LISTENING" in netstat
- Home page loads at http://localhost:5000
- Admin Dashboard loads without errors
- TestDataPersistence shows "Success": true

**Your Student Management System is now running perfectly!** 🚀
