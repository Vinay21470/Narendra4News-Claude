import { Link } from 'react-router-dom';
import Seo from '../components/common/Seo';

export default function NotFound() {
  return (
    <div className="container" style={{ textAlign: 'center', padding: '80px 16px' }}>
      <Seo title="Page Not Found" />
      <h1 style={{ fontSize: 60, color: 'var(--n4n-orange)' }}>404</h1>
      <p>The page you're looking for doesn't exist.</p>
      <Link to="/" className="btn-primary" style={{ display: 'inline-block', marginTop: 12 }}>Back to Home</Link>
    </div>
  );
}
