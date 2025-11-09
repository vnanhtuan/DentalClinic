import api from '../../../js/utils/api.js';
import { handleApiError } from '../../../js/utils/errorHandler.js';
import { userService } from '../../../js/manage/services/user-service.js';

const response = await fetch('/components/manage/logins/login.html');
const templateHtml = await response.text();

export const LoginPage = {
    template: templateHtml,
    data() {
        return {
            username: '',
            password: '',
            showPassword: false,
            rememberMe: false,
            loading: false,
            error: null,
            rules: {
                required: value => !!value || 'This field is required.'            }
        };
    },
    mounted() {
    },
    methods: {async handleLogin() {
            const { valid } = await this.$refs.loginForm.validate();
            if (!valid) return;

            this.loading = true;
            this.error = null;

            try {
                const response = await api.post('/auth/login', {
                    username: this.username,
                    password: this.password
                });

                // Store user information using the service
                userService.setUser(response.data);

                this.$router.push({ name: 'Dashboard' });
            } catch (err) {
                this.error = handleApiError(err);
            } finally {
                this.loading = false;
            }
        },
        loginWithGoogle() {
            // Logic để đăng nhập bằng Google (cần tích hợp Google OAuth)
            console.log('Login with Google clicked');
            alert('Chức năng đăng nhập bằng Google chưa được triển khai.');
        }
    }
};