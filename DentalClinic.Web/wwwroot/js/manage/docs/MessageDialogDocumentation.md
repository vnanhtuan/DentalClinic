# Message Dialog System Documentation

## Overview

The Message Dialog system provides a consistent, reusable way to display different types of dialogs (Info, Warning, Error, Success) throughout the dental clinic management application.

## Features

✅ **Multiple Dialog Types**: Info, Warning, Error, Success
✅ **Customizable Content**: Title, message, button text
✅ **Callback Support**: OK and Cancel button callbacks
✅ **Loading States**: Supports async operations
✅ **Responsive Design**: Works on mobile and desktop
✅ **Easy Integration**: Simple to use in any component

## Components

### 1. MessageDialogComponent
**Location**: `/js/manage/components/messageDialog.js`

The main Vue component that renders the dialog.

#### Props
| Prop | Type | Default | Description |
|------|------|---------|-------------|
| `modelValue` | Boolean | `false` | Controls dialog visibility |
| `type` | String | `'info'` | Dialog type: 'info', 'warning', 'error', 'success' |
| `title` | String | `''` | Dialog title (auto-generated if empty) |
| `message` | String | `''` | Dialog message (supports HTML) |
| `showCancel` | Boolean | `false` | Show cancel button |
| `showClose` | Boolean | `true` | Show close (X) button |
| `okText` | String | `'Đồng ý'` | OK button text |
| `cancelText` | String | `'Hủy'` | Cancel button text |
| `loading` | Boolean | `false` | Show loading state |

#### Events
| Event | Description |
|-------|-------------|
| `@ok` | Fired when OK button is clicked |
| `@cancel` | Fired when Cancel button is clicked |
| `@close` | Fired when Close (X) button is clicked |

### 2. Dialog Types & Styling

#### Info Dialog (`type="info"`)
- **Color**: Blue theme
- **Icon**: `mdi-information`
- **Use**: General information, system messages

#### Warning Dialog (`type="warning"`)
- **Color**: Orange theme  
- **Icon**: `mdi-alert`
- **Use**: Confirmations, potential issues

#### Error Dialog (`type="error"`)
- **Color**: Red theme
- **Icon**: `mdi-alert-circle`  
- **Use**: Error messages, failed operations

#### Success Dialog (`type="success"`)
- **Color**: Green theme
- **Icon**: `mdi-check-circle`
- **Use**: Success confirmations, completed operations

## Usage Examples

### Basic Usage

```javascript
// In your component
export const YourComponent = {
    template: `
        <div>
            <v-btn @click="showInfo">Show Info</v-btn>
            
            <message-dialog
                v-model="showDialog"
                :type="dialogConfig.type"
                :title="dialogConfig.title"
                :message="dialogConfig.message"
                :show-cancel="dialogConfig.showCancel"
                @ok="dialogConfig.onOk"
                @cancel="dialogConfig.onCancel">
            </message-dialog>
        </div>
    `,
    data() {
        return {
            showDialog: false,
            dialogConfig: {}
        };
    },
    methods: {
        showInfo() {
            this.dialogConfig = {
                type: 'info',
                title: 'Information',
                message: 'This is an info message',
                showCancel: false,
                onOk: () => { this.showDialog = false; }
            };
            this.showDialog = true;
        }
    }
};
```

### Confirmation Dialog with Async Operation

```javascript
confirmDelete(item) {
    this.dialogConfig = {
        type: 'warning',
        title: 'Xác nhận xóa',
        message: `Bạn có chắc chắn muốn xóa <strong>${item.name}</strong>?`,
        showCancel: true,
        loading: false,
        okText: 'Xóa',
        cancelText: 'Hủy',
        onOk: async () => {
            this.dialogConfig.loading = true;
            try {
                await this.deleteItem(item.id);
                this.showDialog = false;
                this.showSuccessMessage('Xóa thành công!');
            } catch (error) {
                this.dialogConfig.loading = false;
                this.showErrorMessage('Lỗi khi xóa dữ liệu');
            }
        },
        onCancel: () => { this.showDialog = false; }
    };
    this.showDialog = true;
}
```

### Helper Methods Pattern

```javascript
// Add these helper methods to your components
methods: {
    showInfoDialog(message, title = 'Thông tin', onOk = null) {
        this.dialogConfig = {
            type: 'info',
            title, message,
            showCancel: false,
            onOk: onOk || (() => { this.showDialog = false; })
        };
        this.showDialog = true;
    },
    
    showSuccessDialog(message, title = 'Thành công', onOk = null) {
        this.dialogConfig = {
            type: 'success',
            title, message,
            showCancel: false,
            onOk: onOk || (() => { this.showDialog = false; })
        };
        this.showDialog = true;
    },
    
    showErrorDialog(message, title = 'Lỗi') {
        this.dialogConfig = {
            type: 'error',
            title, message,
            showCancel: false,
            onOk: () => { this.showDialog = false; }
        };
        this.showDialog = true;
    },
    
    showConfirmDialog(message, title = 'Xác nhận', onConfirm = null) {
        this.dialogConfig = {
            type: 'warning',
            title, message,
            showCancel: true,
            okText: 'Xác nhận',
            cancelText: 'Hủy',
            onOk: onConfirm || (() => { this.showDialog = false; }),
            onCancel: () => { this.showDialog = false; }
        };
        this.showDialog = true;
    }
}
```

## Integration Steps

### 1. Import the Component
```javascript
import { MessageDialogComponent } from '../components/messageDialog.js';
```

### 2. Register in Component
```javascript
export const YourComponent = {
    components: {
        'message-dialog': MessageDialogComponent
    },
    // ... rest of component
};
```

### 3. Add Data Properties
```javascript
data() {
    return {
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
}
```

### 4. Add to Template
```html
<message-dialog
    v-model="showDialog"
    :type="dialogConfig.type"
    :title="dialogConfig.title"
    :message="dialogConfig.message"
    :show-cancel="dialogConfig.showCancel"
    :loading="dialogConfig.loading"
    :ok-text="dialogConfig.okText"
    :cancel-text="dialogConfig.cancelText"
    @ok="dialogConfig.onOk"
    @cancel="dialogConfig.onCancel">
</message-dialog>
```

## Best Practices

### 1. Message Content
- Keep messages clear and actionable
- Use HTML sparingly for emphasis (`<strong>`, `<br/>`)
- Provide context for the user

### 2. Button Text
- Use action-oriented text ("Xóa", "Lưu", "Tiếp tục")
- Keep text short but descriptive

### 3. Loading States
- Always set `loading: true` for async operations
- Reset loading state on error
- Close dialog automatically on success

### 4. Error Handling
```javascript
try {
    this.dialogConfig.loading = true;
    await someAsyncOperation();
    this.showDialog = false;
    this.showSuccessDialog('Thao tác thành công!');
} catch (error) {
    this.dialogConfig.loading = false;
    this.showErrorDialog(
        error.response?.data?.message || 'Đã xảy ra lỗi không mong muốn'
    );
}
```

## Current Implementation

The Message Dialog system has been successfully integrated into:

✅ **Staff Management**: 
- Delete confirmations
- Success/error notifications  
- Form validation messages

✅ **Global Registration**: 
- Available in all components
- Registered in site.js

## Future Enhancements

🔄 **Utility Functions**: Simplified API like `MessageDialog.info(message)`
🔄 **Toast Notifications**: Non-blocking notifications
🔄 **Custom Icons**: Support for custom icons
🔄 **Animation Options**: Different entrance/exit animations
