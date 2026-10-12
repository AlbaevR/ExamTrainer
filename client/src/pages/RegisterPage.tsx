import { useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { register as registerApi, ApiError } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import { ErrorMessage } from '../components/ErrorMessage';
import { useT } from '../i18n/strings';
import './RegisterPage.css';

const MIN_PASSWORD_LENGTH = 6; // must match options.Password.RequiredLength in api/Program.cs

export function RegisterPage() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const t = useT();
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);

    if (password.length < MIN_PASSWORD_LENGTH) {
      setError(t.errPasswordTooShort);
      return;
    }

    setLoading(true);

    try {
      const result = await registerApi({ username, email, password });
      login(result.token, result.username);
      navigate('/');
    } catch (err: unknown) {
      setError(registerErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }

  // Maps ASP.NET Identity error codes from the API to localized messages
  function registerErrorMessage(err: unknown): string {
    const codes = err instanceof ApiError ? err.codes : [];
    const messages: Record<string, string> = {
      PasswordTooShort: t.errPasswordTooShort,
      DuplicateEmail: t.errDuplicateEmail,
      InvalidUserName: t.errInvalidUserName,
      InvalidEmail: t.errInvalidEmail,
      Email: t.errInvalidEmail,
    };
    const known = codes.map((c) => messages[c]).filter(Boolean);
    return known.length > 0 ? known.join(' ') : t.errRegisterFailed;
  }

  return (
    <div className="login-page">
      <div className="login-card">
        <h1>{t.createAccount}</h1>
        <p className="login-subtitle">{t.registerAccount}</p>

        {error && <ErrorMessage message={error} />}

        <form onSubmit={handleSubmit} className="login-form">
          <div className="form-group">
            <label htmlFor="username">{t.username}</label>
            <input
              id="username"
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              disabled={loading}
              placeholder={t.username}
            />
          </div>
          <div className="form-group">
            <label htmlFor="email">{t.email}</label>
            <input
              id="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              disabled={loading}
              placeholder={t.email}
            />
          </div>
          <div className="form-group">
            <label htmlFor="password">{t.password}</label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              disabled={loading}
              placeholder={t.password}
              aria-describedby="password-hint"
            />
            <small
              id="password-hint"
              className={`form-hint ${password.length > 0 && password.length < MIN_PASSWORD_LENGTH ? 'form-hint--invalid' : ''}`}
            >
              {t.passwordHint}
            </small>
          </div>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? t.registering : t.registerBtn}
          </button>
        </form>

        <p className="login-switch">
          {t.alreadyHaveAccount} <Link to="/login">{t.login}</Link>
        </p>
      </div>
    </div>
  );
}
