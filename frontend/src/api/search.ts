import { apiClient } from './apiClient';
import type { ApiResponse, PagedResult, SearchResult } from '../types';

export const searchApi = {
  search: (q: string, page = 1, pageSize = 12) =>
    apiClient
      .get<ApiResponse<PagedResult<SearchResult>>>('/search', { params: { q, page, pageSize } })
      .then((r) => r.data.data),
};
