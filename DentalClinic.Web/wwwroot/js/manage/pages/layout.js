const response = await fetch('/components/manage/layout.html');
const templateHtml = await response.text();

export const LayoutPage = {
    template: templateHtml,
    data() {
        return {
            // Mặc định cho desktop (luôn mở)
            drawer: true,
            rail: false,
        };
    },
    computed: {
        isMobile() {
            // Dùng 'smAndDown' để bắt mobile và tablet (<= 960px)
            return this.$vuetify.display.smAndDown;
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