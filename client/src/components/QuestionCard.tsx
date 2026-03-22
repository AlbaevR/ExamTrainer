import './QuestionCard.css';
import type { Question } from '../types';
import { useT, formatString } from '../i18n/strings';

interface QuestionCardProps {
  question: Question;
  questionNumber: number;
  totalQuestions: number;
  selectedOptionId: number | null;
  onSelectOption: (optionId: number) => void;
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
            <input
              type="radio"
              name={`question-${question.id}`}
              checked={selectedOptionId === option.id}
              onChange={() => onSelectOption(option.id)}
            />
            <span className="option-text">{option.text}</span>
          </label>
        ))}
      </div>
    </div>
  );
}
