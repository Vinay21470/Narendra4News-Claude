import { Link } from 'react-router-dom';
import type { ArticleListItem } from '../../types';
import styles from './TrendingCard.module.css';

export default function TrendingCard({ article, rank }: { article: ArticleListItem; rank?: number }) {
  return (
    <Link to={`/article/${article.slug}`} className={styles.item}>
      {rank !== undefined && <strong style={{ color: 'var(--n4n-orange)', fontSize: 18 }}>{rank}</strong>}
      <div className={styles.thumb}>
        {article.featuredImageUrl && <img src={article.featuredImageUrl} alt={article.title} loading="lazy" />}
      </div>
      <div className={styles.title}>{article.title}</div>
    </Link>
  );
}
