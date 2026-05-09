import { useNavigate, Link } from 'react-router-dom';
import { getExamHistory } from '../services/api';
import { useApi } from '../hooks/useApi';
import { useAuth } from '../contexts/AuthContext';
import { useLanguage } from '../contexts/LanguageContext';
import { useT } from '../i18n/strings';
import { Loader } from '../components/Loader';
import { ErrorMessage } from '../components/ErrorMessage';
import type { ExamAttemptHistory } from '../types';
import './HistoryPage.css';

function groupBySubject(attempts: ExamAttemptHistory[]): Map<string, ExamAttemptHistory[]> {
  const map = new Map<string, ExamAttemptHistory[]>();
  for (const a of attempts) {
    const key = `${a.subjectId}__${a.subjectName}`;
    if (!map.has(key)) map.set(key, []);
    map.get(key)!.push(a);
  }
  return map;
}

/** Red (low score) → Blue (high score) */
function scoreToColor(pct: number): string {
  const hue = Math.round(pct * 2.17); // 0 → 0° (red), 100 → 217° (blue)
  const saturation = 75;
  const lightness = Math.round(58 - pct * 0.13); // slightly darker at top
  return `hsl(${hue}, ${saturation}%, ${lightness}%)`;
}

export function HistoryPage() {
  const navigate = useNavigate();
  const { token, username } = useAuth();
  const { lang } = useLanguage();
  const t = useT();

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

  const grouped = groupBySubject(attempts);

  return (
    <div className="history-page">
      <h1>{t.myHistory}</h1>

      {attempts.length === 0 ? (
        <p className="empty-state">{t.noHistory}</p>
      ) : (
        <div className="history-layout">

          {/* ── LEFT: attempts list ── */}
          <div className="history-list">
            {attempts.map((attempt) => {
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
                    <span className="history-badge">{attempt.score}</span>
                    <div>
                      <p className="history-subject">{attempt.subjectName}</p>
                      <p className="history-date">{date}</p>
                    </div>
                  </div>
                  <div className="history-card-right">
                    <span className="history-score">{attempt.score}/{attempt.totalQuestions}</span>
                    <span className="history-pct">{attempt.percentage}%</span>
                    <Link
                      to={`/history/${attempt.id}/feedback`}
                      className="history-ai-btn"
                      onClick={(e) => e.stopPropagation()}
                      title={t.viewAiFeedback}
                    >
                      ✨
                    </Link>
                  </div>
                </div>
              );
            })}
          </div>

          {/* ── RIGHT: vertical bar charts ── */}
          <aside className="history-charts">
            {Array.from(grouped.entries()).map(([key, subjectAttempts]) => {
              const name = subjectAttempts[0].subjectName;
              const display = subjectAttempts.slice(-10);
              const emptySlots = 10 - display.length;
              const best = Math.max(...subjectAttempts.map(a => a.percentage));
              const avg = Math.round(
                subjectAttempts.reduce((s, a) => s + a.percentage, 0) / subjectAttempts.length
              );

              return (
                <div key={key} className="vchart-card">
                  <div className="vchart-header">
                    <span className="vchart-title">{name}</span>
                    <div className="vchart-pills">
                      <span className="vchart-pill vchart-pill--best">Best {best}%</span>
                      <span className="vchart-pill vchart-pill--avg">Avg {avg}%</span>
                    </div>
                  </div>

                  <div className="vchart-area">
                    <div className="vchart-yaxis">
                      <span>100</span>
                      <span>75</span>
                      <span>50</span>
                      <span>25</span>
                      <span>0</span>
                    </div>
                    <div className="vchart-bars">
                      {/* Empty placeholder slots (oldest end) */}
                      {Array.from({ length: emptySlots }).map((_, i) => (
                        <div key={`empty-${i}`} className="vchart-bar-col vchart-bar-col--empty">
                          <span className="vchart-bar-value" style={{ visibility: 'hidden' }}> </span>
                          <div className="vchart-bar-track" />
                          <span className="vchart-bar-label"> </span>
                        </div>
                      ))}
                      {display.map((a, i) => (
                        <div
                          key={a.id}
                          className="vchart-bar-col"
                          onClick={() => navigate(`/history/${a.id}`)}
                          title={`#${i + 1} — ${a.percentage}%`}
                        >
                          <span className="vchart-bar-value">{Math.round(a.percentage)}%</span>
                          <div className="vchart-bar-track">
                            <div
                              className="vchart-bar-fill"
                              style={{
                                '--bar-h': `${a.percentage}%`,
                                '--bar-color': scoreToColor(a.percentage),
                              } as React.CSSProperties}
                            />
                          </div>
                          <span className="vchart-bar-label">#{emptySlots + i + 1}</span>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              );
            })}
          </aside>

        </div>
      )}
    </div>
  );
}
