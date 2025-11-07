
/**
 * Message Dialog Mixin
 * Provides reusable dialog methods for all components
 */
export const MessageDialogMixin = {
    data() {
        return {
            // Message dialog state - will be merged with component's existing data
            showDialog: false,
            dialogConfig: {
                type: 'info',
                title: '',
                message: '',
                showCancel: false,
                loading: false,
                okText: 'Đồng ý',
                cancelText: 'Hủy',
                onOk: () => {},
                onCancel: () => {}
            }
        };
    },
    
    methods: {
        /**
         * Show an information dialog
         * @param {string} message - Dialog message (supports HTML)
         * @param {string} title - Dialog title (optional)
         * @param {Function} onOk - Callback when OK is clicked (optional)
         */
        showInfoDialog(message, title = 'Thông tin', onOk = null) {
            this.dialogConfig = {
                type: 'info',
                title,
                message,
                showCancel: false,
                loading: false,
                okText: 'Đồng ý',
                cancelText: 'Hủy',
                onOk: onOk || (() => { this.showDialog = false; }),
                onCancel: () => {}
            };
            this.showDialog = true;
        },

        /**
         * Show a success dialog
         * @param {string} message - Dialog message (supports HTML)
         * @param {string} title - Dialog title (optional)
         * @param {Function} onOk - Callback when OK is clicked (optional)
         */
        showSuccessDialog(message, title = 'Thành công', onOk = null) {
            this.dialogConfig = {
                type: 'success',
                title,
                message,
                showCancel: false,
                loading: false,
                okText: 'Đồng ý',
                cancelText: 'Hủy',
                onOk: onOk || (() => { this.showDialog = false; }),
                onCancel: () => {}
            };
            this.showDialog = true;
        },

        /**
         * Show an error dialog
         * @param {string} message - Dialog message (supports HTML)
         * @param {string} title - Dialog title (optional)
         * @param {Function} onOk - Callback when OK is clicked (optional)
         */
        showErrorDialog(message, title = 'Lỗi', onOk = null) {
            this.dialogConfig = {
                type: 'error',
                title,
                message,
                showCancel: false,
                loading: false,
                okText: 'Đồng ý',
                cancelText: 'Hủy',
                onOk: onOk || (() => { this.showDialog = false; }),
                onCancel: () => {}
            };
            this.showDialog = true;
        },

        /**
         * Show a warning dialog
         * @param {string} message - Dialog message (supports HTML)
         * @param {string} title - Dialog title (optional)
         * @param {Function} onOk - Callback when OK is clicked (optional)
         */
        showWarningDialog(message, title = 'Cảnh báo', onOk = null) {
            this.dialogConfig = {
                type: 'warning',
                title,
                message,
                showCancel: false,
                loading: false,
                okText: 'Đồng ý',
                cancelText: 'Hủy',
                onOk: onOk || (() => { this.showDialog = false; }),
                onCancel: () => {}
            };
            this.showDialog = true;
        },

        /**
         * Show a confirmation dialog with Yes/No options
         * @param {string} message - Dialog message (supports HTML)
         * @param {string} title - Dialog title (optional)
         * @param {Function} onConfirm - Callback when confirmed (optional)
         * @param {Function} onCancel - Callback when cancelled (optional)
         * @param {Object} options - Additional options (okText, cancelText, etc.)
         */
        showConfirmDialog(message, title = 'Xác nhận', onConfirm = null, onCancel = null, options = {}) {
            this.dialogConfig = {
                type: 'warning',
                title,
                message,
                showCancel: true,
                loading: false,
                okText: options.okText || 'Xác nhận',
                cancelText: options.cancelText || 'Hủy',
                onOk: onConfirm || (() => { this.showDialog = false; }),
                onCancel: onCancel || (() => { this.showDialog = false; })
            };
            this.showDialog = true;
        },

        /**
         * Show a delete confirmation dialog
         * @param {string} itemName - Name of the item being deleted
         * @param {Function} onConfirm - Callback when deletion is confirmed
         * @param {Function} onCancel - Callback when cancelled (optional)
         */
        showDeleteConfirmDialog(itemName, onConfirm, onCancel = null) {
            const message = `Bạn có chắc chắn muốn xóa <strong>${itemName}</strong>?<br/>Hành động này không thể hoàn tác.`;
            this.showConfirmDialog(
                message,
                'Xác nhận xóa',
                onConfirm,
                onCancel,
                { okText: 'Xóa', cancelText: 'Hủy' }
            );
        },

        /**
         * Close the current dialog
         */
        closeDialog() {
            this.showDialog = false;
        },

        /**
         * Set dialog loading state
         * @param {boolean} loading - Loading state
         */
        setDialogLoading(loading) {
            if (this.dialogConfig) {
                this.dialogConfig.loading = loading;
            }
        }
    }
};
