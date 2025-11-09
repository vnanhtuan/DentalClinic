import api from '../../../js/utils/api.js';
import { handleApiError } from '../../../js/utils/errorHandler.js';

const API_URL = '/role';

export const roleApi = {
    async getAll() {
        try {
            const response = await api.get(API_URL);
            return response.data;
        } catch (error) {
            throw new Error(handleApiError(error));
        }
    },
    async getById(id) {
        try {
            const response = await api.get(`${API_URL}/${id}`);
            return response.data;
        } catch (error) {
            throw new Error(handleApiError(error));
        }
    },
    async create(roleCreateDto) {
        try {
            const response = await api.post(API_URL, roleCreateDto);
            return response.data;
        } catch (error) {
            throw new Error(handleApiError(error));
        }
    },
    async update(id, roleUpdateDto) {
        try {
            const response = await api.put(`${API_URL}/${id}`, roleUpdateDto);
            return response.data;
        } catch (error) {
            throw new Error(handleApiError(error));
        }
    },
    async delete(id) {
        try {
            const response = await api.delete(`${API_URL}/${id}`);
            return response.data;
        } catch (error) {
            throw new Error(handleApiError(error));
        }
    }
};