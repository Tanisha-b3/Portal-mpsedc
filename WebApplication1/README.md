# Appointment Scheduler - ASP.NET Core MVC Project

A complete appointment scheduling application built with ASP.NET Core MVC (.NET 10.0). This project demonstrates modern web development patterns including MVC architecture, dependency injection, services, and CRUD operations.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [File Structure](#file-structure)
4. [Code Explanation - Models](#code-explanation---models)
5. [Code Explanation - Services](#code-explanation---services)
6. [Code Explanation - Controller](#code-explanation---controller)
7. [Code Explanation - Views](#code-explanation---views)
8. [Code Explanation - Program.cs](#code-explanation---programcs)
9. [How to Run](#how-to-run)
10. [Features](#features)

---

## Project Overview

This is a **web application** that allows users to:
- Create, view, edit, and delete appointments
- Search appointments by keyword
- Filter appointments by status (upcoming, ongoing, completed, past)
- Mark appointments as complete/incomplete
- View statistics dashboard

**Tech Stack:**
- **.NET 10.0** - Runtime
- **ASP.NET Core MVC** - Web framework
- **Razor Views** - Server-side templating
- **Bootstrap 5** - CSS framework
- **Bootstrap Icons** - Icon library
- **In-Memory Storage** - Data stored in a list (no database needed)

---

## Architecture

This project follows the **MVC (Model-View-Controller)** pattern:

```
┌─────────────────────────────────────────────────────────────┐
│                      USER (Browser)                         │
└─────────────────────────┬───────────────────────────────────┘
                          │ HTTP Request
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                    CONTROLLER                                │
│  (AppointmentsController.cs)                                │
│  - Receives HTTP requests                                   │
│  - Processes user input                                     │
│  - Calls service methods                                    │
│  - Returns views or redirects                               │
└─────────────────────────┬───────────────────────────────────┘
                          │ Calls methods
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                     SERVICE                                  │
│  (IAppointmentService / InMemoryAppointmentService)         │
│  - Contains business logic                                  │
│  - Manages data (CRUD operations)                           │
│  - Can be swapped (e.g., database) without changing code   │
└─────────────────────────┬───────────────────────────────────┘
                          │ Uses
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                      MODEL                                   │
│  (Appointment.cs, ViewModels)                               │
│  - Defines data structure                                   │
│  - Contains validation rules                                │
│  - Represents the "shape" of data                           │
└─────────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                      VIEW                                    │
│  (Index.cshtml, Create.cshtml, etc.)                        │
│  - User interface (HTML)                                    │
│  - Displays data from models                                │
│  - Contains forms for user input                            │
└─────────────────────────────────────────────────────────────┘
```

---

## File Structure

```
WebApplication1/
├── Controllers/
│   ├── HomeController.cs          # Home page controller
│   └── AppointmentsController.cs  # Appointment CRUD operations
├── Models/
│   ├── Appointment.cs             # Main appointment entity
│   ├── CreateAppointmentViewModel.cs  # ViewModel for creation
│   ├── EditAppointmentViewModel.cs    # ViewModel for editing
│   └── ErrorViewModel.cs         # Error handling model
├── Services/
│   ├── IAppointmentService.cs     # Service interface (contract)
│   └── InMemoryAppointmentService.cs  # In-memory data storage
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml          # Home page
│   │   └── Privacy.cshtml        # Privacy page
│   ├── Appointments/
│   │   ├── Index.cshtml          # Appointment list/dashboard
│   │   ├── Create.cshtml         # Create form
│   │   ├── Edit.cshtml           # Edit form
│   │   ├── Details.cshtml        # View details
│   │   └── Delete.cshtml         # Delete confirmation
│   └── Shared/
│       ├── _Layout.cshtml        # Master layout (navbar, footer)
│       ├── _Layout.cshtml.css    # Layout-specific CSS
│       └── _ViewImports.cshtml   # Global tag helpers
├── wwwroot/
│   ├── css/
│   │   └── site.css              # Custom styles
│   ├── js/
│   │   └── site.js               # Custom JavaScript
│   └── lib/                      # Third-party libraries (Bootstrap, jQuery)
├── Program.cs                     # Application entry point
├── WebApplication1.csproj        # Project configuration
└── appsettings.json              # Application settings
```

---

## Code Explanation - Models

### 1. `Models/Appointment.cs` - The Main Entity

This file defines the **data structure** for an appointment. Think of it as a blueprint.

```csharp
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;
```

**Line-by-line explanation:**

| Line | Code | What it Means |
|------|------|---------------|
| 1 | `using System.ComponentModel.DataAnnotations;` | Imports attributes for data validation (like `[Required]`) |
| 3 | `namespace WebApplication1.Models;` | Groups this class under the "Models" namespace |

```csharp
public class Appointment
{
    public int Id { get; set; }
```

- `public class Appointment` - Creates a class named "Appointment"
- `public int Id { get; set; }` - Auto-property for unique identifier
  - `public` = accessible from anywhere
  - `int` = whole number type
  - `{ get; set; }` = auto-generated getter and setter (read/write)

```csharp
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "...")]
    public string Title { get; set; } = string.Empty;
```

- `[Required]` = Validation attribute - field cannot be empty
- `[StringLength(100, MinimumLength = 3)]` = Must be 3-100 characters
- `string.Empty` = Default value is empty string (not null)

```csharp
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; } = DateTime.Now.AddHours(1);
```

- `DateTime` = Date and time type
- `DateTime.Now` = Current date/time
- `.AddHours(1)` = Add 1 hour to current time

```csharp
    public AppointmentPriority Priority { get; set; } = AppointmentPriority.Medium;
```

- Uses an `enum` (enumeration) for predefined values
- Default is "Medium"

```csharp
    public bool IsUpcoming => StartDate > DateTime.Now && !IsCompleted;
    public bool IsOngoing => StartDate <= DateTime.Now && EndDate >= DateTime.Now && !IsCompleted;
    public bool IsPast => EndDate < DateTime.Now || IsCompleted;
```

- These are **computed properties** (read-only, calculated on access)
- `=>` = Expression-bodied member (shorthand for a property that only returns a value)
- `&&` = AND operator
- `||` = OR operator
- `!` = NOT operator (negation)

### 2. `Models/CreateAppointmentViewModel.cs` - View Model

ViewModels are used to **transfer data between Controller and View**. They often have validation but are not the database entity.

```csharp
public Appointment ToAppointment()
{
    return new Appointment
    {
        Title = Title,
        Description = Description,
        // ... other properties
    };
}
```

- This method **converts** the ViewModel to an actual Appointment entity
- Uses **object initializer** syntax to set properties

---

## Code Explanation - Services

### 3. `Services/IAppointmentService.cs` - Interface (Contract)

An interface defines **what methods must exist** but not how they work. It's like a menu at a restaurant - it lists what's available but doesn't show how the food is made.

```csharp
public interface IAppointmentService
{
    IEnumerable<Appointment> GetAll();
    Appointment? GetById(int id);
    IEnumerable<Appointment> GetByDateRange(DateTime start, DateTime end);
    Appointment Create(Appointment appointment);
    Appointment? Update(Appointment appointment);
    bool Delete(int id);
    bool ToggleComplete(int id);
}
```

| Term | Meaning |
|------|---------|
| `interface` | A contract that classes must implement |
| `IEnumerable<T>` | A collection that can be iterated (looped through) |
| `Appointment?` | The `?` means it can be null (might not find the appointment) |
| `int id` | Parameter - a number passed to the method |
| `bool` | Returns true or false |

### 4. `Services/InMemoryAppointmentService.cs` - Implementation

This is the **actual code** that implements the interface. It stores data in a `List<T>` (in memory).

```csharp
public class InMemoryAppointmentService : IAppointmentService
{
    private readonly List<Appointment> _appointments = new();
    private int _nextId = 1;
```

- `: IAppointmentService` = Implements the interface
- `private` = Only accessible within this class
- `readonly` = Cannot be changed after initialization
- `List<Appointment>` = A dynamic array that holds Appointment objects
- `_appointments` = The underscore prefix is a convention for private fields
- `_nextId` = Counter for generating unique IDs

```csharp
    public IEnumerable<Appointment> GetAll()
    {
        return _appointments.OrderByDescending(a => a.StartDate);
    }
```

- `_appointments.OrderByDescending(a => a.StartDate)` = Sorts by date (newest first)
- `a => a.StartDate` = **Lambda expression** - an anonymous function
  - `a` is the parameter (each appointment)
  - `=>` means "goes to" or "becomes"
  - `a.StartDate` is what we sort by

```csharp
    public Appointment Create(Appointment appointment)
    {
        appointment.Id = _nextId++;
        appointment.CreatedAt = DateTime.Now;
        _appointments.Add(appointment);
        return appointment;
    }
```

- `_nextId++` = Assigns current value, then increments
- `.Add()` = Adds to the list

```csharp
    public bool ToggleComplete(int id)
    {
        var appointment = GetById(id);
        if (appointment == null) return false;
        
        appointment.IsCompleted = !appointment.IsCompleted;
        return true;
    }
```

- `var` = Let the compiler figure out the type
- `!appointment.IsCompleted` = Toggle (flip) the boolean value

---

## Code Explanation - Controller

### 5. `Controllers/AppointmentsController.cs` - Request Handler

The controller handles **HTTP requests** and decides what to do.

```csharp
public class AppointmentsController : Controller
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }
```

- `: Controller` = Inherits from base Controller class
- **Constructor Injection** = Service is passed in automatically by ASP.NET Core
- This is **Dependency Injection** - the controller doesn't create the service, it receives it

```csharp
    // GET: Appointments
    public IActionResult Index(string? search, string? filter)
    {
        IEnumerable<Appointment> appointments;

        if (!string.IsNullOrWhiteSpace(search))
        {
            appointments = _appointmentService.Search(search);
            ViewBag.SearchQuery = search;
        }
        else
        {
            appointments = _appointmentService.GetAll();
        }
```

- `IActionResult` = Return type for actions that return a view
- `string?` = Nullable string (can be null)
- `ViewBag` = Pass data from Controller to View (temporary, dynamic)

```csharp
        return View(appointments.ToList());
    }
```

- `View()` = Returns a Razor view
- `.ToList()` = Converts IEnumerable to List

```csharp
    // POST: Appointments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateAppointmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            var appointment = model.ToAppointment();
            _appointmentService.Create(appointment);
            TempData["SuccessMessage"] = "Appointment created successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }
```

- `[HttpPost]` = This action only responds to POST requests
- `[ValidateAntiForgeryToken]` = Security feature to prevent CSRF attacks
- `ModelState.IsValid` = Checks if all validation rules passed
- `TempData` = Pass data between requests (survives a redirect)
- `RedirectToAction()` = Redirects to another action (PRG pattern)

### HTTP Methods Explained:

| Method | Purpose | Example |
|--------|---------|---------|
| `GET` | Retrieve data | View list of appointments |
| `POST` | Create/submit data | Submit create form |
| `PUT` | Update data | Edit an appointment |
| `DELETE` | Remove data | Delete an appointment |

---

## Code Explanation - Views

### 6. `Views/Appointments/Index.cshtml` - Main Dashboard

```html
@model List<WebApplication1.Models.Appointment>
@{
    ViewData["Title"] = "Appointments";
}
```

- `@model` = Specifies what data type the view expects
- `@{ }` = Razor code block (C# code in the view)
- `ViewData["Title"]` = Sets the page title (used in layout)

```html
<div class="d-flex justify-content-between align-items-center mb-4">
    <h1><i class="bi bi-calendar-event"></i> Appointments</h1>
    <a asp-action="Create" class="btn btn-primary">
        <i class="bi bi-plus-circle"></i> New Appointment
    </a>
</div>
```

- `asp-action` = Tag helper - generates URL for the "Create" action
- `class="btn btn-primary"` = Bootstrap CSS classes for styling
- `<i class="bi bi-calendar-event"></i>` = Bootstrap Icon

### Tag Helpers Explained:

| Tag Helper | Generates |
|------------|-----------|
| `asp-action="Create"` | `href="/Appointments/Create"` |
| `asp-controller="Home"` | Points to HomeController |
| `asp-route-id="5"` | Adds route parameter `id=5` |

```html
@if (Model.Any())
{
    @foreach (var appointment in Model)
    {
        <div class="card">
            <h5>@appointment.Title</h5>
        </div>
    }
}
else
{
    <p>No appointments found.</p>
}
```

- `@if ( ) { }` = Conditional rendering
- `@foreach (var appointment in Model)` = Loop through each appointment
- `@appointment.Title` = Output the Title property

### 7. `Views/Appointments/Create.cshtml` - Form

```html
<form asp-action="Create">
    <div asp-validation-summary="ModelOnly" class="text-danger mb-3"></div>

    <div class="mb-3">
        <label asp-for="Title" class="form-label"></label>
        <input asp-for="Title" class="form-control" />
        <span asp-validation-for="Title" class="text-danger"></span>
    </div>
    
    <button type="submit" class="btn btn-primary">Create</button>
</form>
```

- `<form asp-action="Create">` = Form submits to Create action
- `asp-validation-summary="ModelOnly"` = Shows model-level errors
- `asp-for="Title"` = Binds input to Title property
- `asp-validation-for="Title"` = Shows field-level validation errors
- `type="submit"` = Button that submits the form

---

## Code Explanation - Program.cs

### 8. `Program.cs` - Application Entry Point

This is where the application **starts** and **configures** services.

```csharp
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);
```

- `WebApplication.CreateBuilder(args)` = Creates a builder for configuring the app
- `args` = Command-line arguments

```csharp
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IAppointmentService, InMemoryAppointmentService>();
```

- `AddControllersWithViews()` = Registers MVC services
- `AddSingleton<TService, TImplementation>()` = Registers a service
  - **Singleton** = One instance for the entire application lifetime
  - Other options: `AddTransient` (new instance each time), `AddScoped` (one per request)

```csharp
var app = builder.Build();
```

- `Build()` = Creates the application instance

```csharp
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

- `UseHttpsRedirection()` = Forces HTTPS
- `UseRouting()` = Enables routing
- `UseAuthorization()` = Enables authorization
- `MapControllerRoute()` = Defines URL pattern
  - `{controller=Home}` = Default controller is "Home"
  - `{action=Index}` = Default action is "Index"
  - `{id?}` = Optional id parameter

```csharp
app.Run();
```

- Starts the web server

---

## How to Run

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later

### Steps

```bash
# 1. Navigate to project directory
cd E:\php\WebApplication1

# 2. Build the project
dotnet build

# 3. Run the application
dotnet run
```

### Access the App

- **Home Page:** `https://localhost:5001` or `http://localhost:5000`
- **Appointments:** `https://localhost:5001/Appointments`

---

## Features

### 1. Dashboard Statistics
- Total appointments count
- Upcoming appointments count
- Completed appointments count
- Today's appointments count

### 2. CRUD Operations
- **Create** - Add new appointments with title, description, dates, location, priority
- **Read** - View list or details of appointments
- **Update** - Edit existing appointments
- **Delete** - Remove appointments with confirmation

### 3. Search & Filter
- Search by title, description, or location
- Filter by status: All, Upcoming, Ongoing, Completed, Past

### 4. Priority System
- Low (Gray)
- Medium (Blue)
- High (Yellow)
- Urgent (Red)

### 5. Status Indicators
- **Upcoming** - Future appointments (Blue badge)
- **Ongoing** - Currently happening (Yellow badge)
- **Completed** - Finished (Green badge)
- **Past** - Ended (Gray badge)

### 6. Toggle Complete
- One-click to mark appointment as complete/incomplete
- Visual feedback with color changes

---

## Key Concepts Summary

| Concept | What It Is | Example |
|---------|-----------|---------|
| **MVC** | Model-View-Controller pattern | Separates data, UI, and logic |
| **Dependency Injection** | Services are passed to classes | Controller receives service |
| **Interface** | Contract for implementations | `IAppointmentService` |
| **ViewModel** | Data transfer object | `CreateAppointmentViewModel` |
| **Tag Helpers** | HTML helpers in Razor | `asp-action`, `asp-for` |
| **Razor** | Server-side templating language | `@model`, `@if`, `@foreach` |
| **CRUD** | Create, Read, Update, Delete | Basic database operations |
| **Singleton** | One instance per app | Service registered as Singleton |
| **Validation** | Data validation attributes | `[Required]`, `[StringLength]` |

---

## Troubleshooting

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### Port Already in Use
```bash
# Kill process on port 5000
netstat -ano | findstr :5000
taskkill /PID <process_id> /F
```

### Run on Different Port
```bash
dotnet run --urls "http://localhost:8080"
```

---

## License

This project is for educational purposes. Feel free to use and modify.

---

**Created with ASP.NET Core MVC | .NET 10.0**
