namespace DentalClinic.Domain.Common
{
    public static class RoleConstants
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Doctor = "Doctor";
        public const string Nurse = "Nurse";
        public const string Receptionist = "Receptionist";
        public const string Staff = "Staff";

        public const string AdminManager = Admin + "," + Manager;

    }
}
