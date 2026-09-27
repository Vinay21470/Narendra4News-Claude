import { NavLink } from 'react-router-dom';
import styles from './Navbar.module.css';

const NAV_ITEMS = [
  { label: 'Home', to: '/' },
  { label: 'Telugu Updates', to: '/category/telugu-updates' },
  { label: 'Trending', to: '/category/trending' },
  { label: 'Box Office', to: '/box-office' },
  { label: 'Gossips', to: '/category/gossips' },
  { label: 'Reviews', to: '/category/reviews' },
  { label: 'Special', to: '/category/special' },
  { label: 'TRP', to: '/category/trp' },
  { label: 'Records', to: '/category/records' },
];

export default function Navbar({ mobileOpen, onNavigate }: { mobileOpen: boolean; onNavigate: () => void }) {
  return (
    <nav className={`${styles.nav} ${mobileOpen ? styles.open : ''}`}>
      <div className="container">
        <ul className={styles.list}>
          {NAV_ITEMS.map((item) => (
            <li key={item.to}>
              <NavLink to={item.to} onClick={onNavigate} className={({ isActive }) => (isActive ? styles.active : '')}>
                {item.label}
              </NavLink>
            </li>
          ))}
        </ul>
      </div>
    </nav>
  );
}
