import './SubjectCard.css';
import type { Subject } from '../types';
import type { MouseEvent } from 'react';

interface SubjectCardProps {
  subject: Subject;
  onClick: () => void;
  isAdmin?: boolean;
  onEdit?: (subject: Subject) => void;
  onDelete?: (id: number) => void;
  onManage?: (id: number) => void;
}

export function SubjectCard({ subject, onClick, isAdmin, onEdit, onDelete, onManage }: SubjectCardProps) {
  
  const handleAction = (e: MouseEvent, action: () => void) => {
    e.stopPropagation();
    action();
  };

  return (
    <div className="subject-card-container">
      <button className="subject-card" onClick={onClick}>
        <span className="subject-name">{subject.name}</span>
        <span className="subject-arrow">→</span>
      </button>
      {isAdmin && (
        <div className="subject-admin-actions">
          <button className="btn btn-secondary btn-sm" onClick={(e) => handleAction(e, () => onEdit?.(subject))}>Edit</button>
          <button className="btn btn-secondary btn-sm" onClick={(e) => handleAction(e, () => onManage?.(subject.id))}>Manage Qs</button>
          <button className="btn btn-danger btn-sm" onClick={(e) => handleAction(e, () => onDelete?.(subject.id))}>Delete</button>
        </div>
      )}
    </div>
  );
}
