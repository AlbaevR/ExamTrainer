import './SubjectCard.css';
import type { Subject } from '../types';

interface SubjectCardProps {
  subject: Subject;
  onClick: () => void;
}

export function SubjectCard({ subject, onClick }: SubjectCardProps) {
  return (
    <button className="subject-card" onClick={onClick}>
      <span className="subject-name">{subject.name}</span>
      <span className="subject-arrow">→</span>
    </button>
  );
}
