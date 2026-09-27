import { Link } from 'react-router-dom';
import styles from './Footer.module.css';

export default function Footer() {
  const year = new Date().getFullYear();
  return (
    <footer className={styles.footer}>
      <div className="container">
        <div className={styles.grid}>
          <div>
            <h4 className={styles.logo}>NARENDRA4NEWS</h4>
            <p className={styles.about}>
              Narendra4News brings you the latest Telugu movie news, box office collections, reviews and entertainment updates.
            </p>
          </div>
          <div>
            <h5>Company</h5>
            <ul>
              <li><Link to="/about">About Narendra4News</Link></li>
              <li><Link to="/contact">Contact</Link></li>
            </ul>
          </div>
          <div>
            <h5>Legal</h5>
            <ul>
              <li><Link to="/privacy-policy">Privacy Policy</Link></li>
              <li><Link to="/terms">Terms</Link></li>
              <li><Link to="/disclaimer">Disclaimer</Link></li>
            </ul>
          </div>
          <div>
            <h5>Follow Us</h5>
            <div className={styles.social}>
              <a href="#" aria-label="Facebook">Facebook</a>
              <a href="#" aria-label="Twitter">X / Twitter</a>
              <a href="#" aria-label="Instagram">Instagram</a>
              <a href="#" aria-label="YouTube">YouTube</a>
            </div>
          </div>
        </div>
        <div className={styles.copyright}>© {year} Narendra4News. All rights reserved.</div>
      </div>
    </footer>
  );
}
