import { roleApi } from '../services/role-api.js';

const roleListResponse = await fetch('/components/manage/roleList.html');
const roleListHtml = await roleListResponse.text();
const roleFormResponse = await fetch('/components/manage/roleForm.html');
const roleFormHtml = await roleFormResponse.text();

export const SettingsPage = {
    template: `<router-view></router-view>`
};

export const RoleListComponent = {
    template: roleListHtml,

    data() {
        return {
            loading: true,
            itemToDelete: {},
            headers: [
                { title: 'Tên Vai Trò (RoleName)', key: 'name' },
                { title: 'Mô tả', key: 'description' },
                { title: 'Hành động', key: 'actions', sortable: false, align: 'end' },
            ],
            roles: [],
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
        isMobile() {
            return this.$vuetify.display.smAndDown;
        }
    },
    methods: {
        async fetchRoles() {
            this.loading = true;
            try {
                this.roles = await roleApi.getAll();
            } catch (err) {
                // (Thêm thông báo lỗi)
            } finally {
                this.loading = false;
            }
        },
        goToCreate() {
            this.$router.push({ name: 'RoleCreate' });
        },
        goToEdit(id) {
            this.$router.push({ name: 'RoleEdit', params: { id: id } });
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
        }
    },
    mounted() {
        this.fetchRoles();
    }
};

// 3. Component Form (Tạo mới/Chỉnh sửa Role)
export const RoleFormPage = {
    template: roleFormHtml,
    data() {
        return {
            loading: false,
            roleId: this.$route.params.id || null,
            role: {
                name: '',
                description: '',
                color: ''
            },
            error: null,
            rules: {
                required: v => !!v || 'Tên vai trò là bắt buộc.',
            }
        };
    },
    computed: {
        isEditMode() {
            return !!this.roleId;
        },
        formTitle() {
            return this.isEditMode ? 'Chỉnh sửa Vai trò' : 'Tạo mới Vai trò';
        }
    },
    methods: {
        async loadRoleData() {
            this.loading = true;
            try {
                const data = await roleApi.getById(this.roleId);
                this.role = data;
            } catch (err) {
                this.error = 'Không thể tải dữ liệu vai trò.';
            } finally {
                this.loading = false;
            }
        },
        async saveRole() {
            const { valid } = await this.$refs.roleForm.validate();
            if (!valid) return;

            this.loading = true;
            this.error = null;

            try {
                if (this.isEditMode) {
                    await roleApi.update(this.roleId, this.role);
                } else {
                    await roleApi.create(this.role);
                }
                this.goBack();
            } catch (err) {
                this.error = err.response?.data?.message || 'Đã xảy ra lỗi khi lưu.';
            } finally {
                this.loading = false;
            }
        },
        goBack() {
            this.$router.push({ name: 'RoleList' });
        }
    },
    created() {
        if (this.isEditMode) {
            this.loadRoleData();
        }
    }
};