import type { MistakeExplanation } from '../types';
import './MistakeCard.css';

interface MistakeCardProps {
  mistake: MistakeExplanation;
  index: number;
}

export function MistakeCard({ mistake, index }: MistakeCardProps) {
  return (
    <div className="mistake-card" style={{ animationDelay: `${index * 0.08}s` }}>
      <div className="mistake-index">
        Question #{mistake.questionId}
      </div>
      <p className="mistake-explanation">{mistake.explanation}</p>
    </div>
  );
}
