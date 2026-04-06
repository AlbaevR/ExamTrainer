import { useNavigate } from 'react-router-dom';
import { getExamHistory } from '../services/api';
import { useApi } from '../hooks/useApi';
import { useAuth } from '../contexts/AuthContext';
import { useLanguage } from '../contexts/LanguageContext';
import { useT } from '../i18n/strings';
import { Loader } from '../components/Loader';
import { ErrorMessage } from '../components/ErrorMessage';
import type { ExamAttemptHistory } from '../types';
import './HistoryPage.css';

export function HistoryPage() {
  const navigate = useNavigate();
  const { token, username } = useAuth();
  const { lang } = useLanguage();
  const t = useT();

  // Only call the API when the user is authenticated.
  // Without this guard, apiFetch would get a 401, fire api:unauthorized,
  // and AuthContext would call logout() — clearing username even for valid sessions.
  const { data: attempts, loading, error, reload } = useApi(
    () => token ? getExamHistory(lang) : Promise.resolve<ExamAttemptHistory[]>([]),
    [lang, token],
  );

  if (!username) {
    return (
      <div className="history-page">
        <h1>{t.myHistory}</h1>
        <p className="empty-state">{t.loginToSeeHistory}</p>
      </div>
    );
  }

  if (loading) return <Loader />;
  if (error || !attempts) return <ErrorMessage message={error || 'Failed to load history'} onRetry={reload} />;

  return (
    <div className="history-page">
      <h1>{t.myHistory}</h1>

      {attempts.length === 0 ? (
        <p className="empty-state">{t.noHistory}</p>
      ) : (
        <div className="history-list">
          {attempts.map((attempt) => {
            const passed = attempt.percentage >= 70;
            const date = new Date(attempt.takenAt).toLocaleString();
            return (
              <div
                key={attempt.id}
                className="history-card"
                onClick={() => navigate(`/history/${attempt.id}`)}
                role="button"
                tabIndex={0}
                onKeyDown={(e) => e.key === 'Enter' && navigate(`/history/${attempt.id}`)}
              >
                <div className="history-card-left">
                  <span className={`history-badge ${passed ? 'pass' : 'fail'}`}>
                    {passed ? '✓' : '✗'}
                  </span>
                  <div>
                    <p className="history-subject">{attempt.subjectName}</p>
                    <p className="history-date">{date}</p>
                  </div>
                </div>
                <div className="history-card-right">
                  <span className="history-score">
                    {attempt.score}/{attempt.totalQuestions}
                  </span>
                  <span className={`history-pct ${passed ? 'pass' : 'fail'}`}>
                    {attempt.percentage}%
                  </span>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
