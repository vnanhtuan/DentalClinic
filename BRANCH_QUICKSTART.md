# Branch Implementation - Quick Reference

## 📁 Files Created

### Domain Layer (DentalClinic.Domain)
✅ `Entities/Branch.cs` - Main branch entity
✅ `Entities/UserBranchMapping.cs` - User-Branch-Role mapping table
✅ `Interfaces/IBranchRepository.cs` - Repository interface
- Updated: `Entities/User.cs` - Added UserBranches navigation
- Updated: `Entities/UserRole.cs` - Added UserBranchMappings navigation
- Updated: `Entities/Appointment.cs` - Added BranchId
- Updated: `Entities/Inventory.cs` - Added BranchId

### Infrastructure Layer (DentalClinic.Infrastructure)
✅ `Repositories/BranchRepository.cs` - Repository implementation
- Updated: `DentalClinicDbContext.cs` - Added Branches & UserBranchMappings DbSet + configurations

### Application Layer (DentalClinic.Application)
✅ `DTOs/Branches/BranchDto.cs`
✅ `DTOs/Branches/CreateBranchDto.cs`
✅ `DTOs/Branches/UpdateBranchDto.cs`
✅ `DTOs/Branches/UserBranchDto.cs`
✅ `DTOs/Branches/AssignUserToBranchDto.cs`
✅ `Interfaces/Branches/IBranchService.cs`
✅ `Services/Branches/BranchService.cs`

### Web Layer (DentalClinic.Web)
✅ `Controllers/BranchController.cs` - API controller with all CRUD operations

### Documentation
✅ `BRANCH_IMPLEMENTATION.md` - Detailed implementation guide
✅ `BRANCH_DIAGRAM.md` - Visual diagrams and examples
✅ `REGISTRATION_GUIDE.md` - DI registration instructions

---

## 🚀 Next Steps to Complete Integration

### 1. Register Services in Program.cs
Add to `DentalClinic.Web/Program.cs`:

```csharp
// Add using statements
using DentalClinic.Application.Interfaces.Branches;
using DentalClinic.Application.Services.Branches;

// Add repository
builder.Services.AddScoped<IBranchRepository, BranchRepository>();

// Add service
builder.Services.AddScoped<IBranchService, BranchService>();
```

### 2. Create Database Migration
```powershell
cd c:\Projects\DentalClinic
dotnet ef migrations add AddBranchManagement --project DentalClinic.Infrastructure --startup-project DentalClinic.Web
```

### 3. Update Database
```powershell
dotnet ef database update --project DentalClinic.Infrastructure --startup-project DentalClinic.Web
```

### 4. Test the API
After running the migration, you can test these endpoints:

**Get all branches:**
```
GET /api/Branch
```

**Create a branch:**
```
POST /api/Branch
Content-Type: application/json

{
  "branchName": "Downtown Clinic",
  "branchCode": "BR01",
  "address": "123 Main Street",
  "city": "Ho Chi Minh City",
  "district": "District 1",
  "phone": "028-1234-5678",
  "email": "downtown@dental.com",
  "isMainBranch": false
}
```

**Assign user to branch:**
```
POST /api/Branch/assign
Content-Type: application/json

{
  "userId": 1,
  "branchId": 1,
  "roleId": 2
}
```

---

## 📊 Database Schema Summary

### New Tables

**Branch**
- Primary Key: BranchId
- Unique Key: BranchCode
- Stores branch information (name, address, contact, status)

**UserBranchMapping**
- Composite Primary Key: (UserId, BranchId, RoleId)
- Foreign Keys: UserId → User, BranchId → Branch, RoleId → UserRole
- Tracks which users work at which branches with what roles

### Modified Tables

**Appointment**
- Added: BranchId (nullable FK to Branch)

**Inventory**
- Added: BranchId (nullable FK to Branch)

---

## 🎯 Key Features

1. **Multi-Branch Support**: Manage multiple clinic locations
2. **Flexible Assignment**: Users can work at multiple branches
3. **Role-Specific**: Different roles at different branches
4. **Audit Trail**: Track when users are assigned/removed
5. **Soft Delete**: Branches can be deactivated instead of deleted
6. **Main Branch**: Flag one branch as headquarters

---

## 💡 Usage Examples

### Example 1: Dr. Nguyen works at 3 branches
```
Dr. Nguyen Van A (UserId: 1)
├─ HQ Main (BranchId: 1) → Medical Director (RoleId: 1)
├─ District 1 (BranchId: 2) → Senior Dentist (RoleId: 2)
└─ Thu Duc (BranchId: 3) → Consultant (RoleId: 3)
```

### Example 2: Downtown Branch has 4 staff
```
Downtown Branch (BranchId: 2)
├─ Dr. Nguyen → Senior Dentist
├─ Dr. Tran → Dentist
├─ Ms. Le → Receptionist
└─ Mr. Pham → Nurse
```

---

## 🔧 API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | /api/Branch | Get all branches | Yes |
| GET | /api/Branch/active | Get active branches | Yes |
| GET | /api/Branch/{id} | Get branch by ID | Yes |
| GET | /api/Branch/code/{code} | Get branch by code | Yes |
| GET | /api/Branch/main | Get main branch | Yes |
| POST | /api/Branch | Create branch | Admin |
| PUT | /api/Branch/{id} | Update branch | Admin |
| DELETE | /api/Branch/{id} | Delete branch (soft) | Admin |
| GET | /api/Branch/{id}/users | Get users at branch | Yes |
| GET | /api/Branch/user/{userId} | Get branches for user | Yes |
| GET | /api/Branch/user/{userId}/mappings | Get user's assignments | Yes |
| POST | /api/Branch/assign | Assign user to branch | Admin/Manager |
| DELETE | /api/Branch/remove | Remove user from branch | Admin/Manager |

---

## ✅ Checklist

- [x] Create Domain entities (Branch, UserBranchMapping)
- [x] Update existing entities (User, Appointment, Inventory)
- [x] Create repository interface and implementation
- [x] Update DbContext with new entities
- [x] Create DTOs for all operations
- [x] Create service interface and implementation
- [x] Create API controller
- [ ] Register services in DI container (Program.cs)
- [ ] Create and run database migration
- [ ] Test API endpoints
- [ ] Create UI components (optional)
- [ ] Add validation and business rules
- [ ] Add unit tests

---

## 📝 Notes

- All branch assignments use soft delete (IsActive flag)
- BranchCode must be unique if provided
- Only one branch can be marked as IsMainBranch
- Users can have multiple roles at the same branch
- Appointments and Inventory are now branch-specific
- Consider adding authorization rules based on user's branch assignments

---

## 🆘 Troubleshooting

**Migration fails:**
- Ensure all files are saved
- Build the solution first: `dotnet build`
- Check connection string in appsettings.json

**Service not found:**
- Verify DI registration in Program.cs
- Check using statements are included

**Foreign key constraint errors:**
- Ensure referenced entities exist
- Check IsActive flags on related records

---

## 📚 Further Enhancements

1. Add branch-specific reporting
2. Implement branch-based authorization
3. Add branch transfer history
4. Create branch performance metrics
5. Add branch opening hours
6. Implement branch capacity management
7. Add branch-specific pricing
8. Create UI for branch selection in appointments
