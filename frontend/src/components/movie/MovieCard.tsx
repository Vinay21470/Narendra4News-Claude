import { Link } from 'react-router-dom';
import type { MovieListItem } from '../../types';
import styles from './MovieCard.module.css';

export default function MovieCard({ movie }: { movie: MovieListItem }) {
  return (
    <Link to={`/movie/${movie.slug}`} className={`card ${styles.card}`}>
      <div className={styles.poster}>
        {movie.posterUrl && <img src={movie.posterUrl} alt={movie.movieName} loading="lazy" />}
      </div>
      <div className={styles.name}>{movie.movieName}</div>
      <div className={styles.meta}>{movie.genre ?? ''}</div>
    </Link>
  );
}
