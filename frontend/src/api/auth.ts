import { apiClient } from './apiClient';
import type { ApiResponse, CurrentUser } from '../types';

interface AuthResponse {
  token: string;
  expiresAtUtc: string;
  userId: string;
  email: string;
  displayName: string;
  roles: string[];
}

export const authApi = {
  login: (email: string, password: string) =>
    apiClient.post<ApiResponse<AuthResponse>>('/auth/login', { email, password }).then((r) => r.data.data),
  register: (email: string, password: string, displayName: string) =>
    apiClient
      .post<ApiResponse<AuthResponse>>('/auth/register', { email, password, displayName })
      .then((r) => r.data.data),
  me: () => apiClient.get<ApiResponse<CurrentUser>>('/auth/me').then((r) => r.data.data),
};
