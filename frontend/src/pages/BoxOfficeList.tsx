import { useQuery } from '@tanstack/react-query';
import { articlesApi } from '../api/articles';
import ArticleCard from '../components/article/ArticleCard';
import { Loading, ErrorState, Empty } from '../components/common/StateViews';
import Seo from '../components/common/Seo';

export default function BoxOfficeList() {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['box-office-articles'],
    queryFn: () => articlesApi.getPublished({ categorySlug: 'box-office', page: 1, pageSize: 24 }),
  });

  return (
    <div className="container">
      <Seo title="Box Office Collections" description="Latest Telugu movie box office collection reports." />
      <h1 className="section-title">Box Office</h1>
      {isLoading && <Loading />}
      {isError && <ErrorState />}
      {data && data.items.length === 0 && <Empty message="No box office reports published yet." />}
      {data && data.items.length > 0 && (
        <div className="n4n-grid-3">
          {data.items.map((a) => <ArticleCard key={a.id} article={a} />)}
        </div>
      )}
    </div>
  );
}
