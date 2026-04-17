import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { LanguageProvider } from './contexts/LanguageContext';
import { Layout } from './components/Layout';
import './App.css';
import { SubjectsPage } from './pages/SubjectsPage';
import { ExamPage } from './pages/ExamPage';
import { ResultPage } from './pages/ResultPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { AdminQuestionsPage } from './pages/AdminQuestionsPage';
import { HistoryPage } from './pages/HistoryPage';
import { AttemptDetailPage } from './pages/AttemptDetailPage';
import { ExamFeedbackPage } from './pages/ExamFeedbackPage';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Layout>
          <Routes>
            <Route path="/" element={<SubjectsPage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/exam/:subjectId" element={<ExamPage />} />
            <Route path="/result" element={<ResultPage />} />
            <Route path="/admin/subjects/:subjectId/questions" element={<AdminQuestionsPage />} />
            <Route path="/history" element={<HistoryPage />} />
            <Route path="/history/:id" element={<AttemptDetailPage />} />
            <Route path="/history/:id/feedback" element={<ExamFeedbackPage />} />
          </Routes>
        </Layout>
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
