# Branch Management System - Implementation Summary

## Overview
The Branch management system allows the dental clinic to manage multiple locations (branches) and assign users to work at different branches with specific roles.

## Database Structure

### 1. **Branch Table**
Stores information about each clinic branch/location.

**Fields:**
- `BranchId` (PK): Unique identifier
- `BranchName`: Name of the branch
- `BranchCode`: Unique code for the branch (e.g., "HQ", "BR01", "BR02")
- `Address`: Full address
- `City`: City name
- `District`: District/area
- `Phone`: Contact phone number
- `Email`: Contact email
- `IsActive`: Whether the branch is currently active
- `IsMainBranch`: Flag to identify the headquarters/main branch
- `CreatedAt`: Creation timestamp
- `UpdatedAt`: Last update timestamp

### 2. **UserBranchMapping Table**
Links users to branches with their specific role at each branch. This is a many-to-many relationship with an additional role dimension.

**Fields:**
- `UserId` (PK, FK): Reference to User
- `BranchId` (PK, FK): Reference to Branch
- `RoleId` (PK, FK): Reference to UserRole
- `IsActive`: Whether this assignment is active
- `AssignedAt`: When the user was assigned to this branch
- `RemovedAt`: When the user was removed (if applicable)

**Composite Primary Key:** (UserId, BranchId, RoleId)
- This allows a user to work at the same branch with different roles
- A user can work at multiple branches
- A user can have different roles at different branches

## Key Features

### Flexibility
1. **Multi-Branch Users**: A user (e.g., a dentist) can work at multiple branches
2. **Multi-Role per Branch**: A user can have different roles at different branches
   - Example: Dr. Smith is a "Senior Dentist" at Branch A and a "Consultant" at Branch B
3. **Branch-Specific Operations**: Appointments and Inventory can be tracked per branch

### Data Integrity
- Unique constraint on `BranchCode` ensures no duplicate codes
- Cascade delete on user-branch mappings when user or branch is deleted
- Soft delete support for branches (IsActive flag)
- Timestamps for audit trail

## Relationships

### Branch Entity
- **One-to-Many** with `UserBranchMapping`: A branch can have many users
- **One-to-Many** with `Appointment`: Appointments are scheduled at specific branches
- **One-to-Many** with `Inventory`: Each branch manages its own inventory

### User Entity
- **Many-to-Many** with `Branch` through `UserBranchMapping`
- Extended with `UserBranches` navigation property

### UserRole Entity
- **One-to-Many** with `UserBranchMapping`: A role can be assigned to many user-branch combinations

## Repository Methods

### IBranchRepository
- `GetByBranchCodeAsync(string branchCode)`: Find branch by unique code
- `GetActiveBranchesAsync()`: Get all active branches
- `GetMainBranchAsync()`: Get the headquarters branch
- `GetUsersByBranchAsync(int branchId)`: Get all users working at a branch
- `GetBranchesByUserAsync(int userId)`: Get all branches where a user works
- `AssignUserToBranchAsync(userId, branchId, roleId)`: Assign a user to a branch with a role
- `RemoveUserFromBranchAsync(userId, branchId, roleId)`: Remove user from a branch
- `GetUserBranchMappingsAsync(int userId)`: Get all branch-role assignments for a user

## Service Methods

### IBranchService
**Branch Management:**
- `GetAllBranchesAsync()`: Get all branches
- `GetActiveBranchesAsync()`: Get active branches only
- `GetBranchByIdAsync(int branchId)`: Get specific branch
- `GetBranchByCodeAsync(string branchCode)`: Find by code
- `GetMainBranchAsync()`: Get main branch
- `CreateBranchAsync(CreateBranchDto)`: Create new branch
- `UpdateBranchAsync(int, UpdateBranchDto)`: Update branch
- `DeleteBranchAsync(int)`: Soft delete branch

**User-Branch Management:**
- `GetUsersByBranchAsync(int branchId)`: Get users at a branch
- `GetBranchesByUserAsync(int userId)`: Get branches for a user
- `GetUserBranchMappingsAsync(int userId)`: Get all user's branch assignments
- `AssignUserToBranchAsync(AssignUserToBranchDto)`: Assign user to branch
- `RemoveUserFromBranchAsync(userId, branchId, roleId)`: Remove assignment

## DTOs

1. **BranchDto**: Full branch information for reading
2. **CreateBranchDto**: Data needed to create a new branch
3. **UpdateBranchDto**: Data needed to update a branch
4. **UserBranchDto**: Combined user-branch-role information
5. **AssignUserToBranchDto**: Data to assign a user to a branch with a role

## Usage Examples

### Example 1: Create a Branch
```csharp
var branch = new CreateBranchDto
{
    BranchName = "Downtown Clinic",
    BranchCode = "BR01",
    Address = "123 Main St",
    City = "Ho Chi Minh City",
    District = "District 1",
    Phone = "028-1234-5678",
    Email = "downtown@dentalclinic.com",
    IsMainBranch = false
};
await branchService.CreateBranchAsync(branch);
```

### Example 2: Assign User to Branch
```csharp
// Assign Dr. Nguyen (UserId: 5) as Senior Dentist (RoleId: 3) at Downtown Branch (BranchId: 2)
var assignment = new AssignUserToBranchDto
{
    UserId = 5,
    BranchId = 2,
    RoleId = 3
};
await branchService.AssignUserToBranchAsync(assignment);
```

### Example 3: Get All Branches for a User
```csharp
var userBranches = await branchService.GetUserBranchMappingsAsync(userId: 5);
// Returns all branches where user 5 works, with their respective roles
```

## Migration Steps

To apply this to your database:

1. Build the solution to ensure no compilation errors
2. Add a new migration:
   ```powershell
   dotnet ef migrations add AddBranchManagement --project DentalClinic.Infrastructure --startup-project DentalClinic.Web
   ```
3. Update the database:
   ```powershell
   dotnet ef database update --project DentalClinic.Infrastructure --startup-project DentalClinic.Web
   ```

## Next Steps

1. Register the repository and service in your dependency injection container (Program.cs)
2. Create API controllers for branch management
3. Create UI components for:
   - Branch listing and management
   - User-branch assignment interface
   - Branch selection in appointment booking
   - Branch-specific inventory management
4. Add validation rules (e.g., prevent deletion of branches with active appointments)
5. Add authorization rules (e.g., only admins can create/delete branches)
