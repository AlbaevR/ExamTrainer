import { useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getExamFeedback } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import { useLanguage } from '../contexts/LanguageContext';
import { useT } from '../i18n/strings';
import { MistakeCard } from '../components/MistakeCard';
import { StudyRoadmap } from '../components/StudyRoadmap';
import type { ExamFeedback } from '../types';
import './ExamFeedbackPage.css';

function timeAgo(iso: string): string {
  const diff = Date.now() - new Date(iso).getTime();
  const mins = Math.floor(diff / 60000);
  if (mins < 1) return 'just now';
  if (mins < 60) return `${mins} min ago`;
  const hrs = Math.floor(mins / 60);
  if (hrs < 24) return `${hrs}h ago`;
  return `${Math.floor(hrs / 24)}d ago`;
}

export function ExamFeedbackPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { token, username } = useAuth();
  const { lang } = useLanguage();
  const t = useT();

  const attemptId = Number(id);

  // Manual fetch states — we don't auto-fetch so we can show a prominent CTA first,
  // and cache the result once loaded to avoid re-fetching on language switch.
  const [feedback, setFeedback] = useState<ExamFeedback | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fetched, setFetched] = useState(false);

  // Cache per lang to avoid re-fetching the same language twice
  const cache = useRef<Map<string, ExamFeedback>>(new Map());

  async function loadFeedback(force = false) {
    if (!token) return;
    const cacheKey = `${attemptId}-${lang}`;
    if (!force && cache.current.has(cacheKey)) {
      setFeedback(cache.current.get(cacheKey)!);
      setFetched(true);
      return;
    }
    setLoading(true);
    setError(null);
    try {
      const result = await getExamFeedback(attemptId, lang === 'en' ? 'ru' : lang, force);
      cache.current.set(cacheKey, result);
      setFeedback(result);
      setFetched(true);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  }

  // Unauthenticated guard
  if (!username) {
    return (
      <div className="feedback-page">
        <div className="feedback-denied">
          <span className="feedback-denied-icon"></span>
          <p>{t.loginToSeeHistory}</p>
          <button className="btn btn-primary" onClick={() => navigate('/login')}>
            {t.login}
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="feedback-page">
      {/* Back button */}
      <button
        className="btn btn-secondary btn-sm mb-2"
        onClick={() => navigate(`/history/${id}`)}
      >
        {t.back}
      </button>

      {/* Page header */}
      <div className="feedback-hero">
        <div className="feedback-hero-left">
          <h1 className="feedback-title">{t.aiFeedback}</h1>
        </div>
      </div>

      {/* Initial CTA — before any fetch */}
      {!fetched && !loading && !error && (
        <div className="feedback-cta">
          <div className="feedback-cta-illustration" aria-hidden="true"></div>
          <p className="feedback-cta-desc">
            {lang === 'ru' && 'AI проанализирует ваши ошибки и даст персональные рекомендации по обучению.'}
            {lang === 'ky' && 'AI сиздин каталарыңызды талдап, жекелештирилген окуу сунуштарын берет.'}
            {lang === 'en' && 'AI will analyze your mistakes and provide personalized study recommendations.'}
          </p>
          <button
            id="generate-feedback-btn"
            className="btn btn-ai"
            onClick={() => loadFeedback()}
          >
            ✨ {t.viewAiFeedback}
          </button>
        </div>
      )}

      {/* Loading state */}
      {loading && (
        <div className="feedback-loading">
          <div className="ai-spinner">
            <div className="ai-spinner-ring" />
          </div>
          <p className="feedback-loading-text">{t.aiFeedbackLoading}</p>
          <p className="feedback-loading-sub">
            {lang === 'ru' && 'Пожалуйста, подождите…'}
            {lang === 'ky' && 'Күтүп туруңуз…'}
            {lang === 'en' && 'Please wait…'}
          </p>
        </div>
      )}

      {/* Error state */}
      {error && !loading && (
        <div className="feedback-error">
          <span className="feedback-error-icon"></span>
          <p className="feedback-error-text">{t.aiFeedbackError}</p>
          <p className="feedback-error-detail">{error}</p>
          <button className="btn btn-primary" onClick={() => loadFeedback()}>
            {t.retryBtn}
          </button>
        </div>
      )}

      {/* Feedback content */}
      {fetched && feedback && !loading && (
        <div className="feedback-content">
          {/* Mistakes section */}
          <section className="feedback-section">
            <div className="feedback-section-header mistakes-header">
              <h2 className="feedback-section-title">
                {t.mistakes}
                {feedback.feedback.mistakes.length > 0 && (
                  <span className="mistakes-count">({feedback.feedback.mistakes.length})</span>
                )}
              </h2>
            </div>

            {feedback.feedback.mistakes.length === 0 ? (
              <div className="no-mistakes">
                <p>{t.noMistakes}</p>
                <p className="no-mistakes-sub">{t.congrats}</p>
              </div>
            ) : (
              <div className="mistakes-list">
                {feedback.feedback.mistakes.map((m, i) => (
                  <MistakeCard key={m.questionId} mistake={m} index={i} />
                ))}
              </div>
            )}
          </section>

          {/* Study roadmap section */}
          {feedback.feedback.studyRoadmap && (
            <section className="feedback-section">
              <StudyRoadmap text={feedback.feedback.studyRoadmap} />
            </section>
          )}

          {/* Generated at timestamp */}
          <p className="feedback-generated-at">
            {t.generatedAt}: {timeAgo(feedback.generatedAt)}{' '}
            <span className="feedback-generated-exact">
              ({new Date(feedback.generatedAt).toLocaleString()})
            </span>
          </p>

          {/* Refresh button */}
          <button
            className="btn btn-secondary btn-sm feedback-refresh-btn"
            onClick={() => {
              // Clear cache for this lang and re-fetch fresh AI analysis
              const cacheKey = `${attemptId}-${lang}`;
              cache.current.delete(cacheKey);
              setFetched(false);
              setFeedback(null);
              loadFeedback(true);
            }}
          >
            {lang === 'ru' && 'Обновить анализ'}
            {lang === 'ky' && 'Анализди жаңыртуу'}
            {lang === 'en' && 'Refresh Analysis'}
          </button>
        </div>
      )}
    </div>
  );
}
