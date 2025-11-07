import api from '../../utils/api.js';
import { handleApiError } from '../../utils/errorHandler.js';

const API_URL = '/role';

export const roleApi = {
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
    async create(roleCreateDto) {
        try {
            const response = await api.post(API_URL, roleCreateDto);
            return response.data;
        } catch (error) {
            this.error = handleApiError(err);
        }
    },
    async update(id, roleUpdateDto) {
        try {
            const response = await api.put(`${API_URL}/${id}`, roleUpdateDto);
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