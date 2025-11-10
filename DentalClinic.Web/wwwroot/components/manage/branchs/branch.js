import { branchApi } from './branch-api.js';
import { MessageDialogMixin } from '../../../js/manage/mixins/messageDialogMixin.js';
import { PAGE_SIZE_OPTIONS, DEFAULT_PAGE_SIZE } from '../../../js/manage/constants/paginationConstants.js';

const branchListResponse = await fetch('/components/manage/branchs/branchList.html');
const branchListHtml = await branchListResponse.text();
const branchFormResponse = await fetch('/components/manage/branchs/branchForm.html');
const branchFormHtml = await branchFormResponse.text();

export const BranchesPage = {
    template: `<router-view></router-view>`
};

export const BranchListComponent = {
    template: branchListHtml,
    mixins: [MessageDialogMixin],

    data() {
        return {
            loading: true,
            itemToDelete: {},
            headers: [
                { title: 'STT', key: 'stt', sortable: false, width: '50px' },
                { title: 'Tên', key: 'branchName' },
                { title: 'Địa chỉ', key: 'address' },
                { title: 'SĐT', key: 'phone' },
                { title: 'Email', key: 'email' },
                { title: 'CN chính', key: 'isMainBranch' },
                { title: 'Hành động', key: 'actions', sortable: false, align: 'end' },],
            branches: [],
            statusFilter: 'all', // all | active | inactive
            totalItems: 0,
            totalPages: 0,
            currentPage: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            pageSizeOptions: PAGE_SIZE_OPTIONS,
            formError: null,
            rules: {
                required: v => !!v || 'Tên chi nhánh là bắt buộc.',
            }
        };
    },
    computed: {
        // screen size helper
        isMobile() {
            return this.$vuetify.display.smAndDown;
        },

        // filtered list according to statusFilter
        filteredBranches() {
            if (!this.branches) return [];
            if (this.statusFilter === 'active') return this.branches.filter(b => b.isActive);
            if (this.statusFilter === 'inactive') return this.branches.filter(b => !b.isActive);
            return this.branches;
        },
        fromItem() {
            if (!this.filteredBranches || this.filteredBranches.length === 0) return 0;
            return (this.currentPage - 1) * this.pageSize + 1;
        },
        toItem() {
            if (!this.filteredBranches || this.filteredBranches.length === 0) return 0;
            // show end index for current page (may be less than pageSize)
            const end = (this.currentPage - 1) * this.pageSize + this.pageSize;
            return Math.min(end, this.totalItems);
        }
    },
    methods: {
        async fetchBranches() {
            this.loading = true;
            try {
                this.branches = await branchApi.getAll();
                // keep currentPage valid
                if (this.currentPage > this.totalPages) this.currentPage = this.totalPages || 1;
            } catch (err) {
                this.showErrorDialog('Không thể tải danh sách chi nhánh.', 'Lỗi tải dữ liệu');
            } finally {
                this.loading = false;
            }
        },
        goToCreate() {
            this.$router.push({ name: 'BranchCreate' });
        },
        goToEdit(id) {
            this.$router.push({ name: 'BranchEdit', params: { id: id } });
        },
        confirmDelete(item) {
            this.itemToDelete = item;
            this.showDeleteConfirmDialog(item.branchName || item.name, this.executeDelete.bind(this));
        },
        async executeDelete() {
            this.setDialogLoading(true);
            try {
                const id = this.itemToDelete.branchId || this.itemToDelete.id;
                await branchApi.delete(id);
                this.closeDialog();
                await this.fetchBranches();
                this.showSuccessDialog(`Chi nhánh "${this.itemToDelete.branchName || ''}" đã được xóa.`, 'Xóa thành công');
            } catch (err) {
                this.setDialogLoading(false);
                this.showErrorDialog('Không thể xóa chi nhánh. Vui lòng thử lại.', 'Lỗi xóa dữ liệu');
            }
        },
        async toggleActive(item) {
            // If currently active, ask for confirmation before deactivating
            const willActivate = !item.isActive;
            if (!willActivate) {
                this.showConfirmDialog(
                    `Bạn có chắc chắn muốn ngừng hoạt động chi nhánh "${item.branchName}"?`,
                    'Xác nhận',
                    async () => {
                        await this._toggleActiveExecute(item, false);
                        this.closeDialog();
                    }
                );
                return;
            }
            // Activating - immediate
            await this._toggleActiveExecute(item, true);
        },

        async _toggleActiveExecute(item, willActivate) {
            this.loading = true;
            try {
                // build update dto from existing item (backend expects full shape or at least isActive)
                const updateDto = {
                    branchName: item.branchName,
                    branchCode: item.branchCode,
                    address: item.address,
                    city: item.city,
                    district: item.district,
                    phone: item.phone,
                    email: item.email,
                    isMainBranch: item.isMainBranch,
                    isActive: willActivate
                };
                await branchApi.update(item.branchId, updateDto);
                await this.fetchBranches();
                this.showSuccessDialog(`Chi nhánh "${item.branchName}" đã được ${willActivate ? 'kích hoạt' : 'ngừng hoạt động'}.`, 'Cập nhật trạng thái');
            } catch (err) {
                this.showErrorDialog('Không thể cập nhật trạng thái chi nhánh. Vui lòng thử lại.', 'Lỗi cập nhật');
            } finally {
                this.loading = false;
            }
        },
    },
    mounted() {
        this.fetchBranches();
    }
};

// Branch Form Component (create/edit)
export const BranchFormComponent = {
    template: branchFormHtml,
    mixins: [MessageDialogMixin],
    data() {
        return {
            loading: false,
            branchId: this.$route.params.id || null,
            branch: {
                branchName: '', branchCode: '', address: '', city: '', district: '', phone: '', email: '', isMainBranch: false, isActive: true
            },
            error: null,
            rules: {
                required: v => !!v || 'Thông tin bắt buộc.',
                email: v => (!v || /.+@.+\..+/.test(v)) || 'Email không hợp lệ.'
            }
        };
    },
    computed: {
        isEditMode() {
            return !!this.branchId;
        },
        formTitle() {
            return this.isEditMode ? 'Chỉnh sửa Chi nhánh' : 'Tạo mới Chi nhánh';
        }
    },
    methods: {
        async loadBranchData() {
            if (!this.isEditMode) return;
            this.loading = true;
            try {
                const data = await branchApi.getById(this.branchId);
                this.branch = {
                    branchName: data.branchName,
                    branchCode: data.branchCode,
                    address: data.address,
                    city: data.city,
                    district: data.district,
                    phone: data.phone,
                    email: data.email,
                    isMainBranch: data.isMainBranch,
                    isActive: data.isActive
                };
            } catch (err) {
                this.showErrorDialog('Không thể tải dữ liệu chi nhánh.', 'Lỗi tải dữ liệu');
            } finally {
                this.loading = false;
            }
        },
        async saveBranch() {
            const { valid } = await this.$refs.branchForm.validate();
            if (!valid) {
                this.showWarningDialog('Vui lòng điền đầy đủ thông tin hợp lệ.', 'Thông tin chưa hợp lệ');
                return;
            }

            this.loading = true;
            this.error = null;
            try {
                if (this.isEditMode) {
                    await branchApi.update(this.branchId, this.branch);
                    this.showSuccessDialog('Chi nhánh đã được cập nhật thành công.', 'Cập nhật thành công', () => this.goBack());
                } else {
                    await branchApi.create(this.branch);
                    this.showSuccessDialog('Chi nhánh mới đã được tạo thành công.', 'Tạo thành công', () => this.goBack());
                }
            } catch (err) {
                const errorMessage = err.message || 'Đã xảy ra lỗi khi lưu thông tin.';
                this.showErrorDialog(errorMessage, 'Lỗi lưu dữ liệu');
            } finally {
                this.loading = false;
            }
        },
        goBack() {
            // Navigate back to branch list route
            this.$router.push({ name: 'BranchList' });
        }
    },
    created() {
        if (this.isEditMode) this.loadBranchData();
    }
};