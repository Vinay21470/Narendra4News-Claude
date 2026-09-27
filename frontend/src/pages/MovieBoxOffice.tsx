import { useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { moviesApi } from '../api/movies';
import { formatDate, formatCrore } from '../utils/format';
import { Loading, ErrorState, Empty } from '../components/common/StateViews';
import Seo from '../components/common/Seo';

// Full historical timeline for one movie - Day 1, Day 2, ... Weekend,
// Week 1, Lifetime - each an independent, permanently-stored row
// (spec section 34).
export default function MovieBoxOffice() {
  const { slug = '' } = useParams();
  const { data: movie, isLoading, isError } = useQuery({
    queryKey: ['movie', slug],
    queryFn: () => moviesApi.getBySlug(slug),
  });

  if (isLoading) return <div className="container"><Loading /></div>;
  if (isError || !movie) return <div className="container"><ErrorState message="Movie not found." /></div>;

  return (
    <div className="container">
      <Seo title={`${movie.movieName} Box Office Collections`} description={`Full historical box office collection timeline for ${movie.movieName}.`} />
      <h1 className="section-title">{movie.movieName} — Box Office History</h1>
      {movie.collections.length === 0 && <Empty message="No collection records yet." />}
      {movie.collections.length > 0 && (
        <div style={{ overflowX: 'auto' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse', background: '#fff', fontSize: 14 }}>
            <thead>
              <tr style={{ background: 'var(--n4n-bg-alt)', textAlign: 'left' }}>
                {['Date', 'Day / Label', 'India Net', 'India Gross', 'Overseas', 'Worldwide', 'Total'].map((h) => (
                  <th key={h} style={{ padding: '10px 12px', borderBottom: '2px solid var(--n4n-border)' }}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {movie.collections.map((c) => (
                <tr key={c.id}>
                  <td style={{ padding: '10px 12px', borderBottom: '1px solid var(--n4n-border)' }}>{formatDate(c.collectionDate)}</td>
                  <td style={{ padding: '10px 12px', borderBottom: '1px solid var(--n4n-border)' }}>{c.label ?? `Day ${c.dayNumber}`}</td>
                  <td style={{ padding: '10px 12px', borderBottom: '1px solid var(--n4n-border)' }}>{formatCrore(c.indiaNet)}</td>
                  <td style={{ padding: '10px 12px', borderBottom: '1px solid var(--n4n-border)' }}>{formatCrore(c.indiaGross)}</td>
                  <td style={{ padding: '10px 12px', borderBottom: '1px solid var(--n4n-border)' }}>{formatCrore(c.overseas)}</td>
                  <td style={{ padding: '10px 12px', borderBottom: '1px solid var(--n4n-border)' }}>{formatCrore(c.worldwideGross)}</td>
                  <td style={{ padding: '10px 12px', borderBottom: '1px solid var(--n4n-border)', fontWeight: 700, color: 'var(--n4n-orange)' }}>{formatCrore(c.totalCollection)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
