import { useQuery } from '@tanstack/react-query';
import { articlesApi } from '../api/articles';
import { moviesApi } from '../api/movies';
import HeroArticle from '../components/article/HeroArticle';
import TrendingCard from '../components/article/TrendingCard';
import ArticleCard from '../components/article/ArticleCard';
import CategorySection from '../components/article/CategorySection';
import MovieCard from '../components/movie/MovieCard';
import BreakingNewsBar from '../components/layout/BreakingNewsBar';
import { Loading, ErrorState, Empty } from '../components/common/StateViews';
import Seo from '../components/common/Seo';
import styles from './Home.module.css';

export default function Home() {
  const featured = useQuery({ queryKey: ['home-featured'], queryFn: () => articlesApi.getFeatured(5) });
  const trending = useQuery({ queryKey: ['home-trending'], queryFn: () => articlesApi.getTrending(5) });
  const latest = useQuery({ queryKey: ['home-latest'], queryFn: () => articlesApi.getPublished({ page: 1, pageSize: 9 }) });
  const movies = useQuery({ queryKey: ['home-movies'], queryFn: () => moviesApi.getAll(1, 6) });

  const heroArticle = featured.data?.[0];
  const sideArticles = featured.data?.slice(1, 5) ?? [];

  return (
    <>
      <Seo
        title="Telugu Movie News, Box Office Collections & Entertainment"
        description="Narendra4News brings you the latest Telugu movie news, box office collections, reviews, gossips and entertainment updates."
      />
      <BreakingNewsBar />
      <div className="container">
        {/* Hero section */}
        {featured.isLoading && <Loading label="Loading top stories…" />}
        {featured.isError && <ErrorState />}
        {heroArticle && (
          <div className={styles.heroGrid}>
            <HeroArticle article={heroArticle} />
            <div className={styles.trendingList}>
              <p className={styles.trendingHeading}>Trending Now</p>
              {sideArticles.map((a) => (
                <TrendingCard key={a.id} article={a} />
              ))}
            </div>
          </div>
        )}

        {/* Latest movie news */}
        <section>
          <h2 className="section-title">Latest Movie News</h2>
          {latest.isLoading && <Loading />}
          {latest.isError && <ErrorState />}
          {latest.data && latest.data.items.length === 0 && <Empty />}
          {latest.data && (
            <div className="n4n-grid-3">
              {latest.data.items.map((a) => (
                <ArticleCard key={a.id} article={a} />
              ))}
            </div>
          )}
        </section>

        {/* Trending news */}
        <section>
          <h2 className="section-title">Trending News</h2>
          {trending.isLoading && <Loading />}
          {trending.data && (
            <div className="n4n-grid-3">
              {trending.data.map((a) => (
                <ArticleCard key={a.id} article={a} />
              ))}
            </div>
          )}
        </section>

        <CategorySection title="Box Office" categorySlug="box-office" count={3} />
        <CategorySection title="Movie Reviews" categorySlug="reviews" count={3} />
        <CategorySection title="Special Articles" categorySlug="special" count={3} />

        {/* Movies */}
        <section>
          <h2 className="section-title">Movies</h2>
          {movies.isLoading && <Loading />}
          {movies.data && movies.data.items.length === 0 && <Empty />}
          {movies.data && (
            <div className="n4n-grid-3" style={{ gridTemplateColumns: 'repeat(6, 1fr)' }}>
              {movies.data.items.map((m) => (
                <MovieCard key={m.id} movie={m} />
              ))}
            </div>
          )}
        </section>
      </div>
    </>
  );
}
