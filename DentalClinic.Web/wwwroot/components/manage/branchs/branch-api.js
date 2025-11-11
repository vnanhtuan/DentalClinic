import api from '../../../js/utils/api.js';
import { handleApiError } from '../../../js/utils/errorHandler.js';
import { urlEncodeParams } from '../../../js/utils/formatters.js';

const API_URL = '/branch';

export const branchApi = {
    async getPaginated(params) {
        try {
            const response = await api.get(`${API_URL}/getpaginated?${urlEncodeParams(params)}`);
            return response.data;
        } catch (error) {
            console.error('Branch API error:', error);
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
    async create(branchCreateDto) {
        try {
            const response = await api.post(API_URL, branchCreateDto);
            return response.data;
        } catch (error) {
            throw new Error(handleApiError(error));
        }
    },
    async update(id, branchUpdateDto) {
        try {
            const response = await api.put(`${API_URL}/${id}`, branchUpdateDto);
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