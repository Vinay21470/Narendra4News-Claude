import { Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { articlesApi } from '../../api/articles';
import styles from './BreakingNewsBar.module.css';

// Latest headlines, pulled live from the API - never hardcoded (spec #4).
export default function BreakingNewsBar() {
  const { data } = useQuery({
    queryKey: ['breaking-news'],
    queryFn: () => articlesApi.getTrending(6),
  });

  if (!data || data.length === 0) return null;

  return (
    <div className={styles.bar}>
      <div className="container">
        <div className={styles.inner}>
          <span className={styles.label}>Breaking</span>
          <div className={styles.scroller}>
            {data.map((a) => (
              <Link key={a.id} to={`/article/${a.slug}`} className={styles.item}>
                {a.title}
              </Link>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
