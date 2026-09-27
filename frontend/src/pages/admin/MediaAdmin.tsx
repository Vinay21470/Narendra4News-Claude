import { useQuery, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '../../api/apiClient';
import ImageUploader from '../../components/admin/ImageUploader';
import { Loading, ErrorState, Empty } from '../../components/common/StateViews';

interface MediaDto {
  id: number; fileName: string; originalFileName: string; blobUrl: string; contentType: string; sizeBytes: number; createdDate: string;
}

export default function MediaAdmin() {
  const qc = useQueryClient();
  const { data, isLoading, isError } = useQuery({
    queryKey: ['admin-media'],
    queryFn: () => apiClient.get<{ data: MediaDto[] }>('/media').then((r) => r.data.data),
  });

  function copyUrl(url: string) {
    navigator.clipboard?.writeText(url);
  }

  async function remove(id: number) {
    if (!confirm('Delete this file?')) return;
    await apiClient.delete(`/media/${id}`);
    qc.invalidateQueries({ queryKey: ['admin-media'] });
  }

  return (
    <div>
      <h1>Media Library</h1>
      <div className="card" style={{ padding: 20, marginBottom: 20, maxWidth: 420 }}>
        <p style={{ marginTop: 0, fontSize: 13, color: '#666' }}>Upload a new image (JPG, PNG, WEBP — max 8MB). It's stored in Azure Blob Storage; only the URL is saved.</p>
        <ImageUploader onChange={() => qc.invalidateQueries({ queryKey: ['admin-media'] })} />
      </div>

      {isLoading && <Loading />}
      {isError && <ErrorState />}
      {data && data.length === 0 && <Empty message="No media uploaded yet." />}
      {data && data.length > 0 && (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(160px, 1fr))', gap: 14 }}>
          {data.map((m) => (
            <div key={m.id} className="card" style={{ padding: 8 }}>
              <img src={m.blobUrl} alt={m.originalFileName} style={{ width: '100%', height: 110, objectFit: 'cover', borderRadius: 4 }} />
              <p style={{ fontSize: 11, margin: '6px 0', wordBreak: 'break-all' }}>{m.originalFileName}</p>
              <div style={{ display: 'flex', gap: 6 }}>
                <button className="btn-outline" style={{ fontSize: 11, padding: '4px 8px' }} onClick={() => copyUrl(m.blobUrl)}>Copy URL</button>
                <button className="btn-outline" style={{ fontSize: 11, padding: '4px 8px' }} onClick={() => remove(m.id)}>Delete</button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
