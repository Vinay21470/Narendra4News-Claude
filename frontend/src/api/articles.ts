import { apiClient } from './apiClient';
import type { ApiResponse, ArticleDetail, ArticleListItem, PagedResult } from '../types';

export interface ArticleQueryParams {
  categorySlug?: string;
  isFeatured?: boolean;
  isTrending?: boolean;
  page?: number;
  pageSize?: number;
}

export const articlesApi = {
  getPublished: (params: ArticleQueryParams) =>
    apiClient
      .get<ApiResponse<PagedResult<ArticleListItem>>>('/articles', { params })
      .then((r) => r.data.data),

  getTrending: (count = 6) =>
    apiClient
      .get<ApiResponse<ArticleListItem[]>>('/articles/trending', { params: { count } })
      .then((r) => r.data.data),

  getFeatured: (count = 5) =>
    apiClient
      .get<ApiResponse<ArticleListItem[]>>('/articles/featured', { params: { count } })
      .then((r) => r.data.data),

  getBySlug: (slug: string) =>
    apiClient.get<ApiResponse<ArticleDetail>>(`/articles/${slug}`).then((r) => r.data.data),

  getForAdmin: (id: number) => apiClient.get<ApiResponse<ArticleDetail>>(`/articles/admin/${id}`).then(r => r.data.data),

  getAllForAdmin: (page = 1, pageSize = 20) =>
    apiClient
      .get<ApiResponse<PagedResult<ArticleListItem>>>('/articles/admin/all', { params: { page, pageSize } })
      .then((r) => r.data.data),

  create: (payload: Record<string, unknown>) =>
    apiClient.post<ApiResponse<ArticleDetail>>('/articles', payload).then((r) => r.data.data),

  update: (id: number, payload: Record<string, unknown>) =>
    apiClient.put<ApiResponse<ArticleDetail>>(`/articles/${id}`, payload).then((r) => r.data.data),

  remove: (id: number) => apiClient.delete(`/articles/${id}`),

  setStatus: (id: number, status: string) => apiClient.patch(`/articles/${id}/status`, JSON.stringify(status)),
};
