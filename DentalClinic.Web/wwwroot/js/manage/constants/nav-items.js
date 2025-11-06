export const GlobalNavItems = [
    {
        title: 'Tổng quan',
        icon: 'mdi-view-dashboard',
        route: { name: 'Dashboard' },
        value: 'dashboard',
        roles: ['Admin', 'Staff'] // Chỉ định vai trò được thấy
    },
    {
        title: 'Quản Lý Lịch Hẹn',
        icon: 'mdi-calendar-month',
        value: 'appointments',
        roles: ['Admin', 'Staff', 'Receptionist'],
        children: [
            {
                title: 'Danh sách lịch hẹn',
                icon: 'mdi-calendar-check',
                route: { name: 'AppointmentList' }, // Route Name MỚI
                value: 'appointment-list'
            },
            {
                title: 'Danh sách lịch hủy',
                icon: 'mdi-calendar-remove',
                route: { name: 'CanceledAppointments' }, // Route Name MỚI
                value: 'canceled-appointments'
            },
        ]
    },
    {
        title: 'Hồ Sơ Bệnh Nhân',
        icon: 'mdi-folder-account',
        route: { name: 'Patients' },
        value: 'patients',
        roles: ['Admin', 'Staff']
    },
    {
        title: 'Quy Trình Điều Trị',
        icon: 'mdi-tooth-outline',
        route: { name: 'Treatments' },
        value: 'treatments',
        roles: ['Admin', 'Staff']
    },
    {
        title: 'Hóa Đơn & Tài Chính',
        icon: 'mdi-currency-usd',
        route: '/manage/finance', // Giả định route này chưa định nghĩa name
        value: 'finance',
        roles: ['Admin', 'Accountant']
    },
    {
        title: 'Quản Lý Kho Vật Tư',
        icon: 'mdi-package-variant',
        route: { name: 'Inventory' },
        value: 'inventory',
        roles: ['Admin', 'InventoryManager']
    },
    {
        title: 'Nhân Sự',
        icon: 'mdi-account-group',
        route: { name: 'Staff' },
        value: 'staff',
        roles: ['Admin', 'HR']
    },
    {
        title: 'Cài Đặt Hệ Thống',
        icon: 'mdi-cog',
        route: { name: 'Settings' },
        value: 'settings',
        roles: ['Admin']
    },
];
