import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { articlesApi } from '../api/articles';
import ArticleCard from '../components/article/ArticleCard';
import Pagination from '../components/common/Pagination';
import { Loading, ErrorState, Empty } from '../components/common/StateViews';
import Seo from '../components/common/Seo';

const TITLES: Record<string, string> = {
  'telugu-updates': 'Telugu Updates', trending: 'Trending', gossips: 'Gossips',
  reviews: 'Movie Reviews', special: 'Special Articles', trp: 'TRP', records: 'Records', ott: 'OTT',
};

export default function CategoryPage() {
  const { slug = '' } = useParams();
  const [page, setPage] = useState(1);
  const title = TITLES[slug] ?? slug.replace(/-/g, ' ');

  const { data, isLoading, isError } = useQuery({
    queryKey: ['category', slug, page],
    queryFn: () => articlesApi.getPublished({ categorySlug: slug, page, pageSize: 12 }),
  });

  return (
    <div className="container">
      <Seo title={title} description={`Latest ${title} from Narendra4News.`} />
      <h1 className="section-title" style={{ textTransform: 'capitalize' }}>{title}</h1>
      {isLoading && <Loading />}
      {isError && <ErrorState />}
      {data && data.items.length === 0 && <Empty message={`No ${title} articles published yet.`} />}
      {data && data.items.length > 0 && (
        <>
          <div className="n4n-grid-3">
            {data.items.map((a) => <ArticleCard key={a.id} article={a} />)}
          </div>
          <Pagination page={data.page} totalPages={data.totalPages} onChange={setPage} />
        </>
      )}
    </div>
  );
}
