import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { adminApi } from '../../api/admin';
import { Loading, ErrorState } from '../../components/common/StateViews';

function StatCard({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="card" style={{ padding: 18 }}>
      <div style={{ fontSize: 12, color: '#888', textTransform: 'uppercase', fontWeight: 700 }}>{label}</div>
      <div style={{ fontSize: 28, fontWeight: 800, marginTop: 6 }}>{value}</div>
    </div>
  );
}

export default function Dashboard() {
  const { data, isLoading, isError } = useQuery({ queryKey: ['admin-dashboard'], queryFn: adminApi.getDashboard });

  if (isLoading) return <Loading label="Loading dashboard…" />;
  if (isError || !data) return <ErrorState />;

  return (
    <div>
      <h1 style={{ marginTop: 0 }}>Dashboard</h1>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(180px, 1fr))', gap: 16 }}>
        <StatCard label="Total Articles" value={data.totalArticles} />
        <StatCard label="Published" value={data.publishedArticles} />
        <StatCard label="Drafts" value={data.draftArticles} />
        <StatCard label="Total Movies" value={data.totalMovies} />
        <StatCard label="Total Users" value={data.totalUsers} />
        <StatCard label="Total Views" value={data.totalViews.toLocaleString()} />
        <StatCard label="Today's Views" value={data.todaysViews.toLocaleString()} />
      </div>
      {data.mostViewedArticleSlug && (
        <div className="card" style={{ padding: 18, marginTop: 20 }}>
          <div style={{ fontSize: 12, color: '#888', textTransform: 'uppercase', fontWeight: 700 }}>Most Viewed Article</div>
          <Link to={`/article/${data.mostViewedArticleSlug}`} style={{ color: 'var(--n4n-orange)', fontWeight: 700 }}>
            {data.mostViewedArticleTitle}
          </Link>
        </div>
      )}
      <div style={{ marginTop: 24, display: 'flex', gap: 12 }}>
        <Link to="/admin/articles/create" className="btn-primary">+ New Article</Link>
        <Link to="/admin/movies" className="btn-outline">Manage Movies</Link>
      </div>
    </div>
  );
}
