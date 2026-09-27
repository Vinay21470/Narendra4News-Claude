import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Navbar from './Navbar';
import SearchBar from '../common/SearchBar';
import { useAuth } from '../../hooks/useAuth';
import styles from './Header.module.css';

export default function Header() {
  const [menuOpen, setMenuOpen] = useState(false);
  const [searchOpen, setSearchOpen] = useState(false);
  const { isAuthenticated, isAdmin, user, logout } = useAuth();
  const navigate = useNavigate();

  return (
    <header className={styles.header}>
      <div className="container">
        <div className={styles.topRow}>
          <button className={styles.hamburger} onClick={() => setMenuOpen((v) => !v)} aria-label="Menu">
            ☰
          </button>

          <Link to="/" className={styles.logo}>
            NARENDRA<span>4</span>NEWS
          </Link>

          <div className={styles.actions}>
            <button className={styles.iconBtn} onClick={() => setSearchOpen((v) => !v)} aria-label="Search">
              🔍
            </button>
            {isAuthenticated ? (
              <>
                {isAdmin && (
                  <button className="btn-outline" onClick={() => navigate('/admin')}>
                    Admin
                  </button>
                )}
                <button className={styles.iconBtn} onClick={logout} title={user?.displayName}>
                  👤 {user?.displayName?.split(' ')[0]}
                </button>
              </>
            ) : (
              <button className={styles.iconBtn} onClick={() => navigate('/login')} aria-label="Login">
                👤
              </button>
            )}
          </div>
        </div>
        {searchOpen && <SearchBar onClose={() => setSearchOpen(false)} />}
      </div>
      <Navbar mobileOpen={menuOpen} onNavigate={() => setMenuOpen(false)} />
    </header>
  );
}
