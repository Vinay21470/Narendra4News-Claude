import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { moviesApi } from '../api/movies';
import MovieCard from '../components/movie/MovieCard';
import Pagination from '../components/common/Pagination';
import { Loading, ErrorState, Empty } from '../components/common/StateViews';
import Seo from '../components/common/Seo';

export default function MoviesList() {
  const [page, setPage] = useState(1);
  const { data, isLoading, isError } = useQuery({
    queryKey: ['movies-list', page],
    queryFn: () => moviesApi.getAll(page, 12),
  });

  return (
    <div className="container">
      <Seo title="Movies" description="Browse all Telugu movies tracked on Narendra4News." />
      <h1 className="section-title">Movies</h1>
      {isLoading && <Loading />}
      {isError && <ErrorState />}
      {data && data.items.length === 0 && <Empty />}
      {data && data.items.length > 0 && (
        <>
          <div className="n4n-grid-3" style={{ gridTemplateColumns: 'repeat(auto-fill, minmax(160px, 1fr))' }}>
            {data.items.map((m) => <MovieCard key={m.id} movie={m} />)}
          </div>
          <Pagination page={data.page} totalPages={data.totalPages} onChange={setPage} />
        </>
      )}
    </div>
  );
}
