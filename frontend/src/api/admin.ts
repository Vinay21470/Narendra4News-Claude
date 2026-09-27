import { apiClient } from './apiClient';
import type { ApiResponse, DashboardStats } from '../types';

export const adminApi = {
  getDashboard: () => apiClient.get<ApiResponse<DashboardStats>>('/admin/dashboard').then((r) => r.data.data),
};
