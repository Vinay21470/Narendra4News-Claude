import { useCallback, useEffect, useState } from 'react';
import type { CurrentUser } from '../types';
import { authApi } from '../api/auth';

// Lightweight auth store backed by localStorage. This is fine for a
// per-viewer JWT session; it is NOT where article/movie content lives -
// that always comes from the API (spec section 5/33).
export function useAuth() {
  const [user, setUser] = useState<CurrentUser | null>(() => {
    const raw = localStorage.getItem('n4n_user');
    return raw ? (JSON.parse(raw) as CurrentUser) : null;
  });

  useEffect(() => {
    const token = localStorage.getItem('n4n_token');
    if (token && !user) {
      authApi
        .me()
        .then((u) => {
          setUser(u);
          localStorage.setItem('n4n_user', JSON.stringify(u));
        })
        .catch(() => {
          localStorage.removeItem('n4n_token');
          localStorage.removeItem('n4n_user');
        });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const login = useCallback(async (email: string, password: string) => {
    const res = await authApi.login(email, password);
    localStorage.setItem('n4n_token', res.token);
    const currentUser: CurrentUser = { userId: res.userId, email: res.email, displayName: res.displayName, roles: res.roles };
    localStorage.setItem('n4n_user', JSON.stringify(currentUser));
    setUser(currentUser);
    return currentUser;
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('n4n_token');
    localStorage.removeItem('n4n_user');
    setUser(null);
  }, []);

  const isAdmin = user?.roles.includes('Admin') ?? false;

  return { user, login, logout, isAdmin, isAuthenticated: !!user };
}
