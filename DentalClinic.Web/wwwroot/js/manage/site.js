import { router } from './router.js';
import { vuetify } from '../plugins/vuetify.js';
import { formatCurrency } from '../utils/formatters.js';

// 2. Định nghĩa Root Component của Admin App
// Component này sẽ chứa <router-view> cho các trang admin
// Tạm thời không sử dụng, đã định nghĩa router-view ở _ManageLayout.cshtml
const ManageApp = {
    template: `
      <router-view></router-view>
    `
};

const app = Vue.createApp({});
app.use(vuetify);
app.use(router);

//Đăng ký hàm $formatCurrency để dùng ở mọi nơi
app.config.globalProperties.$formatCurrency = formatCurrency;

app.mount('#dental-manage-app');