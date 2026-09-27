import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { articlesApi } from '../../api/articles';
import DataTable from '../../components/admin/DataTable';
import { Loading, ErrorState } from '../../components/common/StateViews';
import Pagination from '../../components/common/Pagination';
import { formatDate } from '../../utils/format';

export default function ArticlesAdmin() {
  const [page, setPage] = useState(1);
  const qc = useQueryClient();
  const { data, isLoading, isError } = useQuery({
    queryKey: ['admin-articles', page],
    queryFn: () => articlesApi.getAllForAdmin(page, 20),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => articlesApi.remove(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-articles'] }),
  });

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <h1 style={{ margin: 0 }}>Articles</h1>
        <Link to="/admin/articles/create" className="btn-primary">+ New Article</Link>
      </div>

      {isLoading && <Loading />}
      {isError && <ErrorState />}
      {data && (
        <>
          <DataTable
            keyFn={(a) => a.id}
            rows={data.items}
            columns={[
              { header: 'Title', render: (a) => <Link to={`/admin/articles/edit/${a.id}`}>{a.title}</Link> },
              { header: 'Category', render: (a) => a.categoryName },
              { header: 'Published', render: (a) => formatDate(a.publishedDate) || '—' },
              { header: 'Views', render: (a) => a.views },
              {
                header: 'Actions',
                render: (a) => (
                  <div style={{ display: 'flex', gap: 8 }}>
                    <Link to={`/admin/articles/edit/${a.id}`} className="btn-outline">Edit</Link>
                    <button
                      className="btn-outline"
                      onClick={() => {
                        if (confirm(`Delete "${a.title}"?`)) deleteMutation.mutate(a.id);
                      }}
                    >
                      Delete
                    </button>
                  </div>
                ),
              },
            ]}
          />
          <Pagination page={data.page} totalPages={data.totalPages} onChange={setPage} />
        </>
      )}
    </div>
  );
}
