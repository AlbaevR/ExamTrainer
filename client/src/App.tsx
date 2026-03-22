import { BrowserRouter, Routes, Route, Link, useNavigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { LanguageProvider, useLanguage } from './contexts/LanguageContext';
import { useT } from './i18n/strings';
import { SubjectsPage } from './pages/SubjectsPage';
import { ExamPage } from './pages/ExamPage';
import { ResultPage } from './pages/ResultPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { AdminQuestionsPage } from './pages/AdminQuestionsPage';
import './App.css';

function LanguageSwitcher() {
  const { lang, setLang } = useLanguage();
  return (
    <select 
      value={lang} 
      onChange={(e) => setLang(e.target.value as 'ru' | 'ky' | 'en')}
      className="lang-select"
      title="Select Language"
    >
      <option value="ru">Русский (RU)</option>
      <option value="ky">Кыргызча (KY)</option>
      <option value="en">English (EN)</option>
    </select>
  );
}

function HeaderControls() {
  const { username, logout } = useAuth();
  const navigate = useNavigate();
  const t = useT();

  return (
    <div className="header-controls">
      <LanguageSwitcher />
      {username ? (
        <>
          <span className="welcome-text">{t.hi}, {username}</span>
          <button className="btn btn-secondary btn-sm" onClick={() => { logout(); navigate('/login'); }}>{t.logout}</button>
        </>
      ) : (
        <Link to="/login" className="btn btn-primary btn-sm">{t.login}</Link>
      )}
    </div>
  );
}

function App() {
  const t = useT();
  return (
    <AuthProvider>
      <BrowserRouter>
        <div className="app">
          <header className="app-header">
            <Link to="/" className="app-logo">
              {t.appName}
            </Link>
            <HeaderControls />
          </header>
          <main className="app-main">
            <Routes>
              <Route path="/" element={<SubjectsPage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/exam/:subjectId" element={<ExamPage />} />
              <Route path="/result" element={<ResultPage />} />
              <Route path="/admin/subjects/:subjectId/questions" element={<AdminQuestionsPage />} />
            </Routes>
          </main>
        </div>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default function Root() {
  return (
    <LanguageProvider>
      <App />
    </LanguageProvider>
  );
}
