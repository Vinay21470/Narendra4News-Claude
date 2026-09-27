import { useQuery } from '@tanstack/react-query';
import { articlesApi } from '../../api/articles';
import ArticleCard from './ArticleCard';
import { Loading, Empty, ErrorState } from '../common/StateViews';

export default function CategorySection({ title, categorySlug, count = 6 }: { title: string; categorySlug: string; count?: number }) {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['category-section', categorySlug],
    queryFn: () => articlesApi.getPublished({ categorySlug, page: 1, pageSize: count }),
  });

  return (
    <section>
      <h2 className="section-title">{title}</h2>
      {isLoading && <Loading />}
      {isError && <ErrorState />}
      {data && data.items.length === 0 && <Empty />}
      {data && data.items.length > 0 && (
        <div className="n4n-grid-3">
          {data.items.map((a) => (
            <ArticleCard key={a.id} article={a} />
          ))}
        </div>
      )}
    </section>
  );
}
