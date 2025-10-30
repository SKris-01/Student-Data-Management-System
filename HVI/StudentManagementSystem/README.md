# Student Management System

A comprehensive web application built with ASP.NET Core MVC for managing student records, academic information, and administrative tasks.

## Features

- **Student Registration**: Secure signup process with comprehensive profile creation
- **User Authentication**: Login/logout functionality with role-based access
- **Student Dashboard**: Personal information display with academic progress tracking
- **Admin Search**: Department-wise student search functionality (Admin only)
- **Responsive Design**: Modern UI with Bootstrap and Font Awesome icons
- **Database Integration**: SQL Server database with Entity Framework Core

## Database Schema

### Tables Created:
1. **tblStudent**: Stores student information (Id, Name, Username, Role, Course, Semester, CGPA, DOB, Hometown, Password, DepartmentId)
2. **tblDepartment**: Stores department information (DepartmentId, DepartmentName)
3. **ASP.NET Identity Tables**: For user authentication and authorization

### Pre-seeded Departments:
- Computer Science
- Information Technology
- Electronics Engineering
- Mechanical Engineering
- Civil Engineering

## Prerequisites

- .NET 8.0 SDK
- SQL Server or SQL Server Express
- SQL Server Management Studio (SSMS)
- Visual Studio 2022 or VS Code

## Setup Instructions

### 1. Database Setup

1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Create a new database named `StudentManagementDB` or run the provided SQL script
4. Execute the SQL script located at `database-script.sql` to create all tables and seed data

### 2. Connection String Configuration

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=StudentManagementDB;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

Replace `YOUR_SERVER_NAME` with your SQL Server instance name (e.g., `localhost`, `.\SQLEXPRESS`, etc.)

### 3. Running the Application

1. Open terminal/command prompt in the project directory
2. Run the following commands:

```bash
dotnet restore
dotnet build
dotnet run
```

3. Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`

## Usage

### For Students:
1. **Register**: Click "Register" and fill in all required information
2. **Login**: Use your username and password to log in
3. **Dashboard**: View your personal and academic information
4. **Logout**: Use the dropdown menu to log out

### For Admins:
1. **Register**: Register with Role set to "Admin"
2. **Login**: Log in with admin credentials
3. **Dashboard**: View your information and access admin features
4. **Search**: Use the search functionality to find students by department
5. **View Results**: Browse through search results with detailed student information

## Project Structure

```
StudentManagementSystem/
├── Controllers/
│   ├── AccountController.cs      # Authentication logic
│   ├── HomeController.cs         # Home page
│   └── StudentController.cs      # Student dashboard and search
├── Data/
│   └── ApplicationDbContext.cs   # Database context
├── Models/
│   ├── Department.cs             # Department model
│   ├── Student.cs                # Student and ApplicationUser models
│   └── ErrorViewModel.cs         # Error handling
├── ViewModels/
│   └── RegisterViewModel.cs      # Form view models
├── Views/
│   ├── Account/
│   │   ├── Login.cshtml          # Login page
│   │   └── Register.cshtml       # Registration page
│   ├── Home/
│   │   └── Index.cshtml          # Home page
│   ├── Student/
│   │   ├── Details.cshtml        # Student dashboard
│   │   └── Search.cshtml         # Admin search results
│   └── Shared/
│       └── _Layout.cshtml        # Main layout
└── database-script.sql           # Database creation script
```

## Technologies Used

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: HTML5, CSS3, Bootstrap 5, Font Awesome
- **ORM**: Entity Framework Core 8.0

## Security Features

- Password hashing and validation
- Role-based authorization
- CSRF protection
- Secure authentication cookies
- Input validation and sanitization

## Future Enhancements

- Email verification for registration
- Password reset functionality
- File upload for student photos
- Grade management system
- Attendance tracking
- Report generation
- API endpoints for mobile app integration

## Support

For any issues or questions, please check the code comments or create an issue in the project repository.
