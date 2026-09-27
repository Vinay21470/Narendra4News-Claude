import type { ReactNode } from 'react';
import { NavLink, Link } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import styles from './AdminLayout.module.css';

const LINKS = [
  { to: '/admin', label: 'Dashboard', end: true },
  { to: '/admin/articles', label: 'Articles' },
  { to: '/admin/movies', label: 'Movies' },
  { to: '/admin/categories', label: 'Categories' },
  { to: '/admin/media', label: 'Media' },
];

export default function AdminLayout({ children }: { children: ReactNode }) {
  const { user, logout } = useAuth();
  return (
    <div className={styles.shell}>
      <aside className={styles.sidebar}>
        <h2>NARENDRA4NEWS Admin</h2>
        {LINKS.map((l) => (
          <NavLink key={l.to} to={l.to} end={l.end} className={({ isActive }) => (isActive ? styles.active : '')}>
            {l.label}
          </NavLink>
        ))}
      </aside>
      <main className={styles.main}>
        <div className={styles.topbar}>
          <Link to="/" style={{ fontSize: 13, color: 'var(--n4n-orange)' }}>← View Site</Link>
          <div style={{ fontSize: 13 }}>
            {user?.displayName} <button className="btn-outline" style={{ marginLeft: 10 }} onClick={logout}>Logout</button>
          </div>
        </div>
        {children}
      </main>
    </div>
  );
}
