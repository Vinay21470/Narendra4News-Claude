import { apiClient } from './apiClient';
import type { ApiResponse, MovieCollectionRecord, MovieDetail, MovieListItem, PagedResult } from '../types';

export const moviesApi = {
  getAll: (page = 1, pageSize = 12) =>
    apiClient
      .get<ApiResponse<PagedResult<MovieListItem>>>('/movies', { params: { page, pageSize } })
      .then((r) => r.data.data),

  getBySlug: (slug: string) =>
    apiClient.get<ApiResponse<MovieDetail>>(`/movies/${slug}`).then((r) => r.data.data),

  create: (payload: Record<string, unknown>) =>
    apiClient.post<ApiResponse<MovieDetail>>('/movies', payload).then((r) => r.data.data),

  update: (id: number, payload: Record<string, unknown>) =>
    apiClient.put<ApiResponse<MovieDetail>>(`/movies/${id}`, payload).then((r) => r.data.data),

  addCollection: (movieId: number, payload: Record<string, unknown>) =>
    apiClient
      .post<ApiResponse<MovieCollectionRecord>>(`/movies/${movieId}/collections`, payload)
      .then((r) => r.data.data),
};
