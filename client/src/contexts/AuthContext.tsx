import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';

// Decodes standard ASP.NET Core JWT payload without heavy dependencies
function parseJwt(token: string) {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch {
    return null;
  }
}

interface AuthState {
  token: string | null;
  username: string | null;
  isAdmin: boolean;
}

interface AuthContextType extends AuthState {
  login: (token: string, username: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [authState, setAuthState] = useState<AuthState>(() => {
    const savedToken = localStorage.getItem('token');
    const savedUsername = localStorage.getItem('username');
    let isAdmin = false;

    if (savedToken) {
      const payload = parseJwt(savedToken);
      // ASP.NET Core default role claim URI
      const roleClaim =
        payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
        payload?.role;
      isAdmin = roleClaim === 'Admin';
    }

    return {
      token: savedToken,
      username: savedUsername,
      isAdmin,
    };
  });

  const login = (token: string, username: string) => {
    localStorage.setItem('token', token);
    localStorage.setItem('username', username);

    const payload = parseJwt(token);
    const roleClaim =
      payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      payload?.role;

    setAuthState({
      token,
      username,
      isAdmin: roleClaim === 'Admin',
    });
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    setAuthState({
      token: null,
      username: null,
      isAdmin: false,
    });
  };

  useEffect(() => {
    const handleStorageChange = () => {
      const token = localStorage.getItem('token');
      if (!token && authState.token) {
        logout();
      }
    };
    window.addEventListener('storage', handleStorageChange);
    // Add custom event for apiFetch interceptor to clear auth
    window.addEventListener('api:unauthorized', logout);

    return () => {
      window.removeEventListener('storage', handleStorageChange);
      window.removeEventListener('api:unauthorized', logout);
    };
  }, [authState.token]);

  return (
    <AuthContext.Provider value={{ ...authState, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
