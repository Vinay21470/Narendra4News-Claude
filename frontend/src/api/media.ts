import { apiClient } from './apiClient';
import type { ApiResponse } from '../types';

interface MediaDto {
  id: number;
  fileName: string;
  originalFileName: string;
  blobUrl: string;
  contentType: string;
  sizeBytes: number;
  createdDate: string;
}

export const mediaApi = {
  upload: (file: File, onProgress?: (pct: number) => void) => {
    const formData = new FormData();
    formData.append('file', file);
    return apiClient
      .post<ApiResponse<MediaDto>>('/media/upload', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
        onUploadProgress: (evt) => {
          if (onProgress && evt.total) onProgress(Math.round((evt.loaded / evt.total) * 100));
        },
      })
      .then((r) => r.data.data);
  },
};
