import { useParams, useNavigate } from 'react-router-dom';
import { getAttemptDetail, BASE_URL } from '../services/api';
import { useApi } from '../hooks/useApi';
import { useAuth } from '../contexts/AuthContext';
import { useLanguage } from '../contexts/LanguageContext';
import { useT } from '../i18n/strings';
import { Loader } from '../components/Loader';
import { ErrorMessage } from '../components/ErrorMessage';
import type { ExamAttemptDetail } from '../types';
import './AttemptDetailPage.css';

function resolveImageUrl(url: string): string {
  if (!url) return url;
  if (url.startsWith('http://') || url.startsWith('https://')) return url;
  return `${BASE_URL}${url}`;
}

export function AttemptDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { token, username } = useAuth();
  const { lang } = useLanguage();
  const t = useT();

  // Guard: only call the API when authenticated — same reason as HistoryPage.
  const { data: attempt, loading, error, reload } = useApi(
    () => token ? getAttemptDetail(Number(id), lang) : Promise.resolve<ExamAttemptDetail | null>(null),
    [id, lang, token],
  );

  if (!username) {
    return (
      <div className="attempt-detail-page">
        <p className="empty-state">{t.loginToSeeHistory}</p>
      </div>
    );
  }

  if (loading) return <Loader />;
  if (error || !attempt) return <ErrorMessage message={error || 'Failed to load attempt'} onRetry={reload} />;

  const passed = attempt.percentage >= 70;
  const date = new Date(attempt.takenAt).toLocaleString();

  return (
    <div className="attempt-detail-page">
      <button className="btn btn-secondary btn-sm mb-2" onClick={() => navigate('/history')}>
        {t.back}
      </button>

      <div className="attempt-header">
        <div>
          <h1>{t.attemptDetail}</h1>
          <p className="attempt-subject">{attempt.subjectName}</p>
          <p className="attempt-date">{date}</p>
        </div>
        <span className={`result-badge ${passed ? 'pass' : 'fail'}`}>
          {passed ? t.passed : t.failed}
        </span>
      </div>

      {/* Summary stats — same pattern as ResultPage */}
      <div className="attempt-stats">
        <div className="stat">
          <span className="stat-value">{attempt.score}</span>
          <span className="stat-label">{t.correct}</span>
        </div>
        <div className="stat">
          <span className="stat-value">{attempt.totalQuestions}</span>
          <span className="stat-label">{t.total}</span>
        </div>
        <div className="stat">
          <span className="stat-value">{attempt.percentage}%</span>
          <span className="stat-label">{t.score}</span>
        </div>
      </div>

      {/* Per-question review */}
      <div className="attempt-questions">
        {attempt.answers.map((ua, idx) => (
          <div key={ua.questionId} className={`attempt-q-card ${ua.isCorrect ? 'correct' : 'wrong'}`}>
            <p className="attempt-q-text">
              <strong>Q{idx + 1}.</strong> {ua.questionText}
            </p>

            <div className="attempt-answers-grid">
              {/* Your answer */}
              <div className={`attempt-answer-block ${ua.isCorrect ? 'ans-correct' : 'ans-wrong'}`}>
                <span className="answer-label">{t.yourAnswer}</span>
                {ua.selectedAnswerImageUrl && (
                  <img
                    src={resolveImageUrl(ua.selectedAnswerImageUrl)}
                    alt="your answer"
                    className="answer-thumb"
                  />
                )}
                <span className="answer-text">{ua.selectedAnswerText}</span>
              </div>

              {/* Correct answer — only shown when the user was wrong */}
              {!ua.isCorrect && (() => {
                const correct = ua.allOptions.find(o => o.isCorrect);
                if (!correct) return null;
                return (
                  <div className="attempt-answer-block ans-correct">
                    <span className="answer-label">{t.correctAnswer}</span>
                    {correct.imageUrl && (
                      <img
                        src={resolveImageUrl(correct.imageUrl)}
                        alt="correct answer"
                        className="answer-thumb"
                      />
                    )}
                    <span className="answer-text">{correct.text}</span>
                  </div>
                );
              })()}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
