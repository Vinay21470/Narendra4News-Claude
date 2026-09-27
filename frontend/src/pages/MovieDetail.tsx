import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { moviesApi } from '../api/movies';
import { formatDate } from '../utils/format';
import { Loading, ErrorState } from '../components/common/StateViews';
import Seo from '../components/common/Seo';
import BoxOfficeCard from '../components/movie/BoxOfficeCard';
import styles from './MovieDetail.module.css';

export default function MovieDetail() {
  const { slug = '' } = useParams();
  const { data: movie, isLoading, isError } = useQuery({
    queryKey: ['movie', slug],
    queryFn: () => moviesApi.getBySlug(slug),
  });

  if (isLoading) return <div className="container"><Loading label="Loading movie…" /></div>;
  if (isError || !movie) return <div className="container"><ErrorState message="Movie not found." /></div>;

  return (
    <div className="container">
      <Seo
        title={movie.movieName}
        description={movie.description}
        image={movie.posterUrl}
        jsonLd={{
          '@context': 'https://schema.org', '@type': 'Movie', name: movie.movieName,
          image: movie.posterUrl, datePublished: movie.releaseDate, director: movie.director ? { '@type': 'Person', name: movie.director } : undefined,
        }}
      />

      <div className={styles.heroBanner}>
        <div className={styles.poster}>
          {movie.posterUrl && <img src={movie.posterUrl} alt={movie.movieName} />}
        </div>
        <div className={styles.info}>
          <h1 style={{ fontSize: 26, marginBottom: 4 }}>{movie.movieName}</h1>
          {movie.description && <p style={{ color: '#555' }}>{movie.description}</p>}
          <dl>
            {movie.releaseDate && (<><dt>Release Date</dt><dd>{formatDate(movie.releaseDate)}</dd></>)}
            {movie.hero && (<><dt>Hero</dt><dd>{movie.hero}</dd></>)}
            {movie.director && (<><dt>Director</dt><dd>{movie.director}</dd></>)}
            {movie.producer && (<><dt>Producer</dt><dd>{movie.producer}</dd></>)}
            {movie.productionHouse && (<><dt>Production</dt><dd>{movie.productionHouse}</dd></>)}
            {movie.genre && (<><dt>Genre</dt><dd>{movie.genre}</dd></>)}
            {movie.language && (<><dt>Language</dt><dd>{movie.language}</dd></>)}
            {movie.runtimeMinutes && (<><dt>Runtime</dt><dd>{movie.runtimeMinutes} min</dd></>)}
            {movie.certification && (<><dt>Certification</dt><dd>{movie.certification}</dd></>)}
          </dl>
          <p style={{ marginTop: 16 }}>
            <Link to={`/movie/${movie.slug}/box-office`} className="btn-primary">View Full Box Office History</Link>
          </p>
        </div>
      </div>

      {movie.collections.length > 0 && (
        <section>
          <h2 className="section-title">Box Office Collections</h2>
          <div className="n4n-grid-3">
            {movie.collections.map((c) => (
              <BoxOfficeCard key={c.id} movieName={movie.movieName} collection={c} />
            ))}
          </div>
        </section>
      )}
    </div>
  );
}
