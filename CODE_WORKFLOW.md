# TradeMaster - Code Workflow & Architecture Guide

**Author**: Sangam Baral  
**Role**: Software Engineer  
**Version**: 1.0

---

## 🏗️ Architecture Overview

TradeMaster follows the **Clean Architecture** principles combined with the **MVVM (Model-View-ViewModel)** design pattern. This ensures a strict separation of concerns, making the code testable, maintainable, and scalable.

### High-Level Structure

The solution is divided into three distinct layers (Projects):

1.  **TradeMaster.Core** (The "Heart")
    *   Contains the business entities (e.g., `Product`, `Sale`, `Customer`).
    *   Defines Interfaces (e.g., `IRepository<T>`).
    *   *No external dependencies* (Pure C#).

2.  **TradeMaster.Infrastructure** (The "Plumbing")
    *   Implements the Interfaces defined in Core.
    *   Handles Database connections using **Entity Framework Core** (EF Core).
    *   Contains the `TradeMasterDbContext`.

3.  **TradeMaster.Desktop** (The "Face")
    *   The WPF Application (Frontend).
    *   Contains Views (XAML) and ViewModels (C# logic).
    *   Handles Dependency Injection (DI) to glue everything together.

---

## 🔄 Data Flow: How It Works

Here is the journey of a data request through the application:

```mermaid
graph TD
    User[👤 User Action] --> View[🖥️ View (UI/XAML)]
    View -->|Data Binding| ViewModel[🧠 ViewModel (Logic)]
    ViewModel -->|Calls Interface| Repository[🗄️ Repository (Data Access)]
    Repository -->|EF Core / ADO.NET| DB[(💾 SQLite Database)]
    DB -->|Results| Repository
    Repository -->|Entities| ViewModel
    ViewModel -->|Property Change| View
    View -->|Visual Update| User
```

---

## 🛠️ Detailed Component Breakdown

### 1. Frontend (The View)
*   **Technology**: WPF (Windows Presentation Foundation) with XAML.
*   **Role**: Displays data and captures user input.
*   **Key Concept**: **Data Binding**. The View never talks to the database directly. It only knows about the `ViewModel`.
*   **Example**: `CustomerListView.xaml` defines the grid and buttons, but doesn't know *how* to delete a customer. It just binds to the `DeleteCustomerCommand`.

### 2. Backend Logic (The ViewModel)
*   **Technology**: C# with CommunityToolkit.Mvvm.
*   **Role**: Holds the state of the view and handles logic.
*   **Key Concept**: **ObservableObject**. When data changes here (e.g., `_customers` collection), it automatically notifies the UI to update.
*   **Example**: `CustomerListViewModel.cs` has a method `LoadCustomers()`. It asks the Repository for data, puts it in a list, and the UI updates automatically.

### 3. Database Layer (EF Core & ADO.NET)
*   **Technology**: Entity Framework Core (EF Core) 9.0.
*   **Role**: Translates C# objects into SQL commands.
*   **How it relates to ADO.NET**:
    *   You asked about **ADO.NET**. EF Core is a modern wrapper *built on top of* ADO.NET.
    *   Instead of writing raw SQL (e.g., `SELECT * FROM Customers`), we write C# code: `_context.Customers.ToList()`.
    *   **Under the hood**, EF Core converts this C# code into a raw ADO.NET SQL command, opens a connection, executes the query, and maps the results back to C# objects.

---

## 👣 Step-by-Step Workflow Example

Let's trace exactly what happens when you click **"Save Customer"**:

### Step 1: User Interaction (Frontend)
*   **User** fills in the "Name" and "Email" fields in `CustomerEditDialog.xaml`.
*   **User** clicks the "Save" button.
*   **Event**: `SaveButton_Click` event is triggered in the code-behind.

### Step 2: Logic Processing (ViewModel/Code-Behind)
*   The code creates a new `Customer` entity object:
    ```csharp
    var newCustomer = new Customer { Name = "John Doe", Email = "john@example.com" };
    ```
*   The code calls the repository:
    ```csharp
    await _customerRepository.AddAsync(newCustomer);
    ```

### Step 3: Data Access (Infrastructure)
*   The `EfRepository` receives the `Customer` object.
*   It adds it to the `TradeMasterDbContext`:
    ```csharp
    _dbContext.Set<Customer>().Add(entity);
    ```
*   It saves changes:
    ```csharp
    await _dbContext.SaveChangesAsync();
    ```

### Step 4: Database Execution (EF Core -> SQL)
*   **Translation**: EF Core translates the C# object into an SQL `INSERT` statement.
*   **Execution**: It opens a connection to `trademaster.db` (SQLite).
*   **Commit**: The data is written to the disk.

### Step 5: UI Update (Return Trip)
*   The `AddAsync` method completes.
*   The dialog closes.
*   The `CustomerListViewModel` refreshes the list (`LoadCustomers`).
*   The **DataGrid** on the screen detects the new item and displays "John Doe".

---

## 🔌 Dependency Injection (The Glue)

How does the app know which database to use? This is handled in `App.xaml.cs`.

This is the "Startup" configuration where we wire everything together:

```csharp
// 1. Configure Database
services.AddDbContext<TradeMasterDbContext>(options => 
    options.UseSqlite("Data Source=trademaster.db"));

// 2. Link Interfaces to Implementations
// "Whenever a class asks for IRepository, give them EfRepository"
services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

// 3. Register ViewModels
services.AddTransient<CustomerListViewModel>();
```

This ensures that `CustomerListViewModel` doesn't need to know *how* to connect to the database; it just asks for a repository, and the system provides it.

---

## 📂 Folder Structure Guide

*   `TradeMaster.Core/`
    *   `Entities/` -> Defines *what* data we have (Product.cs, Customer.cs).
    *   `Interfaces/` -> Defines *what* operations we can do (IRepository.cs).
*   `TradeMaster.Infrastructure/`
    *   `Data/` -> Implements the database connection (DbContext, EfRepository).
    *   `Migrations/` -> History of database changes.
*   `TradeMaster.Desktop/`
    *   `Views/` -> The visual windows (XAML files).
    *   `ViewModels/` -> The logic behind the windows.
    *   `App.xaml.cs` -> The startup configuration.
