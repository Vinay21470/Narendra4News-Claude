import type { MovieCollectionRecord } from '../../types';
import { formatCrore } from '../../utils/format';
import styles from './BoxOfficeCard.module.css';

export default function BoxOfficeCard({ movieName, collection }: { movieName: string; collection: MovieCollectionRecord }) {
  const rows: [string, number | undefined][] = [
    ['Opening Day', collection.openingDay],
    ['India Net', collection.indiaNet],
    ['India Gross', collection.indiaGross],
    ['Overseas', collection.overseas],
    ['Weekend', collection.weekendCollection],
    ['Worldwide', collection.worldwideGross],
    ['Total', collection.totalCollection],
  ].filter(([, v]) => v !== undefined && v !== null) as [string, number][];

  return (
    <div className={`card ${styles.card}`}>
      <h4 className={styles.title}>
        {movieName} — {collection.label ?? `Day ${collection.dayNumber}`}
      </h4>
      <table className={styles.table}>
        <tbody>
          {rows.map(([label, value]) => (
            <tr key={label}>
              <td>{label}</td>
              <td>{formatCrore(value)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
