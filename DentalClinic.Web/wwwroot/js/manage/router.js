// 1. IMPORT CÁC COMPONENT TRANG (đã có đủ logic)
import { LoginPage } from './pages/login.js';
import { LayoutPage } from './pages/layout.js';
import { DashboardPage } from './pages/dashboard.js';
import { StaffPage, StaffListComponent, StaffFormPage } from './pages/staff.js';
import { SettingsPage, RoleListComponent, RoleFormPage } from './pages/role.js';

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
                name: 'Staff', // Tên route cha
                component: StaffPage, // Component cha (chỉ chứa <router-view>)
                meta: { requiresAuth: true }, // Meta cha
                // Các route con
                children: [
                    {
                        path: '', // /manage/staff (Mặc định)
                        name: 'StaffList',
                        component: StaffListComponent,
                        meta: { breadcrumbTitle: 'Danh sách Nhân sự', requiresAuth: true }
                    },
                    {
                        path: 'new', // /manage/staff/new
                        name: 'StaffCreate',
                        component: StaffFormPage,
                        meta: {
                            breadcrumbTitle: 'Tạo Nhân sự Mới',
                            requiresAuth: true,
                            parentBreadcrumb: { name: 'StaffList', title: 'Danh sách Nhân sự' }
                        }
                    },
                    {
                        path: ':id/edit', // /manage/staff/123/edit
                        name: 'StaffEdit',
                        component: StaffFormPage,
                        meta: {
                            breadcrumbTitle: 'Chỉnh sửa Nhân sự',
                            requiresAuth: true,
                            parentBreadcrumb: { name: 'StaffList', title: 'Danh sách Nhân sự' }
                        },
                        props: true // Tự động truyền :id làm prop
                    }
                ]
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
                component: SettingsPage, // Temporary, will be replaced later
                meta: { breadcrumbTitle: 'Cài Đặt Hệ Thống', requiresAuth: true },
                redirect: { name: 'RoleList' },
                children: [                    {
                        path: 'roles', // /manage/settings/roles
                        name: 'RoleList',
                        component: RoleListComponent,
                        meta: { 
                            breadcrumbTitle: 'Quản lý Vai trò', 
                            requiresAuth: true,
                            parentBreadcrumb: { name: 'Settings', title: 'Cài Đặt Hệ Thống' }
                        }
                    },
                    {
                        path: 'roles/new', // /manage/settings/roles/new
                        name: 'RoleCreate',
                        component: RoleFormPage,
                        meta: {
                            breadcrumbTitle: 'Tạo Vai trò Mới',
                            requiresAuth: true,
                            parentBreadcrumb: { name: 'RoleList', title: 'Quản lý Vai trò' }
                        }
                    },
                    {
                        path: 'roles/:id/edit', // /manage/settings/roles/123/edit
                        name: 'RoleEdit',
                        component: RoleFormPage,
                        meta: {
                            breadcrumbTitle: 'Chỉnh sửa Vai trò',
                            requiresAuth: true,
                            parentBreadcrumb: { name: 'RoleList', title: 'Quản lý Vai trò' }
                        },
                        props: true
                    }
                ]
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