// 1. Tạo một instance Axios
const api = axios.create({
    baseURL: '/' // URL gốc cho mọi API call
});

// 2. TẠO MỘT "REQUEST INTERCEPTOR" (BỘ ĐÁNH CHẶN REQUEST)
// Đoạn code này sẽ chạy TRƯỚC KHI bất kỳ request nào được gửi đi
api.interceptors.request.use(
    (config) => {
        // 3. Lấy token từ localStorage
        const token = localStorage.getItem('manage-token');

        // 4. Nếu token tồn tại, gán nó vào header
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }

        return config; // Gửi request đi với header mới
    },
    (error) => {
        return Promise.reject(error);
    }
);

// (Dùng để bắt lỗi 401 - Unauthorized)
api.interceptors.response.use(
    // (response) => response: 
    // Nếu response thành công (status 2xx), cứ cho nó đi qua
    (response) => {
        return response;
    },

    // (error) => { ... }:
    // Nếu response bị lỗi, nó sẽ chạy vào hàm này
    (error) => {
        // Kiểm tra xem có lỗi 401 (Unauthorized) không
        if (error.response && error.response.status === 401) {

            // 1. Xóa token cũ/hết hạn
            localStorage.removeItem('manage-token');

            // 2. Chuyển hướng về trang login
            // Dùng window.location để reload lại trang, 
            // đảm bảo mọi trạng thái của Vue được reset sạch sẽ.
            window.location.href = '/manage/login';
        }

        // 3. Đối với các lỗi khác (như 404, 500), 
        // cứ để component tự xử lý (vào khối 'catch')
        return Promise.reject(error);
    }
);

// 5. Export instance đã được cấu hình
export default api;