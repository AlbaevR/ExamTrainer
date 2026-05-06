import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { startExam, submitExam } from '../services/api';
import { useApi } from '../hooks/useApi';
import { QuestionCard } from '../components/QuestionCard';
import { Loader } from '../components/Loader';
import { ErrorMessage } from '../components/ErrorMessage';
import { useLanguage } from '../contexts/LanguageContext';
import { useT } from '../i18n/strings';
import type { SubmittedAnswer, Question } from '../types';
import './ExamPage.css';

/** Pick the best available translation, falling back to the first one present. */
function resolveText(translations: Record<string, string>, lang: string): string {
  return translations[lang] ?? Object.values(translations)[0] ?? '';
}

export function ExamPage() {
  const { subjectId } = useParams<{ subjectId: string }>();
  const navigate = useNavigate();
  const id = Number(subjectId);

  const { lang } = useLanguage();
  const t = useT();

  // Only refetch if the subject changes, NOT when language changes
  const { data: exam, loading, error } = useApi(() => startExam(id), [id]);

  const [currentIndex, setCurrentIndex] = useState(0);
  const [answers, setAnswers] = useState<Map<number, number>>(new Map());
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  if (loading) return <Loader />;
  if (error || !exam) return <ErrorMessage message={error || t.failedLoadExam} />;

  const subjectName = resolveText(exam.subjectNames, lang);
  const multilingualQuestions = exam.questions;

  // Guard: subject exists but has no questions yet
  if (multilingualQuestions.length === 0) {
    return (
      <div className="exam-page">
        <div className="exam-subject-label">{subjectName}</div>
        <ErrorMessage message="This subject has no questions yet. Please check back later." />
        <div style={{ textAlign: 'center', marginTop: '1.5rem' }}>
          <button className="btn btn-secondary" onClick={() => navigate('/')}>
            {t.back}
          </button>
        </div>
      </div>
    );
  }

  const currentMultilingualQ = multilingualQuestions[currentIndex];

  // Resolve translations for the current language — this rerenders instantly on lang change
  const currentQuestion: Question = {
    id: currentMultilingualQ.id,
    text: resolveText(currentMultilingualQ.translations, lang),
    answers: currentMultilingualQ.answers.map(a => ({
      id: a.id,
      text: resolveText(a.translations, lang),
      isCorrect: a.isCorrect,
      imageUrl: a.imageUrl,
    })),
  };

  const isLast = currentIndex === multilingualQuestions.length - 1;
  const allAnswered = multilingualQuestions.every((q) => answers.has(q.id));

  // Progress
  const progressPct = Math.round((answers.size / multilingualQuestions.length) * 100);
  const positionPct = Math.round(((currentIndex + 1) / multilingualQuestions.length) * 100);

  function handleSelect(optionId: number) {
    setAnswers((prev) => {
      const next = new Map(prev);
      next.set(currentMultilingualQ.id, optionId);
      return next;
    });
  }

  async function handleSubmit() {
    setSubmitting(true);
    setSubmitError(null);

    const submittedAnswers: SubmittedAnswer[] = multilingualQuestions
      .filter((q) => answers.has(q.id))
      .map((q) => ({
        questionId: q.id,
        selectedAnswerOptionId: answers.get(q.id)!,
      }));

    try {
      const result = await submitExam({
        subjectId: id,
        answers: submittedAnswers,
      });
      navigate('/result', { state: { result, subjectName, subjectId: id } });
    } catch (err: unknown) {
      setSubmitError(err instanceof Error ? err.message : t.failedLoadExam);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="exam-page">
      {/* Progress Bar */}
      <div className="exam-progress-bar-wrap" role="progressbar" aria-valuenow={progressPct} aria-valuemin={0} aria-valuemax={100}>
        <div className="exam-progress-bar-track">
          <div className="exam-progress-bar-fill" style={{ width: `${positionPct}%` }} />
        </div>
        <div className="exam-progress-meta">
          <span className="exam-progress-label">
            {currentIndex + 1} / {multilingualQuestions.length}
          </span>
          <span className="exam-progress-answered">
            {answers.size} {t.answered}
          </span>
        </div>
      </div>

      {/* Subject Title */}
      <div className="exam-subject-label">{subjectName}</div>

      {/* Question */}
      <QuestionCard
        question={currentQuestion}
        questionNumber={currentIndex + 1}
        totalQuestions={multilingualQuestions.length}
        selectedOptionId={answers.get(currentMultilingualQ.id) ?? null}
        onSelectOption={handleSelect}
        revealed={answers.has(currentMultilingualQ.id)}
      />

      {submitError && <ErrorMessage message={submitError} />}

      {/* Navigation */}
      <div className="exam-nav">
        <button
          className="btn btn-secondary"
          disabled={currentIndex === 0}
          onClick={() => setCurrentIndex((i) => i - 1)}
        >
          {t.previous}
        </button>

        {/* Question dot navigator */}
        <div className="exam-dots" role="tablist" aria-label="Question navigation">
          {multilingualQuestions.map((q, i) => (
            <button
              key={q.id}
              role="tab"
              aria-selected={i === currentIndex}
              aria-label={`Question ${i + 1}`}
              className={`exam-dot ${i === currentIndex ? 'active' : ''} ${answers.has(q.id) ? 'answered' : ''}`}
              onClick={() => setCurrentIndex(i)}
            />
          ))}
        </div>

        {isLast ? (
          <button
            className="btn btn-primary"
            disabled={!allAnswered || submitting}
            onClick={handleSubmit}
          >
            {submitting ? t.submitting : t.submitExam}
          </button>
        ) : (
          <button
            className="btn btn-primary"
            onClick={() => setCurrentIndex((i) => i + 1)}
          >
            {t.next}
          </button>
        )}
      </div>
    </div>
  );
}

