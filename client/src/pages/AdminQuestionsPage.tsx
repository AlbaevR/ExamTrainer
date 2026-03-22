import { useState } from 'react';
import type { FormEvent } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getAdminQuestions, createQuestion, updateQuestion, deleteQuestion } from '../services/api';
import { useApi } from '../hooks/useApi';
import { useAuth } from '../contexts/AuthContext';
import { Loader } from '../components/Loader';
import { ErrorMessage } from '../components/ErrorMessage';
import type { Question } from '../types';
import './AdminQuestionsPage.css';

interface OptionInput {
  id?: number;
  text: string;
  isCorrect: boolean;
}

export function AdminQuestionsPage() {
  const { subjectId } = useParams<{ subjectId: string }>();
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const id = Number(subjectId);

  const { data: questions, loading, error, reload } = useApi(() => getAdminQuestions(id), [id]);

  const [isEditing, setIsEditing] = useState<number | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [formText, setFormText] = useState('');
  const [formOptions, setFormOptions] = useState<OptionInput[]>([]);
  const [actionError, setActionError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  if (!isAdmin) {
    return <ErrorMessage message="Access denied. Admin only." />;
  }

  if (loading) return <Loader />;
  if (error || !questions) return <ErrorMessage message={error || 'Failed to load questions'} onRetry={reload} />;

  const resetForm = () => {
    setIsEditing(null);
    setShowForm(false);
    setFormText('');
    setFormOptions([{ text: '', isCorrect: true }, { text: '', isCorrect: false }]);
    setActionError(null);
  };

  const startAdd = () => {
    resetForm();
    setShowForm(true);
  };

  const startEdit = (q: Question) => {
    resetForm();
    setIsEditing(q.id);
    setFormText(q.text);
    setFormOptions(q.answers.map(a => ({ id: a.id, text: a.text, isCorrect: a.isCorrect || false })));
    setShowForm(true);
  };

  const handleDelete = async (qId: number) => {
    if (!window.confirm('Delete this question?')) return;
    try {
      setActionError(null);
      await deleteQuestion(id, qId);
      reload();
    } catch (err: any) {
      setActionError(err.message || 'Failed to delete question');
    }
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!formText.trim() || formOptions.length < 2) {
      setActionError('Question must have text and at least 2 options.');
      return;
    }

    if (!formOptions.some(o => o.isCorrect)) {
      setActionError('At least one option must be correct.');
      return;
    }

    setSubmitting(true);
    setActionError(null);

    try {
      if (isEditing) {
        await updateQuestion(id, isEditing, {
          translations: [{ languageCode: 'en', text: formText }],
          answerOptions: formOptions.map(o => ({
            id: (o.id ?? 0) as number,
            isCorrect: o.isCorrect,
            translations: [{ languageCode: 'en', text: o.text }],
          })),
        });
      } else {
        await createQuestion(id, {
          translations: [{ languageCode: 'en', text: formText }],
          answerOptions: formOptions.map(o => ({
            isCorrect: o.isCorrect,
            translations: [{ languageCode: 'en', text: o.text }],
          })),
        });
      }
      reload();
      resetForm();
    } catch (err: any) {
      setActionError(err.message || 'Failed to save question');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="admin-questions-page">
      <div className="aq-header">
        <div>
          <button className="btn btn-secondary btn-sm mb-2" onClick={() => navigate('/')}>← Back</button>
          <h1>Manage Questions</h1>
          <p className="aq-subtitle">Total Questions: {questions.length}</p>
        </div>
        {!showForm && <button className="btn btn-primary" onClick={startAdd}>+ Add Question</button>}
      </div>

      {actionError && <ErrorMessage message={actionError} onRetry={() => setActionError(null)} />}

      {showForm && (
        <form onSubmit={handleSubmit} className="aq-form-card">
          <h2>{isEditing ? 'Edit Question' : 'Add Question'}</h2>
          <div className="form-group">
            <label>Question Text</label>
            <input type="text" value={formText} onChange={e => setFormText(e.target.value)} required />
          </div>
          
          <div className="aq-options-list">
            <label>Answer Options</label>
            {formOptions.map((opt, i) => (
              <div key={i} className="aq-option-row">
                <input 
                  type="radio" 
                  checked={opt.isCorrect} 
                  onChange={() => {
                    const next = [...formOptions];
                    next.forEach((o, idx) => o.isCorrect = (idx === i));
                    setFormOptions(next);
                  }} 
                  title="Mark correct"
                />
                <input 
                  type="text" 
                  value={opt.text} 
                  onChange={e => {
                    const next = [...formOptions];
                    next[i].text = e.target.value;
                    setFormOptions(next);
                  }}
                  required
                  placeholder="Option text..."
                />
                <button 
                  type="button" 
                  className="btn btn-danger btn-sm"
                  onClick={() => setFormOptions(opts => opts.filter((_, idx) => idx !== i))}
                  disabled={formOptions.length <= 2}
                >Del</button>
              </div>
            ))}
            <button 
              type="button" 
              className="btn btn-secondary btn-sm aq-add-opt"
              onClick={() => setFormOptions([...formOptions, { text: '', isCorrect: false }])}
            >+ Add Option</button>
          </div>

          <div className="aq-form-actions">
            <button type="button" className="btn btn-secondary" disabled={submitting} onClick={resetForm}>Cancel</button>
            <button type="submit" className="btn btn-primary" disabled={submitting}>
              {submitting ? 'Saving...' : 'Save Question'}
            </button>
          </div>
        </form>
      )}

      {!showForm && (
        <div className="aq-list">
          {questions.map((q, idx) => (
            <div key={q.id} className="aq-card">
              <div className="aq-card-header">
                <strong>Q{idx + 1}. {q.text}</strong>
                <div className="aq-card-actions">
                  <button className="btn btn-secondary btn-sm" onClick={() => startEdit(q)}>Edit</button>
                  <button className="btn btn-danger btn-sm" onClick={() => handleDelete(q.id)}>Delete</button>
                </div>
              </div>
              <ul className="aq-answers">
                {q.answers.map(a => (
                  <li key={a.id} className={a.isCorrect ? 'aq-correct' : ''}>
                    {a.isCorrect ? '✓ ' : '○ '}{a.text}
                  </li>
                ))}
              </ul>
            </div>
          ))}
          {questions.length === 0 && <p className="empty-state">No questions found.</p>}
        </div>
      )}
    </div>
  );
}
