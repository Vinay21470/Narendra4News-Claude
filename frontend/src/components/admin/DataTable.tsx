import type { ReactNode } from 'react';

interface Column<T> {
  header: string;
  render: (row: T) => ReactNode;
  width?: string;
}

export default function DataTable<T>({ columns, rows, keyFn }: { columns: Column<T>[]; rows: T[]; keyFn: (row: T) => string | number }) {
  return (
    <div className="card" style={{ overflowX: 'auto' }}>
      <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 14 }}>
        <thead>
          <tr style={{ background: 'var(--n4n-bg-alt)', textAlign: 'left' }}>
            {columns.map((c) => (
              <th key={c.header} style={{ padding: '10px 14px', borderBottom: '2px solid var(--n4n-border)', width: c.width }}>{c.header}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={keyFn(row)}>
              {columns.map((c) => (
                <td key={c.header} style={{ padding: '10px 14px', borderBottom: '1px solid var(--n4n-border)' }}>{c.render(row)}</td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
