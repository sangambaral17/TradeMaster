# 📚 Walsong TradeMaster Developer Documentation

## 🎯 Quick Start for Developers

Welcome to Walsong TradeMaster! This guide will help you understand and work with the codebase.

---

## 📖 Essential Reading

### **[CODE_WALKTHROUGH.md](CODE_WALKTHROUGH.md)** ⭐ **START HERE!**
**Complete architecture and code flow explanation**
- How the application starts (App.xaml.cs → Database → MainWindow)
- Project structure and dependency flow
- MVVM pattern with real examples
- Step-by-step feature walkthroughs
- Debugging guide for common issues

### **[README.md](README.md)**
**Project overview and setup instructions**

### **[PRODUCT_DOCUMENTATION.md](PRODUCT_DOCUMENTATION.md)**
**User-facing features and functionality**

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    TradeMaster.Desktop                   │
│              (WPF UI Layer - MVVM Pattern)              │
│   Views/ ViewModels/ Services/ Converters/ Helpers/    │
└────────────────────┬────────────────────────────────────┘
                     │ Uses
                     ▼
┌─────────────────────────────────────────────────────────┐
│              TradeMaster.Infrastructure                  │
│           (Data Access Layer - EF Core)                 │
│        Data/ Migrations/ Repositories/                  │
└────────────────────┬────────────────────────────────────┘
                     │ Implements
                     ▼
┌─────────────────────────────────────────────────────────┐
│                  TradeMaster.Core                        │
│         (Domain Layer - Pure Business Logic)            │
│            Entities/ Interfaces/                        │
└─────────────────────────────────────────────────────────┘
```

**Dependency Rule**: Core has NO dependencies. Infrastructure depends on Core. Desktop depends on both.

---

## 🚀 How to Run

1. **Prerequisites**: .NET 9.0 SDK installed
2. **Build**: `dotnet build TradeMaster.sln`
3. **Run**: `dotnet run --project TradeMaster.Desktop`
4. **Database**: SQLite file `trademaster.db` auto-created on first run

---

## 🔑 Key Concepts

### 1. Dependency Injection (DI)
All services, repositories, and views are registered in `App.xaml.cs`:
```csharp
services.AddDbContext<TradeMasterDbContext>();
services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
services.AddTransient<ProductListViewModel>();
```

### 2. MVVM Pattern
- **Model**: Entities (Product, Category, Sale)
- **View**: XAML files (ProductListView.xaml)
- **ViewModel**: Logic + Data Binding (ProductListViewModel.cs)

### 3. Repository Pattern
Generic `IRepository<T>` interface for all CRUD operations:
```csharp
await _productRepository.GetAllAsync();
await _productRepository.AddAsync(product);
```

### 4. Entity Framework Core
- Database: SQLite (`trademaster.db`)
- Migrations: `TradeMaster.Infrastructure/Migrations/`
- DbContext: `TradeMasterDbContext.cs`

---

## 📂 Project Structure

```
TradeMaster/
├── 📄 CODE_WALKTHROUGH.md          ⭐ Complete code explanation
├── 📄 README.md                     Project overview
├── 📄 PRODUCT_DOCUMENTATION.md      Feature documentation
│
├── 📁 TradeMaster.Core/             Domain layer
│   ├── Entities/                    Business entities
│   │   ├── Product.cs
│   │   ├── Category.cs
│   │   ├── Sale.cs
│   │   └── Customer.cs
│   └── Interfaces/
│       └── IRepository.cs
│
├── 📁 TradeMaster.Infrastructure/   Data layer
│   ├── Data/
│   │   ├── TradeMasterDbContext.cs  EF Core DbContext
│   │   ├── EfRepository.cs          Generic repository
│   │   └── DbInitializer.cs         Database setup
│   └── Migrations/                  EF Core migrations
│
└── 📁 TradeMaster.Desktop/          UI layer
    ├── App.xaml.cs                  ⭐ Application entry point
    ├── MainWindow.xaml.cs           Dashboard & navigation
    ├── Views/                       XAML views
    │   ├── ProductListView.xaml
    │   ├── ProductEditDialog.xaml
    │   ├── CategoryManagementDialog.xaml
    │   └── SettingsView.xaml
    ├── ViewModels/                  MVVM view models
    │   ├── ProductListViewModel.cs
    │   └── SettingsViewModel.cs
    └── Services/                    Application services
        ├── SettingsService.cs
        └── LocalizationService.cs
```

---

## 🛠️ Common Development Tasks

### Adding a New Feature

1. **Create Entity** (if needed) in `TradeMaster.Core/Entities/`
2. **Add DbSet** to `TradeMasterDbContext.cs`
3. **Create Migration**: `dotnet ef migrations add FeatureName`
4. **Create View** in `TradeMaster.Desktop/Views/`
5. **Create ViewModel** in `TradeMaster.Desktop/ViewModels/`
6. **Register in DI** in `App.xaml.cs`
7. **Add Navigation** in `MainWindow.xaml.cs`

### Debugging Tips

- **Database Issues**: Delete `trademaster.db` and restart
- **UI Not Updating**: Ensure ViewModel inherits `ObservableObject`
- **DI Errors**: Check service registration in `App.xaml.cs`
- **Build Errors**: Run `dotnet clean` then `dotnet build`

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project TradeMaster.Infrastructure

# Update database
dotnet ef database update --project TradeMaster.Desktop

# Remove last migration
dotnet ef migrations remove --project TradeMaster.Infrastructure
```

---

## 🎨 Recent Updates

### Multi-Currency Support (Latest)
- Products can be priced in USD ($), AUD (A$), or NPR (Rs.)
- Currency selector in product edit dialog
- See `ProductEditDialog.xaml` for implementation

### Category Management
- Dynamic category creation from product dialog
- CRUD operations via `CategoryManagementDialog`
- Auto-refresh category dropdown

### Settings Module
- Language selection: English / Nepali
- Currency symbol configuration
- Persistent settings via JSON file

---

## 📞 Need Help?

1. **Read**: [CODE_WALKTHROUGH.md](CODE_WALKTHROUGH.md) - Complete explanation
2. **Check**: Debugging section in walkthrough
3. **Search**: Look for similar code in existing features
4. **Ask**: Contact the development team

---

## 🔐 Important Notes

- **Database**: SQLite file-based, located at project root
- **Default Login**: Username: `admin`, Password: `admin123`
- **Settings File**: `appsettings.json` in application directory
- **Build Configuration**: Use Release for deployment

---

## 📝 Code Standards

- Use **MVVM pattern** for all UI features
- Follow **Repository pattern** for data access
- Use **async/await** for all database operations
- Implement **INotifyPropertyChanged** via `ObservableObject`
- Add **XML comments** for public APIs
- Write **unit tests** for business logic

---

**Happy Coding! 🚀**

For detailed code explanations, see [CODE_WALKTHROUGH.md](CODE_WALKTHROUGH.md)
