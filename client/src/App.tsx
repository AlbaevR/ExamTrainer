import { BrowserRouter, Routes, Route, Link, useNavigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { SubjectsPage } from './pages/SubjectsPage';
import { ExamPage } from './pages/ExamPage';
import { ResultPage } from './pages/ResultPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { AdminQuestionsPage } from './pages/AdminQuestionsPage';
import './App.css';

function HeaderControls() {
  const { username, logout } = useAuth();
  const navigate = useNavigate();

  return (
    <div className="header-controls">
      {username ? (
        <>
          <span className="welcome-text">Hi, {username}</span>
          <button className="btn btn-secondary btn-sm" onClick={() => { logout(); navigate('/login'); }}>Logout</button>
        </>
      ) : (
        <Link to="/login" className="btn btn-primary btn-sm">Login</Link>
      )}
    </div>
  );
}

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <div className="app">
          <header className="app-header">
            <Link to="/" className="app-logo">
              📝 Exam Trainer
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

export default App;
