import { useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { searchApi } from '../api/search';
import Pagination from '../components/common/Pagination';
import { Loading, ErrorState, Empty } from '../components/common/StateViews';
import Seo from '../components/common/Seo';
import { formatDate } from '../utils/format';

export default function SearchPage() {
  const [params] = useSearchParams();
  const q = params.get('q') ?? '';
  const [page, setPage] = useState(1);

  const { data, isLoading, isError } = useQuery({
    queryKey: ['search', q, page],
    queryFn: () => searchApi.search(q, page, 12),
    enabled: q.length > 0,
  });

  return (
    <div className="container">
      <Seo title={`Search results for "${q}"`} />
      <h1 className="section-title">Search: {q}</h1>
      {isLoading && <Loading />}
      {isError && <ErrorState />}
      {data && data.items.length === 0 && <Empty message={`No results for "${q}".`} />}
      {data && data.items.length > 0 && (
        <>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
            {data.items.map((r) => (
              <Link
                key={`${r.type}-${r.slug}`}
                to={r.type === 'Movie' ? `/movie/${r.slug}` : `/article/${r.slug}`}
                className="card"
                style={{ display: 'flex', gap: 14, padding: 12 }}
              >
                {r.imageUrl && <img src={r.imageUrl} alt={r.title} style={{ width: 90, height: 60, objectFit: 'cover', borderRadius: 4 }} />}
                <div>
                  <span className="badge">{r.type}</span>
                  <h3 style={{ margin: '6px 0 4px', fontSize: 15 }}>{r.title}</h3>
                  {r.excerpt && <p style={{ margin: 0, fontSize: 13, color: '#777' }}>{r.excerpt}</p>}
                  {r.date && <span style={{ fontSize: 11, color: '#999' }}>{formatDate(r.date)}</span>}
                </div>
              </Link>
            ))}
          </div>
          <Pagination page={data.page} totalPages={data.totalPages} onChange={setPage} />
        </>
      )}
    </div>
  );
}
