import api from '../../../js/utils/api.js';
import { handleApiError } from '../../../js/utils/errorHandler.js';

const API_URL = '/staff';

export const staffApi = {
    async getAll() {
        try {
            const response = await api.get(API_URL);
            return response.data;
        } catch (error) {
            this.error = handleApiError(err);
        }
    },
    async getById(id) {
        try {
            const response = await api.get(`${API_URL}/${id}`);
            return response.data;
        } catch (error) {
            this.error = handleApiError(err);
        }
    },
    async create(staffCreateDto) {
        try {
            const response = await api.post(API_URL, staffCreateDto);
            return response.data;
        } catch (error) {
            this.error = handleApiError(err);
        }
    },
    async update(id, staffUpdateDto) {
        try {
            const response = await api.put(`${API_URL}/${id}`, staffUpdateDto);
            return response.data;
        } catch (error) {
            this.error = handleApiError(err);
        }
    },
    async delete(id) {
        try {
            const response = await api.delete(`${API_URL}/${id}`);
            return response.data;
        } catch (error) {
            this.error = handleApiError(err);
        }
    }
};