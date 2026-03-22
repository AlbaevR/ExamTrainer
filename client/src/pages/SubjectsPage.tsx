import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getSubjects, updateSubject, deleteSubject, createSubject, getAdminSubject } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import { useLanguage } from '../contexts/LanguageContext';
import { useT } from '../i18n/strings';
import { SubjectCard } from '../components/SubjectCard';
import { Loader } from '../components/Loader';
import { ErrorMessage } from '../components/ErrorMessage';
import type { Subject, TranslationEntry } from '../types';
import './SubjectsPage.css';

interface SubjectModalProps {
  isOpen: boolean;
  subjectId: number | null;
  onClose: () => void;
  onSaved: () => void;
}

function SubjectModal({ isOpen, subjectId, onClose, onSaved }: SubjectModalProps) {
  const t = useT();
  const [ru, setRu] = useState('');
  const [ky, setKy] = useState('');
  const [en, setEn] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen) {
      if (subjectId) {
        setLoading(true);
        getAdminSubject(subjectId).then(sub => {
          setRu(sub.translations.find(t => t.languageCode === 'ru')?.text || '');
          setKy(sub.translations.find(t => t.languageCode === 'ky')?.text || '');
          setEn(sub.translations.find(t => t.languageCode === 'en')?.text || '');
          setLoading(false);
        }).catch(err => {
          setError(err.message);
          setLoading(false);
        });
      } else {
        setRu('');
        setKy('');
        setEn('');
      }
    }
  }, [isOpen, subjectId]);

  if (!isOpen) return null;

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!ru.trim() || !ky.trim()) {
      setError('Russian and Kyrgyz translations are required.');
      return;
    }

    setLoading(true);
    setError(null);

    const translations: TranslationEntry[] = [
      { languageCode: 'ru', text: ru.trim() },
      { languageCode: 'ky', text: ky.trim() },
    ];
    if (en.trim()) {
      translations.push({ languageCode: 'en', text: en.trim() });
    }

    const payload = translations.map(t => ({ languageCode: t.languageCode, name: t.text }));

    try {
      if (subjectId) {
        await updateSubject(subjectId, { translations: payload });
      } else {
        await createSubject({ translations: payload });
      }
      onSaved();
      onClose();
    } catch (err: any) {
      setError(err.message || 'Failed to save subject');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h2>{subjectId ? t.editBtn : t.addBtn} Subject</h2>
        {error && <ErrorMessage message={error} />}
        {loading && !subjectId ? <Loader /> : (
          <form onSubmit={handleSubmit} className="admin-form">
            <div className="form-group">
              <label>Russian (Required)</label>
              <input value={ru} onChange={e => setRu(e.target.value)} required />
            </div>
            <div className="form-group">
              <label>Kyrgyz (Required)</label>
              <input value={ky} onChange={e => setKy(e.target.value)} required />
            </div>
            <div className="form-group">
              <label>English (Optional)</label>
              <input value={en} onChange={e => setEn(e.target.value)} />
            </div>
            <div className="modal-actions">
              <button type="button" className="btn btn-secondary" onClick={onClose} disabled={loading}>{t.cancel}</button>
              <button type="submit" className="btn btn-primary" disabled={loading}>{loading ? t.saving : t.save}</button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}

export function SubjectsPage() {
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const { lang } = useLanguage();
  const t = useT();
  
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  // Modal state
  const [modalOpen, setModalOpen] = useState(false);
  const [editSubjectId, setEditSubjectId] = useState<number | null>(null);

  const loadSubjects = () => {
    setLoading(true);
    getSubjects(lang)
      .then(setSubjects)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    loadSubjects();
  }, [lang]);

  const handleEdit = (subject: Subject) => {
    setEditSubjectId(subject.id);
    setModalOpen(true);
  };

  const handleAddSubject = () => {
    setEditSubjectId(null);
    setModalOpen(true);
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this subject?')) return;
    try {
      setError(null);
      await deleteSubject(id);
      loadSubjects();
    } catch (err: any) {
      setError(err.message || 'Failed to delete subject');
    }
  };

  const handleManage = (id: number) => {
    navigate(`/admin/subjects/${id}/questions`);
  };

  if (loading && subjects.length === 0) return <Loader />;
  if (error) return <ErrorMessage message={error} onRetry={loadSubjects} />;

  return (
    <div className="subjects-page">
      <div className="subjects-header">
        <div>
          <h1>{t.chooseSubject}</h1>
          <p className="subjects-subtitle">{t.chooseSubjectSubtitle}</p>
        </div>
        {isAdmin && (
          <button className="btn btn-primary" onClick={handleAddSubject}>{t.addBtn}</button>
        )}
      </div>

      <div className="subjects-list">
        {subjects.length > 0 ? (
          subjects.map((subject) => (
            <SubjectCard
              key={subject.id}
              subject={subject}
              isAdmin={isAdmin}
              onEdit={handleEdit}
              onDelete={handleDelete}
              onManage={handleManage}
              onClick={() => navigate(`/exam/${subject.id}`)}
            />
          ))
        ) : (
          <p className="empty-state">{t.noSubjects}</p>
        )}
      </div>

      <SubjectModal 
        isOpen={modalOpen} 
        subjectId={editSubjectId} 
        onClose={() => setModalOpen(false)} 
        onSaved={loadSubjects} 
      />
    </div>
  );
}
