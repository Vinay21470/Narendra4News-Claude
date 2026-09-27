import { Link } from 'react-router-dom';
import type { ArticleListItem } from '../../types';
import { formatDate, formatViews } from '../../utils/format';
import styles from './ArticleCard.module.css';

export default function ArticleCard({ article }: { article: ArticleListItem }) {
  return (
    <Link to={`/article/${article.slug}`} className={`card ${styles.card}`}>
      <div className={styles.imgWrap}>
        {article.featuredImageUrl ? (
          <img src={article.featuredImageUrl} alt={article.title} loading="lazy" />
        ) : (
          <div style={{ width: '100%', height: '100%', background: '#f0f0f0' }} />
        )}
      </div>
      <div className={styles.body}>
        <span className="badge">{article.categoryName}</span>
        <h3 className={styles.title}>{article.title}</h3>
        <p className={styles.desc}>{article.shortDescription}</p>
        <div className={styles.meta}>
          <span>{formatDate(article.publishedDate)}</span>
          <span>{formatViews(article.views)} views</span>
        </div>
      </div>
    </Link>
  );
}
