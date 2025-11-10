using DentalClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure
{
    public class DentalClinicDbContext : DbContext
    {
        public DentalClinicDbContext(DbContextOptions<DentalClinicDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<StaffDetail> StaffDetails => Set<StaffDetail>();
        public DbSet<PatientDetail> PatientDetails => Set<PatientDetail>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserRoleMapping> UserRoleMappings => Set<UserRoleMapping>();
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<UserBranchMapping> UserBranchMappings => Set<UserBranchMapping>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Treatment> Treatments => Set<Treatment>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Inventory> Inventory => Set<Inventory>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many: User <-> Role
            modelBuilder.Entity<UserRoleMapping>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // Composite key for UserBranchMapping (User can have different roles at different branches)
            modelBuilder.Entity<UserBranchMapping>()
                .HasKey(ub => new { ub.UserId, ub.BranchId, ub.RoleId });

            modelBuilder.Entity<UserBranchMapping>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.UserBranches)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserBranchMapping>()
                .HasOne(ub => ub.Branch)
                .WithMany(b => b.UserBranches)
                .HasForeignKey(ub => ub.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserBranchMapping>()
                .HasOne(ub => ub.Role)
                .WithMany(r => r.UserBranchMappings)
                .HasForeignKey(ub => ub.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Branch unique constraint on BranchCode
            modelBuilder.Entity<Branch>()
                .HasIndex(b => b.BranchCode)
                .IsUnique()
                .HasFilter("[BranchCode] IS NOT NULL");

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Appointment relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(u => u.PatientAppointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Staff)
                .WithMany(u => u.StaffAppointments)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Branch)
                .WithMany(b => b.Appointments)
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            // Inventory - Branch relationship
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Branch)
                .WithMany(b => b.Inventory)
                .HasForeignKey(i => i.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            // Invoice
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Patient)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
