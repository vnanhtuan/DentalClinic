# Branch System - Database Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        BRANCH MANAGEMENT SYSTEM                          │
└─────────────────────────────────────────────────────────────────────────┘

┌──────────────────────┐
│      Branch          │
├──────────────────────┤
│ BranchId (PK)        │
│ BranchName           │
│ BranchCode (Unique)  │
│ Address              │
│ City                 │
│ District             │
│ Phone                │
│ Email                │
│ IsActive             │
│ IsMainBranch         │
│ CreatedAt            │
│ UpdatedAt            │
└──────────────────────┘
         │
         │ 1
         │
         │ *
         ▼
┌──────────────────────────────┐
│   UserBranchMapping          │
├──────────────────────────────┤
│ UserId (PK, FK)              │
│ BranchId (PK, FK)            │
│ RoleId (PK, FK)              │
│ IsActive                     │
│ AssignedAt                   │
│ RemovedAt                    │
└──────────────────────────────┘
         │
         │ *
         │
         │ 1
         ▼
┌──────────────────────┐              ┌──────────────────────┐
│      User            │              │     UserRole         │
├──────────────────────┤              ├──────────────────────┤
│ UserId (PK)          │              │ RoleId (PK)          │
│ FullName             │◄─────────────│ RoleName             │
│ Username             │              │ Description          │
│ Email                │              │ Color                │
│ ...                  │              └──────────────────────┘
└──────────────────────┘
         │
         │
         ▼
┌──────────────────────┐
│   Appointment        │
├──────────────────────┤
│ AppointmentId (PK)   │
│ PatientId (FK)       │
│ StaffId (FK)         │
│ ServiceId (FK)       │
│ BranchId (FK) ◄──────┼───────┐
│ AppointmentDate      │       │
│ Status               │       │
│ Notes                │       │
└──────────────────────┘       │
                               │
┌──────────────────────┐       │
│   Inventory          │       │
├──────────────────────┤       │
│ InventoryId (PK)     │       │
│ BranchId (FK) ◄──────┼───────┘
│ ItemName             │
│ Category             │
│ Quantity             │
│ UnitPrice            │
│ ExpiryDate           │
└──────────────────────┘


═══════════════════════════════════════════════════════════════════════
                            USAGE SCENARIOS
═══════════════════════════════════════════════════════════════════════

Scenario 1: Multi-Branch User with Different Roles
───────────────────────────────────────────────────
User: Dr. Nguyen Van A (UserId: 1)
  ├─ Branch: HQ Main Clinic (BranchId: 1)
  │   └─ Role: Medical Director (RoleId: 1)
  │
  ├─ Branch: District 1 Branch (BranchId: 2)
  │   └─ Role: Senior Dentist (RoleId: 2)
  │
  └─ Branch: Thu Duc Branch (BranchId: 3)
      └─ Role: Consultant (RoleId: 3)


Scenario 2: Branch with Multiple Staff
───────────────────────────────────────
Branch: Downtown Clinic (BranchId: 2)
  ├─ Dr. Nguyen Van A
  │   └─ Role: Senior Dentist
  │
  ├─ Dr. Tran Thi B
  │   └─ Role: Dentist
  │
  ├─ Ms. Le Thi C
  │   └─ Role: Receptionist
  │
  └─ Mr. Pham Van D
      └─ Role: Nurse


Scenario 3: Same User, Same Branch, Multiple Roles
───────────────────────────────────────────────────
User: Dr. Hoang Van E (UserId: 5)
Branch: Main Clinic (BranchId: 1)
  ├─ Role: Dentist (RoleId: 2)
  └─ Role: Department Head (RoleId: 6)


═══════════════════════════════════════════════════════════════════════
                        QUERY EXAMPLES
═══════════════════════════════════════════════════════════════════════

Q1: Get all branches where a user works
────────────────────────────────────────
SELECT b.* 
FROM Branch b
JOIN UserBranchMapping ubm ON b.BranchId = ubm.BranchId
WHERE ubm.UserId = @userId AND ubm.IsActive = 1


Q2: Get all users working at a specific branch
───────────────────────────────────────────────
SELECT u.*, ur.RoleName
FROM User u
JOIN UserBranchMapping ubm ON u.UserId = ubm.UserId
JOIN UserRole ur ON ubm.RoleId = ur.RoleId
WHERE ubm.BranchId = @branchId AND ubm.IsActive = 1


Q3: Get user's role at a specific branch
─────────────────────────────────────────
SELECT ur.*
FROM UserRole ur
JOIN UserBranchMapping ubm ON ur.RoleId = ubm.RoleId
WHERE ubm.UserId = @userId 
  AND ubm.BranchId = @branchId 
  AND ubm.IsActive = 1


Q4: Get all appointments for a branch on a specific date
─────────────────────────────────────────────────────────
SELECT a.*, u.FullName as PatientName, s.FullName as StaffName
FROM Appointment a
JOIN User u ON a.PatientId = u.UserId
JOIN User s ON a.StaffId = s.UserId
WHERE a.BranchId = @branchId 
  AND CAST(a.AppointmentDate AS DATE) = @date


Q5: Get inventory for all branches
───────────────────────────────────
SELECT b.BranchName, i.ItemName, i.Quantity, i.UnitPrice
FROM Inventory i
JOIN Branch b ON i.BranchId = b.BranchId
WHERE b.IsActive = 1
ORDER BY b.BranchName, i.ItemName
```
