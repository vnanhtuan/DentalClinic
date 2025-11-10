# Dependency Injection Registration Guide

## Add these lines to your Program.cs

Add the following registrations to your `DentalClinic.Web/Program.cs` file:

### Step 1: Add using statements at the top
```csharp
using DentalClinic.Application.Interfaces.Branches;
using DentalClinic.Application.Services.Branches;
```

### Step 2: Register the Branch services (add after existing repository/service registrations)

```csharp
// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>(); // ← ADD THIS
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IBranchService, BranchService>(); // ← ADD THIS
```

## Complete Registration Block

Here's what the complete registration section should look like:

```csharp
using DentalClinic.Application.Interfaces;
using DentalClinic.Application.Interfaces.Staffs;
using DentalClinic.Application.Interfaces.Roles;
using DentalClinic.Application.Interfaces.Branches; // ← ADD THIS
using DentalClinic.Application.Services;
using DentalClinic.Application.Services.Staffs;
using DentalClinic.Application.Services.Roles;
using DentalClinic.Application.Services.Branches; // ← ADD THIS
using DentalClinic.Domain.Interfaces;
using DentalClinic.Infrastructure;
using DentalClinic.Infrastructure.Repositories;
using DentalClinic.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DentalClinicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>(); // ← NEW
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IBranchService, BranchService>(); // ← NEW

// ... rest of your configuration
```

## After Registration

Once you've added the registrations:

1. **Build the solution** to ensure everything compiles:
   ```powershell
   dotnet build
   ```

2. **Create and run the migration** to update your database:
   ```powershell
   cd c:\Projects\DentalClinic
   dotnet ef migrations add AddBranchManagement --project DentalClinic.Infrastructure --startup-project DentalClinic.Web
   dotnet ef database update --project DentalClinic.Infrastructure --startup-project DentalClinic.Web
   ```

3. **You can now inject and use the services** in your controllers:
   ```csharp
   public class BranchController : Controller
   {
       private readonly IBranchService _branchService;

       public BranchController(IBranchService branchService)
       {
           _branchService = branchService;
       }

       // ... your action methods
   }
   ```
