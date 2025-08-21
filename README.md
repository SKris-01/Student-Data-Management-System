# 🎓 Student Data Management System

## 📌 Project Overview

The **Student Data Management System (SDMS)** is a web-based application developed using **.NET MVC** and **Microsoft SQL Server** for backend data storage. The system is designed to manage student information efficiently, allowing **students to register, login, and view their details**, while providing **administrators with the ability to manage and search student records department-wise**.

This project demonstrates the application of **MVC architecture, relational databases, authentication, and role-based access control**, making it an ideal academic and professional project for understanding web application development with a database backend.

---

## 🎯 Assignment Purpose

The project aims to:

- Provide a **web interface** for student registration and login.
- Store and manage student data using a **SQL Server database**.
- Implement **role-based access control**, distinguishing between student and admin functionalities.
- Allow admins to **search student records by department**.
- Display **personalized information** (e.g., logged-in username) dynamically.

This project is suitable for students and professionals who want to learn **full-stack web development with .NET MVC and SQL Server integration**.

---

## 🛠️ Technologies Used

| Component | Technology / Tool |
|-----------|------------------|
| Web Framework | .NET MVC |
| Programming Language | C# |
| Database | Microsoft SQL Server / SQL Server Management Studio (SSMS) |
| Development Environment | Visual Studio 2022 |
| Authentication & Roles | MVC Identity or Custom Login Logic |
| Frontend | HTML, CSS, Bootstrap (optional for styling) |
| ORM / Data Access | Entity Framework or ADO.NET |

---

## 📂 Database Design

The database consists of **two primary tables**:

### 1. `tblDepartment`
| Column Name      | Data Type | Description |
|-----------------|-----------|------------|
| DepartmentId     | INT (PK)  | Unique identifier for department |
| DepartmentName   | NVARCHAR  | Name of the department |

### 2. `tblStudent`
| Column Name      | Data Type | Description |
|-----------------|-----------|------------|
| Id               | INT (PK)  | Unique identifier for student |
| Name             | NVARCHAR  | Student's full name |
| Username         | NVARCHAR  | Login username |
| Role             | NVARCHAR  | User role (`Admin` / `Student`) |
| Course           | NVARCHAR  | Enrolled course |
| Semester         | INT       | Current semester |
| CGPA             | DECIMAL   | Current CGPA |
| DOB              | DATE      | Date of birth |
| Hometown         | NVARCHAR  | Hometown |
| Password         | NVARCHAR  | User password |
| DepartmentId     | INT (FK)  | References `DepartmentId` in `tblDepartment` |

> **Relationships:** `DepartmentId` in `tblStudent` is a foreign key referencing `tblDepartment.DepartmentId`.

---

## 🔑 Key Features

### Student Module
- **Signup / Registration**: New students can register by providing personal and academic details.
- **Login**: Students can log in to access their personal details.
- **Student Details Page**: Displays the logged-in student's information.
- **Personalized Experience**: Displays login username on the top-right corner.

### Admin Module
- **Admin Login**: Users with `Admin` role can access administrative functionalities.
- **View & Search Students**: Admins can search for students by `DepartmentName`.
- **Department-wise Search**: Provides filtering of students based on department.
- **Full Access**: Admins can view all student records.
