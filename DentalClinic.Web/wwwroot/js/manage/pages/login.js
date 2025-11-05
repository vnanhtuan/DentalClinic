import api from '../api.js';
import { handleApiError } from '../../utils/errorHandler.js';

const response = await fetch('/components/manage/login.html');
const templateHtml = await response.text();

export const LoginPage = {
    template: templateHtml,
    data() {
        return {
            email: '',
            password: '',
            showPassword: false,
            loading: false,
            error: null
        };
    },
    methods: {
        async handleLogin() {
            this.error = null;
            try {
                const response = await api.post('/api/manage/auth/login', {
                    email: this.email,
                    password: this.password
                });
                localStorage.setItem('manage-token', response.data.token);
                this.$router.push({ name: 'Dashboard' });
            } catch (err) {
                this.error = handleApiError(err);
            } finally {
                this.loading = false;
            }
        }
    }
};