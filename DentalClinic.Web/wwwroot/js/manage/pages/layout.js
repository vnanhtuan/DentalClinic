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

            // Biến state để kiểm soát v-list-group đang mở
            openParentValue: [],
        };
    },
    computed: {
        isMobile() {
            // Dùng 'smAndDown' để bắt mobile và tablet (<= 960px)
            return this.$vuetify.display.smAndDown;
        },
        breadcrumbs() {
            const breadcrumbItems = [];
            const routeName = this.$route.name;

            // 1. Tìm nhóm cha (v-list-group) nếu route hiện tại là con
            const parent = GlobalNavItems.find(item =>
                item.children && item.children.some(child => child.route.name === routeName)
            );

            if (parent) {
                // Thêm tiêu đề của nhóm cha (ví dụ: Quản lý Lịch Hẹn). Không có link.
                breadcrumbItems.push({
                    title: parent.title,
                    disabled: false, // Không thể click vì nó không phải là route
                    to: undefined,
                    value: parent.value
                });
            }

            // 2. Thêm tiêu đề của trang hiện tại
            const currentTitleMeta = this.$route.meta.breadcrumbTitle;
            if (currentTitleMeta) {
                breadcrumbItems.push({
                    title: currentTitleMeta,
                    disabled: true, // Luôn là mục cuối cùng và disabled
                    to: undefined,
                    value: this.$route.name
                });
            }

            return breadcrumbItems;
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
        },

        $route: {
            handler() {
                const routeName = this.$route.name;
                const parent = GlobalNavItems.find(item =>
                    item.children && item.children.some(child => child.route.name === routeName)
                );

                // SỬA LỖI: Gán giá trị cho mảng
                if (parent) {
                    this.openParentValue = [parent.value]; // Phải là mảng
                } else {
                    this.openParentValue = []; // Mảng rỗng
                }
            },
            immediate: true // Chạy ngay khi component tải
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
        },

        toggleSidebarParent(parentValue) {
            
            if (this.rail) {
                this.rail = false;
                return;
            }
            const index = this.openParentValue.indexOf(parentValue);

            if (index > -1) {
                this.openParentValue.splice(index, 1);
            } else {
                this.openParentValue = [parentValue];
            }
        }
    }
};