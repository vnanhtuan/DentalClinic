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
                component: DashboardPage
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