# TODO List Application - Getting Started Guide

A full-stack TODO list application built with **.NET 10** backend and **Angular 21** frontend.

---

## Table of Contents
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Project Structure](#project-structure)
- [Running the Application](#running-the-application)
- [Running Tests](#running-tests)
---

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software

1. **.NET 10 SDK**
   - Version: 10.0.101 or later
   - Download: https://dotnet.microsoft.com/download/dotnet/10.0
   - Verify installation: `dotnet --version`

2. **Node.js**
   - Version: 24.12.0 LTS or later
   - Download: https://nodejs.org/
   - Verify installation: `node --version`

3. **npm**
   - Version: 11.6.2 or later (comes with Node.js)
   - Verify installation: `npm --version`

### Optional (Recommended)

- **Visual Studio 2022** (Preview/Latest) or **Visual Studio Code**
- **Git** for version control
- **Chrome** browser (for Angular testing)

---

## Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/larynuon/todoList.git
cd todoList
```

### 2. Install Dependencies

**Frontend (Angular):**
```bash
cd todolist.client
npm install
cd ..
```

**Backend (.NET):**
```bash
cd todoList.Server
dotnet restore
cd ..
```

### 3. Run the Application

**Option A: Using Visual Studio**
1. Open `todoList.sln` (or open the folder in Visual Studio)
2. Set `todoList.Server` as the startup project!!
3. Press **F5** (Start Debugging) or **Ctrl+F5** (Start Without Debugging)
4. Browser opens automatically at `https://localhost:7284`

**Option B: Using Command Line**
```bash
cd todoList.Server
dotnet run --launch-profile https
```

The application will:
- Start the .NET backend on `https://localhost:7284`
- Automatically launch the Angular dev server on `https://localhost:4200`
- Open your default browser to the application
- Navigate to `https://localhost:7284` in your browser.
---

## Running Tests

### Backend Tests (.NET)

**Run all tests:**
```bash
dotnet test
```

**Run specific test class:**
```bash
dotnet test --filter "FullyQualifiedName~TodoServiceTests"
```

**Run with detailed output:**
```bash
dotnet test --logger "console;verbosity=detailed"
```

**? Expected: 28 tests passing**
