import DOMPurify from 'dompurify';
import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { articlesApi } from '../api/articles';
import { formatDate } from '../utils/format';
import { Loading, ErrorState } from '../components/common/StateViews';
import Seo from '../components/common/Seo';
import SocialShare from '../components/common/SocialShare';
import ArticleCard from '../components/article/ArticleCard';

export default function ArticleDetail() {
  const { slug = '' } = useParams();
  const { data: article, isLoading, isError } = useQuery({
    queryKey: ['article', slug],
    queryFn: () => articlesApi.getBySlug(slug),
  });

  if (isLoading) return <div className="container"><Loading label="Loading article…" /></div>;
  if (isError || !article) return <div className="container"><ErrorState message="Article not found." /></div>;

  const url = typeof window !== 'undefined' ? window.location.href : '';

  return (
    <div className="container">
      <Seo
        title={article.seoTitle || article.title}
        description={article.seoDescription || article.shortDescription}
        keywords={article.seoKeywords}
        image={article.featuredImageUrl}
        type="article"
        jsonLd={{
          '@context': 'https://schema.org',
          '@type': 'NewsArticle',
          headline: article.title,
          image: article.featuredImageUrl ? [article.featuredImageUrl] : undefined,
          datePublished: article.publishedDate,
          dateModified: article.updatedDate ?? article.publishedDate,
          author: { '@type': 'Person', name: article.authorName },
          publisher: { '@type': 'Organization', name: 'Narendra4News' },
        }}
      />

      <div style={{ maxWidth: 820, margin: '0 auto', padding: '20px 0 60px' }}>
        <nav style={{ fontSize: 12, color: '#777', marginBottom: 8 }}>
          <Link to="/">Home</Link> {'>'} <Link to={`/category/${article.categorySlug}`}>{article.categoryName}</Link> {'>'} {article.title}
        </nav>

        <span className="badge">{article.categoryName}</span>
        <h1 style={{ fontSize: 28, fontWeight: 800, lineHeight: 1.3, margin: '10px 0 12px' }}>{article.title}</h1>

        <div style={{ display: 'flex', gap: 14, flexWrap: 'wrap', fontSize: 13, color: '#777', marginBottom: 16 }}>
          <span>By {article.authorName}</span>
          <span>{formatDate(article.publishedDate)}</span>
          {article.updatedDate && <span>Updated {formatDate(article.updatedDate)}</span>}
          <span>{article.views.toLocaleString()} views</span>
        </div>

        {article.featuredImageUrl && (
          <img src={article.featuredImageUrl} alt={article.title} style={{ width: '100%', borderRadius: 6, marginBottom: 20 }} />
        )}

        <div style={{ fontSize: 16, lineHeight: 1.8 }} dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(article.content) }} />

        <SocialShare url={url} title={article.title} />

        {article.movieSlug && (
          <p>
            <Link to={`/movie/${article.movieSlug}/box-office`} className="btn-outline">
              View full box office history for this movie →
            </Link>
          </p>
        )}

        {article.relatedArticles.length > 0 && (
          <section style={{ marginTop: 40 }}>
            <h2 className="section-title">Related Articles</h2>
            <div className="n4n-grid-3">
              {article.relatedArticles.map((a) => (
                <ArticleCard key={a.id} article={a} />
              ))}
            </div>
          </section>
        )}
      </div>
    </div>
  );
}
