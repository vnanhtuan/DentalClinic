// User service for managing authentication state and user information
export const userService = {
    // Key for localStorage
    USER_KEY: 'dental_user_info',
    TOKEN_KEY: 'dental_auth_token',

    // Set user information after login
    setUser(loginResponse) {
        if (loginResponse.token) {
            localStorage.setItem(this.TOKEN_KEY, loginResponse.token);
        }
        if (loginResponse.fullName && loginResponse.role) {
            const userInfo = {
                fullName: loginResponse.fullName,
                role: loginResponse.role,
                loginTime: new Date().toISOString()
            };
            localStorage.setItem(this.USER_KEY, JSON.stringify(userInfo));
        }
    },

    // Get current user information
    getCurrentUser() {
        try {
            const userInfoStr = localStorage.getItem(this.USER_KEY);
            if (userInfoStr) {
                return JSON.parse(userInfoStr);
            }
        } catch (error) {
            console.error('Error parsing user info:', error);
        }
        return null;
    },

    // Get authentication token
    getToken() {
        return localStorage.getItem(this.TOKEN_KEY);
    },

    // Check if user is logged in
    isLoggedIn() {
        return this.getToken() !== null && this.getCurrentUser() !== null;
    },

    // Clear user data on logout
    logout() {
        localStorage.removeItem(this.TOKEN_KEY);
        localStorage.removeItem(this.USER_KEY);
    },

    // Generate avatar from full name
    generateAvatar(fullName) {
        if (!fullName) return '';
        
        // Get initials (first letter of each word, max 2 characters)
        const words = fullName.trim().split(/\s+/);
        let initials = '';
        
        if (words.length >= 2) {
            // Take first letter of first and last word
            initials = words[0][0] + words[words.length - 1][0];
        } else if (words.length === 1) {
            // Take first two letters of the name
            initials = words[0].substring(0, 2);
        }
        
        return initials.toUpperCase();
    },

    // Generate avatar color based on name
    getAvatarColor(fullName) {
        if (!fullName) return '#9E9E9E';
        
        const colors = [
            '#F44336', '#E91E63', '#9C27B0', '#673AB7', 
            '#3F51B5', '#2196F3', '#03A9F4', '#00BCD4',
            '#009688', '#4CAF50', '#8BC34A', '#CDDC39',
            '#FF9800', '#FF5722', '#795548', '#607D8B'
        ];
        
        // Generate a consistent color based on name hash
        let hash = 0;
        for (let i = 0; i < fullName.length; i++) {
            hash = fullName.charCodeAt(i) + ((hash << 5) - hash);
        }
        hash = Math.abs(hash);
        
        return colors[hash % colors.length];
    },

    // Get role display information
    getRoleInfo(role) {
        const roleMap = {
            'Admin': {
                displayName: 'Quản trị viên',
                color: 'red-darken-1',
                icon: 'mdi-shield-crown'
            },
            'Doctor': {
                displayName: 'Bác sĩ',
                color: 'blue-darken-1',
                icon: 'mdi-doctor'
            },
            'Dentist': {
                displayName: 'Bác sĩ nha khoa',
                color: 'blue-darken-1',
                icon: 'mdi-tooth'
            },
            'Receptionist': {
                displayName: 'Lễ tân',
                color: 'green-darken-1',
                icon: 'mdi-account-tie'
            },
            'Manager': {
                displayName: 'Quản lý',
                color: 'purple-darken-1',
                icon: 'mdi-account-star'
            }
        };
        
        return roleMap[role] || {
            displayName: role || 'Nhân viên',
            color: 'grey-darken-1',
            icon: 'mdi-account'
        };
    }
};
