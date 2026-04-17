import './SubjectCard.css';
import type { Subject } from '../types';
import type { MouseEvent } from 'react';
import { useT } from '../i18n/strings';
import { getSubjectEmoji } from '../utils/subjectEmoji';

interface SubjectCardProps {
  subject: Subject;
  onClick: () => void;
  isAdmin?: boolean;
  onEdit?: (subject: Subject) => void;
  onDelete?: (id: number) => void;
  onManage?: (id: number) => void;
}

export function SubjectCard({ subject, onClick, isAdmin, onEdit, onDelete, onManage }: SubjectCardProps) {
  const t = useT();

  const handleAction = (e: MouseEvent, action: () => void) => {
    e.stopPropagation();
    action();
  };

  return (
    <div className="subject-card-container">
      <button className="subject-card" onClick={onClick} aria-label={subject.name}>
        <div className="subject-card-icon" aria-hidden="true">
          {getSubjectEmoji(subject.id)}
        </div>
        <div className="subject-card-body">
          <span className="subject-name">{subject.name}</span>
          <span className="subject-card-cta">
            {t.submitExam.includes('Submit') ? 'Start Exam' : 'Начать →'}
          </span>
        </div>
        <div className="subject-card-arrow" aria-hidden="true">→</div>
      </button>

      {isAdmin && (
        <div className="subject-admin-actions">
          <button
            className="btn btn-secondary btn-sm"
            onClick={(e) => handleAction(e, () => onEdit?.(subject))}
          >
            {t.editBtn}
          </button>
          <button
            className="btn btn-secondary btn-sm"
            onClick={(e) => handleAction(e, () => onManage?.(subject.id))}
          >
            {t.manageQsBtn}
          </button>
          <button
            className="btn btn-danger btn-sm"
            onClick={(e) => handleAction(e, () => onDelete?.(subject.id))}
          >
            {t.deleteBtn}
          </button>
        </div>
      )}
    </div>
  );
}
