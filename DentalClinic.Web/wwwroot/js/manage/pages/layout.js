import { GlobalNavItems } from '../constants/nav-items.js';

const response = await fetch('/components/manage/layout.html');
const templateHtml = await response.text();

export const LayoutPage = {
    template: templateHtml,
    data() {
        return {
            // Mặc định cho desktop (luôn mở)
            drawer: true,
            rail: false,
            navItems: GlobalNavItems,
        };
    },
    computed: {
        isMobile() {
            // Dùng 'smAndDown' để bắt mobile và tablet (<= 960px)
            return this.$vuetify.display.smAndDown;
        },
        breadcrumbs() {
            // Lấy tất cả các route đã khớp (matched)
            const matchedRoutes = this.$route.matched;

            const items = matchedRoutes
                // Lọc chỉ lấy các route có tiêu đề (breadcrumbTitle)
                .filter(route => route.meta && route.meta.breadcrumbTitle)
                .map((route, index) => {
                    const isLast = index === matchedRoutes.length - 1;
                    return {
                        title: route.meta.breadcrumbTitle,
                        disabled: isLast, // Disabled = True cho item cuối cùng
                        href: isLast ? '' : route.path,
                        to: isLast ? undefined : route.path,
                    };
                });

            // Trả về mảng breadcrumb
            return items;
        },
        currentRouteTitle() {
            // Lấy tiêu đề từ meta của route hiện tại
            return this.$route.meta.breadcrumbTitle || 'Quản Trị';
        }
    },
    watch: {
        isMobile: {
            handler(isMobile) {
                if (isMobile) {
                    // Trên Mobile/Tablet: Mặc định ĐÓNG và TẠM THỜI
                    this.drawer = false;
                    this.rail = false;
                } else {
                    // Trên Desktop: Mặc định MỞ và CỐ ĐỊNH (không temporary)
                    this.drawer = true;
                    this.rail = false;
                }
            },
            immediate: true
        }
    },
    methods: {
        handleLogout() {
            localStorage.removeItem('manage-token');
            this.$router.push({ name: 'Login' });
        },

        // Hàm này xử lý mobile (ẩn/hiện menu)
        toggleMobileDrawer() {
            this.drawer = !this.drawer;
        },

        // Hàm này xử lý desktop (thu gọn/mở rộng)
        toggleRail() {
            this.rail = !this.rail;
        }
    }
};