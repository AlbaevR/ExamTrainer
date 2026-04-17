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
    <div className="question-card" role="group" aria-labelledby={`question-${question.id}-text`}>
      {/* Counter chip */}
      <div className="question-header">
        <span className="question-counter">
          {formatString(t.questionCount, questionNumber, totalQuestions)}
        </span>
      </div>

      {/* Question text */}
      <h2 className="question-text" id={`question-${question.id}-text`}>
        {question.text}
      </h2>

      {/* Options */}
      <div className="options-list" role="radiogroup" aria-label="Answer options">
        {question.answers.map((option, idx) => {
          const isSelected = selectedOptionId === option.id;
          return (
            <label
              key={option.id}
              className={`option-item ${isSelected ? 'selected' : ''}`}
              htmlFor={`opt-${question.id}-${option.id}`}
            >
              <input
                type="radio"
                id={`opt-${question.id}-${option.id}`}
                name={`question-${question.id}`}
                checked={isSelected}
                onChange={() => onSelectOption(option.id)}
                className="option-radio-input"
              />
              {/* Custom radio dot */}
              <span className="option-radio-dot" aria-hidden="true">
                {isSelected && <span className="option-radio-fill" />}
              </span>

              {/* Option index letter (A, B, C, D…) */}
              <span className="option-index" aria-hidden="true">
                {String.fromCharCode(65 + idx)}
              </span>

              {/* Content */}
              <span className="option-content">
                {option.imageUrl && (
                  <img
                    src={resolveImageUrl(option.imageUrl)}
                    alt={`Option ${String.fromCharCode(65 + idx)}`}
                    className="option-image"
                  />
                )}
                <span className="option-text">{option.text}</span>
              </span>
            </label>
          );
        })}
      </div>
    </div>
  );
}
