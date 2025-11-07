import { staffApi } from '../services/staff-api.js';
import { roleApi } from '../services/role-api.js';

const staffListResponse = await fetch('/components/manage/staffList.html');
const staffListHtml = await staffListResponse.text();

const staffFormResponse = await fetch('/components/manage/staffForm.html');
const staffFormHtml = await staffFormResponse.text();

export const StaffPage = {
    template: `<router-view></router-view>`
};

// 2. Component Danh sách Nhân sự (List)
export const StaffListComponent = {
    template: staffListHtml,
    data() {
        return {
            loading: true,
            headers: [
                { title: 'Họ tên', key: 'fullName' },
                { title: 'Chức danh', key: 'role' },
                { title: 'Email', key: 'email' },
                { title: 'Điện thoại', key: 'phone' },
                { title: 'Hành động', key: 'actions', sortable: false, align: 'end' },
            ],
            staffs: [],
            // Mobile-specific data
            searchQuery: '',
            currentPage: 1,
            itemsPerPageMobile: 6,
            // Message dialog configuration
            showDialog: false,
            dialogConfig: {
                type: 'info',
                title: '',
                message: '',
                showCancel: false,
                loading: false,
                okText: 'Đồng ý',
                cancelText: 'Hủy',
                onOk: () => { },
                onCancel: () => { }
            },
            // Item being processed
            selectedItem: null
        };
    },
    computed: {
        isMobile() {
            return this.$vuetify.display.smAndDown;
        },
        filteredStaffs() {
            if (!this.searchQuery) return this.staffs;
            const query = this.searchQuery.toLowerCase();
            return this.staffs.filter(staff =>
                staff.fullName.toLowerCase().includes(query) ||
                staff.roleName.toLowerCase().includes(query) ||
                (staff.email && staff.email.toLowerCase().includes(query)) ||
                (staff.phone && staff.phone.toLowerCase().includes(query))
            );
        },
        totalPagesMobile() {
            return Math.ceil(this.filteredStaffs.length / this.itemsPerPageMobile);
        },
        paginatedStaffs() {
            const start = (this.currentPage - 1) * this.itemsPerPageMobile;
            const end = start + this.itemsPerPageMobile;
            return this.filteredStaffs.slice(start, end);
        }
    },
    watch: {
        searchQuery() {
            // Reset to first page when searching
            this.currentPage = 1;
        }
    },
    methods: {
        async fetchStaffs() {
            this.loading = true;
            try {
                this.staffs = await staffApi.getAll();
            } catch (err) {
                this.showErrorDialog('Không thể tải danh sách nhân sự', 'Lỗi tải dữ liệu');
            } finally {
                this.loading = false;
            }
        },
        goToCreate() {
            this.$router.push({ name: 'StaffCreate' });
        },
        goToEdit(id) {
            this.$router.push({ name: 'StaffEdit', params: { id: id } });
        }, 
        confirmDelete(item) {
            this.selectedItem = item;
            this.showDeleteConfirmDialog(item.fullName, this.executeDelete);
        }, 
        async executeDelete() {
            this.setDialogLoading(true);
            try {
                await staffApi.delete(this.selectedItem.id);
                this.closeDialog();
                await this.fetchStaffs();
                this.showSuccessDialog(
                    `Nhân sự "${this.selectedItem.fullName}" đã được xóa thành công.`,
                    'Xóa thành công'
                );
            } catch (err) {
                this.setDialogLoading(false);
                this.showErrorDialog(
                    'Không thể xóa nhân sự. Vui lòng thử lại.',
                    'Lỗi xóa dữ liệu'
                );
            }
        }, 
        getRoleColor(roleName) {
            if (roleName.includes('Bác sĩ')) return 'blue-darken-1';
            if (roleName.includes('Admin')) return 'red-darken-1';
            if (roleName.includes('Lễ tân')) return 'green-darken-1';
            return 'grey';
        }
    },
    mounted() {
        this.fetchStaffs();
    }
};

// 3. Component Form (Tạo mới/Chỉnh sửa)
export const StaffFormPage = {
    template: staffFormHtml,
    data() {
        return {
            loading: false,
            staffId: this.$route.params.id || null,
            staff: {
                fullName: '', email: '', phone: '', roleId: null, username: '', password: ''
            },
            roles: [],
            error: null,
            rules: {
                required: v => !!v || 'Thông tin bắt buộc.',
                requiredRole: v => (v !== null && v > 0) || 'Vai trò là bắt buộc.',
                email: v => /.+@.+\..+/.test(v) || 'Email không hợp lệ.',
                minLength: v => (v && v.length >= 6) || 'Mật khẩu phải ít nhất 6 ký tự.',
            },
            // Message dialog configuration
            showDialog: false,
            dialogConfig: {
                type: 'info',
                title: '',
                message: '',
                showCancel: false,
                loading: false,
                okText: 'Đồng ý',
                cancelText: 'Hủy',
                onOk: () => { },
                onCancel: () => { }
            }
        };
    },
    computed: {
        isEditMode() {
            return !!this.staffId;
        },
        formTitle() {
            return this.isEditMode ? 'Chỉnh sửa Nhân sự' : 'Tạo mới Nhân sự';
        }
    }, methods: {
        async fetchRoles() {
            try {
                this.roles = await roleApi.getAll();
            } catch (err) {
                this.showErrorDialog('Không thể tải danh sách vai trò.', 'Lỗi tải dữ liệu');
            }
        },
        async loadStaffData() {
            this.loading = true;
            try {
                const data = await staffApi.getById(this.staffId);
                this.staff = {
                    fullName: data.fullName,
                    email: data.email,
                    phone: data.phone,
                    roleId: data.roleId,
                    username: data.username
                };
            } catch (err) {
                this.showErrorDialog('Không thể tải dữ liệu nhân sự.', 'Lỗi tải dữ liệu');
            } finally {
                this.loading = false;
            }
        },
        async saveStaff() {
            // Validate form
            const { valid } = await this.$refs.staffForm.validate();
            if (!valid) {
                this.showWarningDialog('Vui lòng điền đầy đủ thông tin hợp lệ.', 'Thông tin chưa hợp lệ');
                return;
            }

            this.loading = true;
            this.error = null;

            try {
                if (this.isEditMode) {
                    const updateDto = {
                        fullName: this.staff.fullName,
                        email: this.staff.email,
                        phone: this.staff.phone,
                        roleId: this.staff.roleId,
                    };
                    await staffApi.update(this.staffId, updateDto);
                    this.showSuccessDialog(
                        'Thông tin nhân sự đã được cập nhật thành công.',
                        'Cập nhật thành công',
                        () => this.goBack()
                    );
                } else {
                    await staffApi.create(this.staff);
                    this.showSuccessDialog(
                        'Nhân sự mới đã được tạo thành công.',
                        'Tạo mới thành công',
                        () => this.goBack()
                    );
                }
            } catch (err) {
                const errorMessage = err.response?.data?.message || 'Đã xảy ra lỗi khi lưu thông tin.';
                this.showErrorDialog(errorMessage, 'Lỗi lưu dữ liệu');
            } finally {
                this.loading = false;
            }
        }, 
        goBack() {
            this.$router.push({ name: 'StaffList' });
        }
    },
    created() {
        this.fetchRoles();
        if (this.isEditMode) {
            this.loadStaffData();
        }
    }
};