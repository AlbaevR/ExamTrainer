import { useState, useEffect } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useLanguage } from '../contexts/LanguageContext';
import { useT } from '../i18n/strings';
import type { ReactNode } from 'react';
import './Layout.css';

interface LayoutProps {
  children: ReactNode;
}

function LanguagePills() {
  const { lang, setLang } = useLanguage();
  return (
    <div className="lang-pills" role="group" aria-label="Select language">
      {(['ky', 'ru', 'en'] as const).map((code) => (
        <button
          key={code}
          onClick={() => setLang(code)}
          className={`lang-pill ${lang === code ? 'active' : ''}`}
          aria-pressed={lang === code}
        >
          {code.toUpperCase()}
        </button>
      ))}
    </div>
  );
}

export function Layout({ children }: LayoutProps) {
  const { username, isAdmin, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const t = useT();
  const [menuOpen, setMenuOpen] = useState(false);

  // Close mobile menu on route change
  useEffect(() => {
    setMenuOpen(false);
  }, [location.pathname]);

  // Prevent body scroll when menu is open
  useEffect(() => {
    document.body.style.overflow = menuOpen ? 'hidden' : '';
    return () => { document.body.style.overflow = ''; };
  }, [menuOpen]);

  const isAdminRoute = location.pathname.startsWith('/admin');

  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <div className={`layout ${isAdmin ? 'layout--admin' : ''}`}>
      {/* ===== HEADER ===== */}
      <header className="site-header">
        <div className="header-inner">
          {/* Logo */}
          <Link to="/" className="header-logo" onClick={() => setMenuOpen(false)}>
            <span className="header-logo-text">ExamTrainer</span>
            {isAdmin && (
              <span className="staff-badge">Staff</span>
            )}
          </Link>

          {/* Desktop Nav */}
          <nav className="header-nav" aria-label="Main navigation">
            <LanguagePills />
            {username ? (
              <>
                <Link to="/history" className="nav-link">
                  {t.myHistory}
                </Link>
                <div className="user-chip">
                  <span className="user-chip-avatar">
                    {username.charAt(0).toUpperCase()}
                  </span>
                  <span className="user-chip-name">{username}</span>
                </div>
                <button
                  id="logout-btn"
                  className="btn btn-ghost btn-sm"
                  onClick={handleLogout}
                >
                  {t.logout}
                </button>
              </>
            ) : (
              <Link to="/login" className="btn btn-primary btn-sm" id="login-header-btn">
                {t.login}
              </Link>
            )}
          </nav>

          {/* Hamburger */}
          <button
            className={`hamburger ${menuOpen ? 'open' : ''}`}
            onClick={() => setMenuOpen((v) => !v)}
            aria-label="Toggle menu"
            aria-expanded={menuOpen}
          >
            <span /><span /><span />
          </button>
        </div>
      </header>

      {/* ===== MOBILE DRAWER ===== */}
      {menuOpen && (
        <div className="mobile-overlay" onClick={() => setMenuOpen(false)} aria-hidden="true" />
      )}
      <nav className={`mobile-drawer ${menuOpen ? 'open' : ''}`} aria-label="Mobile navigation">
        <div className="mobile-drawer-header">
          <span className="mobile-drawer-title">
            {isAdmin && <span className="staff-badge">Staff</span>}
            ExamTrainer
          </span>
          <button className="mobile-close" onClick={() => setMenuOpen(false)} aria-label="Close menu">✕</button>
        </div>
        <div className="mobile-drawer-body">
          <LanguagePills />
          {username ? (
            <>
              <div className="mobile-user">
                <span className="user-chip-avatar">{username.charAt(0).toUpperCase()}</span>
                <span>{username}</span>
              </div>
              <Link to="/" className="mobile-nav-link">{t.chooseSubject}</Link>
              <Link to="/history" className="mobile-nav-link">{t.myHistory}</Link>
              <button className="btn btn-ghost mobile-logout" onClick={handleLogout}>
                {t.logout}
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="btn btn-primary mobile-nav-link">{t.login}</Link>
            </>
          )}
        </div>
      </nav>

      {/* ===== MAIN ===== */}
      <main className={`site-main ${isAdminRoute ? 'site-main--admin' : ''}`}>
        {children}
      </main>

      {/* ===== FOOTER ===== */}
      <footer className="site-footer">
        <div className="footer-inner">
          <div className="footer-brand">
            <span className="footer-logo">ExamTrainer</span>
            <p className="footer-tagline">
              {t.chooseSubjectSubtitle}
            </p>
          </div>
          <div className="footer-links">
            <Link to="/" className="footer-link">{t.chooseSubject}</Link>
            <Link to="/history" className="footer-link">{t.myHistory}</Link>
            <a href="mailto:admin@examtrainer.app" className="footer-link">
              Contact Admin
            </a>
          </div>
          <p className="footer-copy">
            © {new Date().getFullYear()} ExamTrainer. All rights reserved.
          </p>
        </div>
      </footer>
    </div>
  );
}
