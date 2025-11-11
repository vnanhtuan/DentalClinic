import { router } from './router.js';
import { vuetify } from '../plugins/vuetify.js';
import { formatCurrency } from '../utils/formatters.js';
import { MessageDialogComponent } from './components/messageDialog.js';
import { MessageDialogMixin } from './mixins/messageDialogMixin.js';


const ManageApp = {
  template: `
        <v-app>
          <router-view></router-view>
        </v-app>
    `
};

const app = Vue.createApp(ManageApp);
app.use(vuetify);
app.use(router);

// Global component registration
app.component('MessageDialog', MessageDialogComponent);

// Register global mixin for message dialog methods
app.mixin(MessageDialogMixin);

// Make MessageDialog component available globally
window.MessageDialogComponent = MessageDialogComponent;

//Đăng ký hàm $formatCurrency để dùng ở mọi nơi
app.config.globalProperties.$formatCurrency = formatCurrency;

app.mount('#dental-manage-app');