export const handleApiError = (error) => {
    if (error.response) {
        switch (error.response.status) {
            case 400:
                return 'Dữ liệu không hợp lệ';
            case 401:
                return 'Phiên đăng nhập đã hết hạn';
            case 403:
                return 'Bạn không có quyền truy cập';
            case 404:
                return 'Không tìm thấy tài nguyên yêu cầu';
            case 422:
                return 'Dữ liệu không hợp lệ';
            case 500:
                return 'Lỗi máy chủ nội bộ';
            default:
                return `Đã có lỗi xảy ra (${error.response.status})`;
        }
    }

    if (error.request) {
        // Request was made but no response received
        return 'Không thể kết nối đến máy chủ';
    }

    // Something happened in setting up the request
    return 'Đã xảy ra lỗi khi gửi yêu cầu';
};

/**
 * Formats validation errors from API response
 * @param {Object} validationErrors - Object containing validation errors
 * @returns {string[]} Array of error messages
 */
export const formatValidationErrors = (validationErrors) => {
    if (!validationErrors) return [];
    return Object.values(validationErrors).flat();
};