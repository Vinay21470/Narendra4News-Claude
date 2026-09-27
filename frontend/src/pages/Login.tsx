import { useState, type FormEvent } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import Seo from '../components/common/Seo';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError('');
    try {
      const user = await login(email, password);
      navigate(user.roles.includes('Admin') ? '/admin' : '/');
    } catch {
      setError('Invalid email or password.');
    }
  }

  return (
    <div className="container" style={{ maxWidth: 400, padding: '60px 16px' }}>
      <Seo title="Login" />
      <h1 style={{ marginBottom: 20 }}>Login</h1>
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
        <input type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required
          style={{ padding: 10, border: '1px solid var(--n4n-border)', borderRadius: 6 }} />
        <input type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} required
          style={{ padding: 10, border: '1px solid var(--n4n-border)', borderRadius: 6 }} />
        {error && <div className="error-state" style={{ padding: 0 }}>{error}</div>}
        <button className="btn-primary" type="submit">Login</button>
      </form>
      <p style={{ marginTop: 16, fontSize: 13 }}>
        No account? <Link to="/register" style={{ color: 'var(--n4n-orange)' }}>Register</Link>
      </p>
    </div>
  );
}
