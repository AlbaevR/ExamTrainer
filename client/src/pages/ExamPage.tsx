import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { startExam, submitExam } from '../services/api';
import { useApi } from '../hooks/useApi';
import { QuestionCard } from '../components/QuestionCard';
import { Loader } from '../components/Loader';
import { ErrorMessage } from '../components/ErrorMessage';
import type { SubmittedAnswer } from '../types';
import './ExamPage.css';

export function ExamPage() {
  const { subjectId } = useParams<{ subjectId: string }>();
  const navigate = useNavigate();
  const id = Number(subjectId);

  const { data: exam, loading, error } = useApi(() => startExam(id), [id]);

  const [currentIndex, setCurrentIndex] = useState(0);
  const [answers, setAnswers] = useState<Map<number, number>>(new Map());
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  if (loading) return <Loader />;
  if (error || !exam) return <ErrorMessage message={error || 'Failed to load exam.'} />;

  const questions = exam.questions;
  const currentQuestion = questions[currentIndex];
  const isLast = currentIndex === questions.length - 1;
  const allAnswered = questions.every((q) => answers.has(q.id));

  function handleSelect(optionId: number) {
    setAnswers((prev) => {
      const next = new Map(prev);
      next.set(currentQuestion.id, optionId);
      return next;
    });
  }

  async function handleSubmit() {
    setSubmitting(true);
    setSubmitError(null);

    const submittedAnswers: SubmittedAnswer[] = questions
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
      navigate('/result', { state: { result, subjectName: exam?.subjectName ?? '' } });
    } catch (err: unknown) {
      setSubmitError(err instanceof Error ? err.message : 'Submission failed.');
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="exam-page">
      <div className="exam-header">
        <h1>{exam.subjectName}</h1>
        <span className="exam-progress">
          {answers.size} / {questions.length} answered
        </span>
      </div>

      <QuestionCard
        question={currentQuestion}
        questionNumber={currentIndex + 1}
        totalQuestions={questions.length}
        selectedOptionId={answers.get(currentQuestion.id) ?? null}
        onSelectOption={handleSelect}
      />

      {submitError && <ErrorMessage message={submitError} />}

      <div className="exam-nav">
        <button
          className="btn btn-secondary"
          disabled={currentIndex === 0}
          onClick={() => setCurrentIndex((i) => i - 1)}
        >
          ← Previous
        </button>

        {isLast ? (
          <button
            className="btn btn-primary"
            disabled={!allAnswered || submitting}
            onClick={handleSubmit}
          >
            {submitting ? 'Submitting…' : 'Submit Exam'}
          </button>
        ) : (
          <button
            className="btn btn-primary"
            onClick={() => setCurrentIndex((i) => i + 1)}
          >
            Next →
          </button>
        )}
      </div>
    </div>
  );
}
