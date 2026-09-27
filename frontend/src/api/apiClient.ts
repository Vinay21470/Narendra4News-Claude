import axios from 'axios';

// Base URL comes from environment config (VITE_API_BASE_URL), never
// hardcoded, so the same build can point at localhost in dev and the
// production Azure App Service URL when deployed (spec section 22/24).
export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? '/api',
  headers: { 'Content-Type': 'application/json' },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('n4n_token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('n4n_token');
      localStorage.removeItem('n4n_user');
    }
    return Promise.reject(error);
  }
);
