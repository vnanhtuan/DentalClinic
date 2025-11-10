import { roleApi } from './role-api.js';
import { MessageDialogMixin } from '../../../js/manage/mixins/messageDialogMixin.js';
import { PAGE_SIZE_OPTIONS, DEFAULT_PAGE_SIZE } from '../../../js/manage/constants/paginationConstants.js';

const roleListResponse = await fetch('/components/manage/roles/roleList.html');
const roleListHtml = await roleListResponse.text();

export const SettingsPage = {
    template: `<router-view></router-view>`
};

export const RoleListComponent = {
    template: roleListHtml,
    mixins: [MessageDialogMixin],

    data() {
        return {
            loading: true,
            itemToDelete: {},
            headers: [
                { title: 'STT', key: 'stt', sortable: false, width: '80px' },
                { title: 'Tên Vai Trò (Codename)', key: 'name' },
                { title: 'Mô tả', key: 'description' },
                { title: 'Hành động', key: 'actions', sortable: false, align: 'end' },],
            roles: [],
            pageSize: DEFAULT_PAGE_SIZE,
            pageSizeOptions: PAGE_SIZE_OPTIONS,

            // DIALOGS FORM
            showFormDialog: false,
            formLoading: false,
            formTitle: '',
            editedRoleId: null,
            editedRole: {
                roleName: '',
                description: '',
                color: ''
            },
            defaultRole: {
                roleName: '',
                description: '',
                color: ''
            },
            formError: null,
            rules: {
                required: v => !!v || 'Tên vai trò là bắt buộc.',
            }
        };
    },
    computed: {
        isMobile() {
            return this.$vuetify.display.smAndDown;
        },
        isEditMode() {
            return !!this.editedRoleId;
        }
    },
    watch: {
        // pageSize watcher placeholder - if role list switches to server-side pagination, uncomment
        // pageSize() {
        //     this.currentPage = 1;
        //     this.fetchRoles();
        // }
    },
    methods: {
        async fetchRoles() {
            this.loading = true;
            try {
                this.roles = await roleApi.getAll();
            } catch (err) {
               this.showErrorDialog('Không thể tải danh sách vai trò.', 'Lỗi tải dữ liệu');
            } finally {
                this.loading = false;
            }
        },
        confirmDelete(item) {
            this.itemToDelete = item;
            this.showDeleteConfirmDialog(item.name, this.deleteRole);
        },
        async deleteRole() {
            this.setDialogLoading(true);
            try {
                await roleApi.delete(this.itemToDelete.roleId);
                this.closeDialog();
                this.showSuccessDialog(
                    `Vai trò "${this.itemToDelete.name}" đã được xóa thành công.`,
                    'Xóa thành công'
                );
                await this.fetchRoles();
            } catch (err) {
                this.showErrorDialog(
                    'Không thể xóa vai trò này. Vui lòng thử lại.',
                    'Lỗi xóa dữ liệu'
                );
            } finally {
                this.setDialogLoading(false);
            }
        },
        openCreateDialog() {
            this.editedRoleId = null;
            // Dùng Object.assign để tạo bản sao, tránh tham chiếu
            this.editedRole = Object.assign({}, this.defaultRole);
            this.formTitle = 'Tạo mới vai trò';
            this.formError = null;
            this.$refs.roleForm?.resetValidation(); // Reset validation (nếu có)
            this.showFormDialog = true;
        },
        openEditDialog(item) {
            this.editedRoleId = item.roleId;
            // Dùng Object.assign để copy item vào form, tránh thay đổi list
            this.editedRole = Object.assign({}, item);
            this.formTitle = 'Chỉnh sửa vai trò';
            this.formError = null;
            this.$refs.roleForm?.resetValidation();
            this.showFormDialog = true;
        },
        closeFormDialog() {
            this.showFormDialog = false;
        },
        async saveRole() {
            const { valid } = await this.$refs.roleForm.validate();
            if (!valid) return;

            this.formLoading = true;
            this.formError = null;

            try {
                if (this.isEditMode) {
                    await roleApi.update(this.editedRoleId, this.editedRole);
                } else {
                    await roleApi.create(this.editedRole);
                }

                this.closeFormDialog(); // Đóng dialog form
                await this.fetchRoles(); // Tải lại danh sách
                this.showSuccessDialog('Đã lưu vai trò thành công.', 'Lưu thành công');

            } catch (err) {
                this.formError = err.response?.data?.message || 'Đã xảy ra lỗi khi lưu.';
                this.showErrorDialog(
                    'Đã xảy ra lỗi khi lưu. Vui lòng thử lại.',
                    'Lỗi khi lưu dữ liệu'
                );
            } finally {
                this.formLoading = false;
            }
        }
    },
    mounted() {
        this.fetchRoles();
    }
};
