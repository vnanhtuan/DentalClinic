// 1. IMPORT CÁC COMPONENT TRANG (đã có đủ logic)
import { LoginPage } from './pages/login.js';
import { LayoutPage } from './pages/layout.js';
import { DashboardPage } from './pages/dashboard.js';

// 2. ĐỊNH NGHĨA ROUTES (Sạch sẽ)
const routes = [
    {
        path: '/manage/login',
        name: 'Login',
        component: LoginPage
    },
    {
        path: '/manage',
        component: LayoutPage, // Layout cha (có menu)
        children: [
            {
                path: '', // /manage
                name: 'Dashboard',
                component: DashboardPage,
                meta: { breadcrumbTitle: 'Tổng quan', requiresAuth: true }
            },
            {
                path: 'patients',
                name: 'Patients',
                component: DashboardPage, // Temporary, will be replaced later
                meta: { breadcrumbTitle: 'Hồ sơ bệnh nhân', requiresAuth: true }
            },
            {
                path: 'patients/:id',
                name: 'PatientDetail',
                component: DashboardPage, // Temporary, will be replaced later
                meta: { breadcrumbTitle: 'Chi tiết hồ sơ', requiresAuth: true }
            },
            {
                path: 'appointments/list',
                name: 'AppointmentList',
                component: DashboardPage, // Temporary, will be replaced later
                meta: { breadcrumbTitle: 'Danh sách lịch hẹn', requiresAuth: true }
            },

            // ROUTE MỚI: LỊCH HẸN ĐÃ HỦY
            {
                path: 'appointments/canceled',
                name: 'CanceledAppointments',
                component: DashboardPage, // Temporary, will be replaced later
                meta: { breadcrumbTitle: 'Lịch hủy', requiresAuth: true }
            },
            {
                path: 'treatments',
                name: 'Treatments',
                component: DashboardPage, // Temporary, will be replaced later
                meta: { breadcrumbTitle: 'Quy trình điều trị', requiresAuth: true }
            },
            {
                path: 'staff',
                name: 'Staff',
                component: DashboardPage, // Temporary, will be replaced later
                meta: { breadcrumbTitle: 'Nhân sự', requiresAuth: true }
            },
            {
                path: 'inventory',
                name: 'Inventory',
                component: DashboardPage, // Temporary, will be replaced later
                meta: { breadcrumbTitle: 'Quản lý kho', requiresAuth: true }
            },
            {
                path: 'settings',
                name: 'Settings',
                component: DashboardPage, // Temporary, will be replaced later
                meta: { breadcrumbTitle: 'Cài Đặt Hệ Thống', requiresAuth: true }
            }
        ]
    },
];

// 3. TẠO ROUTER
export const router = window.VueRouter.createRouter({
    history: window.VueRouter.createWebHistory(),
    routes,
    scrollBehavior: () => ({ top: 0 })
});

// 4. ĐỊNH NGHĨA BẢO VỆ (Navigation Guard)
// Đây là logic *thuộc về* router, nên để ở đây là hợp lý
router.beforeEach((to, from, next) => {
    const isAuthenticated = localStorage.getItem('manage-token');
    if (to.name === 'Login') {
        if (isAuthenticated) {
            next({ name: 'Dashboard' });
        } else {
            next();
        }
    } else {
        if (isAuthenticated) {
            next();
        } else {
            next({ name: 'Login' });
        }
    }
});