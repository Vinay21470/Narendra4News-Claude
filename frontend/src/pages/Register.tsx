import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { authApi } from '../api/auth';
import Seo from '../components/common/Seo';

export default function Register() {
  const [displayName, setDisplayName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError('');
    try {
      // Register then log in immediately for a smooth flow.
      await authApi.register(email, password, displayName);
      await login(email, password);
      navigate('/');
    } catch {
      setError('Could not create account. Try a different email or a stronger password.');
    }
  }

  return (
    <div className="container" style={{ maxWidth: 400, padding: '60px 16px' }}>
      <Seo title="Register" />
      <h1 style={{ marginBottom: 20 }}>Create Account</h1>
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
        <input placeholder="Display name" value={displayName} onChange={(e) => setDisplayName(e.target.value)} required
          style={{ padding: 10, border: '1px solid var(--n4n-border)', borderRadius: 6 }} />
        <input type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required
          style={{ padding: 10, border: '1px solid var(--n4n-border)', borderRadius: 6 }} />
        <input type="password" placeholder="Password (min 8 characters)" value={password} onChange={(e) => setPassword(e.target.value)} required
          style={{ padding: 10, border: '1px solid var(--n4n-border)', borderRadius: 6 }} />
        {error && <div className="error-state" style={{ padding: 0 }}>{error}</div>}
        <button className="btn-primary" type="submit">Register</button>
      </form>
    </div>
  );
}
