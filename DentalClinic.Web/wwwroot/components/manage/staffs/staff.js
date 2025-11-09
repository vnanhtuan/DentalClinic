import { staffApi } from './staff-api.js';
import { MessageDialogMixin } from '../../../js/manage/mixins/messageDialogMixin.js';
import { PAGE_SIZE_OPTIONS, DEFAULT_PAGE_SIZE } from '../../../js/manage/constants/paginationConstants.js';

const staffListResponse = await fetch('/components/manage/staffs/staffList.html');
const staffListHtml = await staffListResponse.text();

const staffFormResponse = await fetch('/components/manage/staffs/staffForm.html');
const staffFormHtml = await staffFormResponse.text();

export const StaffPage = {
    template: `<router-view></router-view>`
};

// 2. Component Danh sách Nhân sự (List)
export const StaffListComponent = {
    template: staffListHtml,
    mixins: [MessageDialogMixin],
    data() {
        return {
            loading: true,
            headers: [
                { title: 'STT', key: 'stt', sortable: false, width: '80px' },
                { title: 'Họ tên', key: 'fullName' },
                { title: 'Chức danh', key: 'role' },
                { title: 'Email', key: 'email' },
                { title: 'Điện thoại', key: 'phone' }, { title: 'Hành động', key: 'actions', sortable: false, align: 'end' },
            ], staffs: [],
            totalItems: 0,
            totalPages: 0,
            currentPage: 1,
            searchQuery: '',
            pageSize: DEFAULT_PAGE_SIZE,
            pageSizeOptions: PAGE_SIZE_OPTIONS,
            searchTimeout: null,

            // Item being processed
            selectedItem: null
        };
    }, 
    computed: {
        isMobile() {
            return this.$vuetify.display.smAndDown;
        }
    }, 
    watch: {
        currentPage() {
            this.fetchStaffs();
        },
        pageSize() {
            this.currentPage = 1;
            this.fetchStaffs();
        },
        searchQuery() {
            // Debounce search - wait 500ms after user stops typing
            clearTimeout(this.searchTimeout);
            this.searchTimeout = setTimeout(() => {
                this.currentPage = 1;
                this.fetchStaffs();
            }, 500);
        }
    },
    methods: {
        async fetchStaffs() {
            this.loading = true;
            try {
                const params = {
                    pageNumber: this.currentPage,
                    pageSize: this.pageSize,
                    searchTerm: this.searchQuery || ''
                };
                const response = await staffApi.getPaginated(params);
                this.staffs = response.items || [];
                this.totalItems = response.totalItems || 0;
                this.totalPages = response.totalPages || 0;
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
        }
    },
    mounted() {
        this.fetchStaffs();
    }
};

// 3. Component Form (Tạo mới/Chỉnh sửa)
export const StaffFormPage = {
    template: staffFormHtml,
    mixins: [MessageDialogMixin],
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
                requiredRoles: v => (v && v.length > 0) || 'Phải chọn ít nhất một vai trò.',
                email: v => /.+@.+\..+/.test(v) || 'Email không hợp lệ.',
                minLength: v => (v && v.length >= 6) || 'Mật khẩu phải ít nhất 6 ký tự.',
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
    },
    methods: {
        async initForm() {
            try {
                this.roles = await staffApi.initForm();
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
                    roleIds: data.roles.map(r => r.roleId),
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
                        roleIds: this.staff.roleIds,
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
        this.initForm();
        if (this.isEditMode) {
            this.loadStaffData();
        }
    }
};