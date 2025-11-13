import api from '../../../js/utils/api.js';
import { handleApiError } from '../../../js/utils/errorHandler.js';
import { urlEncodeParams } from '../../../js/utils/formatters.js';

const API_URL = '/patient';

export const patientApi = {
    async getPaginated(params) {
        try {
            const response = await api.get(`${API_URL}/getpaginated?${urlEncodeParams(params)}`);
            return response.data;
        } catch (error) {
            console.error('Patient API error:', error);
            throw new Error(handleApiError(error));
        }
    },
    async initForm() {
        try {
            const response = await api.get(`${API_URL}/initform`);
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
    async create(patientCreateDto) {
        try {
            const response = await api.post(API_URL, patientCreateDto);
            return response.data;
        } catch (error) {
            throw new Error(handleApiError(error));
        }
    },
    async update(id, patientUpdateDto) {
        try {
            const response = await api.put(`${API_URL}/${id}`, patientUpdateDto);
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