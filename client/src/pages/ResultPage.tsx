import { useLocation, useNavigate } from 'react-router-dom';
import type { ExamResult } from '../types';
import { useT } from '../i18n/strings';
import { useAuth } from '../contexts/AuthContext';
import './ResultPage.css';

interface ResultState {
  result: ExamResult;
  subjectName: string;
  subjectId: number;
}

export function ResultPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const state = location.state as ResultState | null;
  const t = useT();
  const { token } = useAuth();
  const isLoggedIn = !!token;

  if (!state) {
    return (
      <div className="result-page">
        <p>{t.noResultData}</p>
        <button className="btn btn-primary" onClick={() => navigate('/')}>
          {t.goToSubjects}
        </button>
      </div>
    );
  }

  const { result, subjectName, subjectId } = state;
  const isPerfect = result.percentage === 100;

  return (
    <div className="result-page">
      <div className="result-card">
        <h1>{t.examResults}</h1>
        {subjectName && <p className="result-subject">{subjectName}</p>}

        {isPerfect && (
          <div className="result-congrats">
            {t.congrats}
          </div>
        )}

        <div className="result-stats">
          <div className="stat">
            <span className="stat-value">{result.score}</span>
            <span className="stat-label">{t.correct}</span>
          </div>
          <div className="stat">
            <span className="stat-value">{result.totalQuestions}</span>
            <span className="stat-label">{t.total}</span>
          </div>
          <div className="stat">
            <span className="stat-value">{result.percentage}%</span>
            <span className="stat-label">{t.score}</span>
          </div>
        </div>

        <div className="result-actions">
          {subjectId && (
            <button className="btn btn-primary" onClick={() => navigate(`/exam/${subjectId}`)}>
              {t.retakeExam}
            </button>
          )}
          {isLoggedIn && result.attemptId && (
            <button className="btn btn-secondary" onClick={() => navigate(`/history/${result.attemptId}/feedback`)}>
              {t.getAiFeedback}
            </button>
          )}
          {isLoggedIn && (
            <button className="btn btn-secondary" onClick={() => navigate('/history')}>
              {t.myAttempts}
            </button>
          )}
          <button className="btn btn-ghost" onClick={() => navigate('/')}>
            {t.backToSubjects}
          </button>
        </div>
      </div>
    </div>
  );
}
