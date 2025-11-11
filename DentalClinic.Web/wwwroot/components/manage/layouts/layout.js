import { GlobalNavItems } from '../../../js/manage/constants/nav-items.js';
import { MessageDialogComponent } from '../../../js/manage/components/messageDialog.js';
import { userService } from '../../../js/manage/services/user-service.js';

const response = await fetch('/components/manage/layouts/layout.html');
const templateHtml = await response.text();

export const LayoutPage = {
    template: templateHtml,
    components: {
        'message-dialog': MessageDialogComponent
    },
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
        currentUser() {
            return userService.getCurrentUser();
        },
        userInitials() {
            return userService.generateAvatar(this.currentUser?.fullName || '');
        },
        userAvatarColor() {
            return userService.getAvatarColor(this.currentUser?.fullName || '');
        },
        roleInfo() {
            const primaryRole = this.currentUser?.roles?.[0] || '';
            return userService.getRoleInfo(primaryRole);
        },
        filteredNavItems() {
            const userRoles = this.currentUser?.roles || [];
            if (userRoles.length === 0) {
                return [];
            }

            // Filter parent menu items based on user roles
            return this.navItems.filter(item => {
                // If no roles required, show to all
                if (!item.roles || item.roles.length === 0) {
                    return true;
                }
                // Check if user has any of the required roles
                return item.roles.some(role => userRoles.includes(role));
            })
            .map(item => {
                // After filtering parents, filter children as well
                if (item.children) {
                    const filteredChildren = item.children.filter(child => {
                        if (!child.roles || child.roles.length === 0) {
                            return true;
                        }
                        return child.roles.some(role => userRoles.includes(role));
                    });
                    
                    // Return item with filtered children
                    return { ...item, children: filteredChildren };
                }
                return item;
            });
        },
        breadcrumbs() {
            const breadcrumbItems = [];
            const currentRoute = this.$route;

            // Build breadcrumb by analyzing the route hierarchy
            this.buildBreadcrumbFromRoute(currentRoute, breadcrumbItems);

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
                // Find which navigation group should be open based on current route
                const routeName = this.$route.name;

                // Helper function to find the route and its parent group recursively
                const findRouteInNavItems = (currentRouteName) => {
                    // First, try to find the route directly in navigation
                    for (const item of this.navItems) {
                        if (item.route && item.route.name === currentRouteName) {
                            return { foundRoute: item, parentGroup: null };
                        }
                        if (item.children) {
                            for (const child of item.children) {
                                if (child.route && child.route.name === currentRouteName) {
                                    return { foundRoute: child, parentGroup: item };
                                }
                            }
                        }
                    }
                    return null;
                };

                // Try to find the current route
                let result = findRouteInNavItems(routeName);

                // If not found, try to find through parent breadcrumb chain
                if (!result && this.$route.meta && this.$route.meta.parentBreadcrumb) {
                    let parentRouteName = this.$route.meta.parentBreadcrumb.name;

                    // Keep going up the chain until we find a route in navigation
                    while (parentRouteName && !result) {
                        result = findRouteInNavItems(parentRouteName);

                        if (!result) {
                            // Find the parent route config to get its parent breadcrumb
                            const findRoute = (routes, targetName) => {
                                for (const r of routes) {
                                    if (r.name === targetName) return r;
                                    if (r.children) {
                                        const found = findRoute(r.children, targetName);
                                        if (found) return found;
                                    }
                                }
                                return null;
                            };

                            const parentRouteConfig = findRoute(this.$router.options.routes, parentRouteName);
                            parentRouteName = parentRouteConfig?.meta?.parentBreadcrumb?.name;
                        }
                    }
                }

                // Set the navigation state
                if (result && result.parentGroup) {
                    this.openParentValue = [result.parentGroup.value];
                } else {
                    this.openParentValue = [];
                }
            },
            immediate: true // Chạy ngay khi component tải
        }
    },
    methods: {
        onToggle(item) {
            if (this.rail) {
                this.rail = false;
            }
        },

        // Helper method to find navigation item by value
        findNavItemByValue(value) {
            return this.navItems.find(item => item.value === value);
        },
        // Check if a navigation item should be active based on current route and its hierarchy
        isNavItemActive(navItem) {
            const currentRoute = this.$route;

            // Direct match
            if (navItem.route && navItem.route.name === currentRoute.name) {
                return true;
            }

            // Check if current route is a child of this nav item through breadcrumb chain
            if (this.isChildOfNavRoute(currentRoute.name, navItem.route.name)) {
                return true;
            }

            return false;
        },

        // Check if current route is a descendant of the target route
        isChildOfNavRoute(currentRouteName, targetRouteName) {
            // Build the parent chain for current route
            const buildParentChain = (routeName) => {
                const chain = [routeName];

                const findRoute = (routes, targetName) => {
                    for (const r of routes) {
                        if (r.name === targetName) return r;
                        if (r.children) {
                            const found = findRoute(r.children, targetName);
                            if (found) return found;
                        }
                    }
                    return null;
                };

                let currentRoute = findRoute(this.$router.options.routes, routeName);
                while (currentRoute?.meta?.parentBreadcrumb) {
                    const parentName = currentRoute.meta.parentBreadcrumb.name;
                    chain.push(parentName);
                    currentRoute = findRoute(this.$router.options.routes, parentName);
                }

                return chain;
            };

            const parentChain = buildParentChain(currentRouteName);
            return parentChain.includes(targetRouteName);
        },

        buildBreadcrumbFromRoute(route, breadcrumbItems) {
            const routeName = route.name;
            const routeMeta = route.meta;

            // Build breadcrumb chain by following parent breadcrumb references
            const buildChain = (currentRouteName, chain = []) => {
                // Find the route definition
                const findRoute = (routes) => {
                    for (const r of routes) {
                        if (r.name === currentRouteName) return r;
                        if (r.children) {
                            const found = findRoute(r.children);
                            if (found) return found;
                        }
                    }
                    return null;
                };

                const routeConfig = findRoute(this.$router.options.routes);
                if (!routeConfig || !routeConfig.meta) return chain;

                const meta = routeConfig.meta;

                // Add current route to chain
                chain.unshift({
                    name: currentRouteName,
                    title: meta.breadcrumbTitle || currentRouteName,
                    meta: meta
                });

                // If there's a parent, recursively build the chain
                if (meta.parentBreadcrumb) {
                    return buildChain(meta.parentBreadcrumb.name, chain);
                }

                return chain;
            };

            // Build the complete chain
            const chain = buildChain(routeName);

            // Convert chain to breadcrumb items
            chain.forEach((item, index) => {
                const isLast = index === chain.length - 1;
                breadcrumbItems.push({
                    title: item.title,
                    disabled: isLast, // Only the last item is disabled
                    to: isLast ? undefined : { name: item.name },
                    value: item.name
                });
            });
        },

        handleLogout() {
            // Clear user data using the service
            userService.logout();
            // Also clear any legacy tokens
            localStorage.removeItem('manage-token');
            this.$router.push({ name: 'Login' });
        },

        // User menu actions
        showProfile() {
            this.showInfoDialog(
                `<strong>Họ tên:</strong> ${this.currentUser?.fullName}<br/>
                 <strong>Vai trò:</strong> ${this.roleInfo.displayName}<br/>
                 <strong>Đăng nhập lúc:</strong> ${new Date(this.currentUser?.loginTime).toLocaleString('vi-VN')}`,
                'Thông tin cá nhân'
            );
        },

        showSettings() {
            this.showInfoDialog(
                'Tính năng cài đặt đang được phát triển.<br/>Vui lòng quay lại sau.',
                'Cài đặt'
            );
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
            }
            const index = this.openParentValue.indexOf(parentValue);

            if (index > -1) {
                this.openParentValue.splice(index, 1);
            } else {
                this.openParentValue = [parentValue];
            }
        },
        goToCreateCustomer(){
            
        }
    }
};