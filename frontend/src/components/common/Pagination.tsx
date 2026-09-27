import styles from './Pagination.module.css';

interface Props {
  page: number;
  totalPages: number;
  onChange: (page: number) => void;
}

export default function Pagination({ page, totalPages, onChange }: Props) {
  if (totalPages <= 1) return null;
  const pages = Array.from({ length: totalPages }, (_, i) => i + 1).filter(
    (p) => p === 1 || p === totalPages || Math.abs(p - page) <= 1
  );

  return (
    <div className={styles.wrap}>
      <button className={styles.btn} disabled={page <= 1} onClick={() => onChange(page - 1)}>
        Prev
      </button>
      {pages.map((p, i) => (
        <span key={p}>
          {i > 0 && pages[i - 1] !== p - 1 && <span style={{ padding: '0 4px' }}>…</span>}
          <button className={`${styles.btn} ${p === page ? styles.active : ''}`} onClick={() => onChange(p)}>
            {p}
          </button>
        </span>
      ))}
      <button className={styles.btn} disabled={page >= totalPages} onClick={() => onChange(page + 1)}>
        Next
      </button>
    </div>
  );
}
