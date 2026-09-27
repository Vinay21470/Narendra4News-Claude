import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './SearchBar.module.css';

export default function SearchBar({ onClose }: { onClose?: () => void }) {
  const [q, setQ] = useState('');
  const navigate = useNavigate();

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (!q.trim()) return;
    navigate(`/search?q=${encodeURIComponent(q.trim())}`);
    onClose?.();
  }

  return (
    <form className={styles.wrap} onSubmit={handleSubmit}>
      <input
        className={styles.input}
        placeholder="Search movies, articles, box office…"
        value={q}
        onChange={(e) => setQ(e.target.value)}
        autoFocus
      />
      <button className="btn-primary" type="submit">Search</button>
    </form>
  );
}
