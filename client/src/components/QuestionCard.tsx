import './QuestionCard.css';
import type { Question } from '../types';
import { useT, formatString } from '../i18n/strings';
import { BASE_URL } from '../services/api';

interface QuestionCardProps {
  question: Question;
  questionNumber: number;
  totalQuestions: number;
  selectedOptionId: number | null;
  onSelectOption: (optionId: number) => void;
}

/** Prefix relative server paths (/uploads/...) with the API origin */
function resolveImageUrl(url: string): string {
  if (!url) return url;
  if (url.startsWith('http://') || url.startsWith('https://')) return url;
  return `${BASE_URL}${url}`;
}

export function QuestionCard({
  question,
  questionNumber,
  totalQuestions,
  selectedOptionId,
  onSelectOption,
}: QuestionCardProps) {
  const t = useT();

  return (
    <div className="question-card">
      <div className="question-header">
        <span className="question-counter">
          {formatString(t.questionCount, questionNumber, totalQuestions)}
        </span>
      </div>
      <h2 className="question-text">{question.text}</h2>
      <div className="options-list">
        {question.answers.map((option) => (
          <label
            key={option.id}
            className={`option-item ${selectedOptionId === option.id ? 'selected' : ''}`}
          >
            {option.imageUrl && (
              <img
                src={resolveImageUrl(option.imageUrl)}
                alt="option"
                className="option-image"
              />
            )}
            <span className="option-item-row">
              <input
                type="radio"
                name={`question-${question.id}`}
                checked={selectedOptionId === option.id}
                onChange={() => onSelectOption(option.id)}
              />
              <span className="option-text">{option.text}</span>
            </span>
          </label>
        ))}
      </div>
    </div>
  );
}
