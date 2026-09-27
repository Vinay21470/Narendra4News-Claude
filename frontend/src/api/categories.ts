import { apiClient } from './apiClient';
import type { ApiResponse, Category } from '../types';

export const categoriesApi = {
  getAll: (activeOnly = true) =>
    apiClient.get<ApiResponse<Category[]>>('/categories', { params: { activeOnly } }).then((r) => r.data.data),
  create: (payload: Record<string, unknown>) =>
    apiClient.post<ApiResponse<Category>>('/categories', payload).then((r) => r.data.data),
  update: (id: number, payload: Record<string, unknown>) =>
    apiClient.put<ApiResponse<Category>>(`/categories/${id}`, payload).then((r) => r.data.data),
  remove: (id: number) => apiClient.delete(`/categories/${id}`),
};
