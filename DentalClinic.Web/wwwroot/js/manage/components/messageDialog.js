
const response = await fetch('/components/manage/messageDialog.html');
const templateHtml = await response.text();

export const MessageDialogComponent = {
    name: 'MessageDialog',
    template: templateHtml,
    props: {
        // Controls visibility of dialog
        modelValue: {
            type: Boolean,
            default: false
        },
        // Dialog type: 'info', 'warning', 'error', 'success'
        type: {
            type: String,
            default: 'info',
            validator: value => ['info', 'warning', 'error', 'success'].includes(value)
        },
        // Dialog title
        title: {
            type: String,
            default: ''
        },
        // Dialog message (supports HTML)
        message: {
            type: String,
            default: ''
        },
        // Show cancel button
        showCancel: {
            type: Boolean,
            default: false
        },
        // Show close button (X)
        showClose: {
            type: Boolean,
            default: true
        },
        // Custom text for buttons
        okText: {
            type: String,
            default: 'Đồng ý'
        },
        cancelText: {
            type: String,
            default: 'Hủy'
        },
        // Loading state for async operations
        loading: {
            type: Boolean,
            default: false
        }
    },
    emits: ['update:modelValue', 'ok', 'cancel', 'close'],
    computed: {
        show: {
            get() {
                return this.modelValue;
            },
            set(value) {
                this.$emit('update:modelValue', value);
            }
        },
        dialogTitle() {
            if (this.title) return this.title;
            
            // Default titles based on type
            const defaultTitles = {
                info: 'Thông tin',
                warning: 'Cảnh báo',
                error: 'Lỗi',
                success: 'Thành công'
            };
            return defaultTitles[this.type];
        },
        dialogMessage() {
            return this.message;
        },
        dialogIcon() {
            const icons = {
                info: 'mdi-information',
                warning: 'mdi-alert',
                error: 'mdi-alert-circle',
                success: 'mdi-check-circle'
            };
            return icons[this.type];
        },
        headerColorClass() {
            const colorClasses = {
                info: 'bg-blue-lighten-4',
                warning: 'bg-orange-lighten-4',
                error: 'bg-red-lighten-4',
                success: 'bg-green-lighten-4'
            };
            return colorClasses[this.type];
        },
        headerTextClass() {
            const textClasses = {
                info: 'text-blue-darken-2',
                warning: 'text-orange-darken-2',
                error: 'text-red-darken-2',
                success: 'text-green-darken-2'
            };
            return textClasses[this.type];
        },
        buttonColor() {
            const buttonColors = {
                info: 'primary',
                warning: 'orange-darken-1',
                error: 'error',
                success: 'success'
            };
            return buttonColors[this.type];
        },
        showCancelButton() {
            return this.showCancel;
        },
        showCloseButton() {
            return this.showClose;
        }
    },
    methods: {
        handleOk() {
            this.$emit('ok');
            if (!this.loading) {
                this.show = false;
            }
        },
        handleCancel() {
            this.$emit('cancel');
            this.show = false;
        },
        handleClose() {
            this.$emit('close');
            this.show = false;
        }
    }
};
