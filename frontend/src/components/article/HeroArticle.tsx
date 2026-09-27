import { Link } from 'react-router-dom';
import type { ArticleListItem } from '../../types';
import { formatDate } from '../../utils/format';
import styles from './HeroArticle.module.css';

export default function HeroArticle({ article }: { article: ArticleListItem }) {
  return (
    <Link to={`/article/${article.slug}`} className={styles.hero}>
      {article.featuredImageUrl && <img src={article.featuredImageUrl} alt={article.title} />}
      <div className={styles.overlay}>
        <span className="badge">{article.categoryName}</span>
        <h2>{article.title}</h2>
        <p>{article.shortDescription}</p>
        <span className={styles.meta}>{formatDate(article.publishedDate)}</span>
      </div>
    </Link>
  );
}
