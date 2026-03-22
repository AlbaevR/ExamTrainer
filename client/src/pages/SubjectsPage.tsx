import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getSubjects, updateSubject, deleteSubject, createSubject } from '../services/api';
import { useApi } from '../hooks/useApi';
import { useAuth } from '../contexts/AuthContext';
import { SubjectCard } from '../components/SubjectCard';
import { Loader } from '../components/Loader';
import { ErrorMessage } from '../components/ErrorMessage';
import type { Subject } from '../types';
import './SubjectsPage.css';

export function SubjectsPage() {
  const { data: subjects, loading, error, reload } = useApi(() => getSubjects(), []);
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const [actionError, setActionError] = useState<string | null>(null);

  const handleEdit = async (subject: Subject) => {
    const newName = prompt('Enter new subject name:', subject.name);
    if (!newName || newName === subject.name) return;
    
    try {
      setActionError(null);
      await updateSubject(subject.id, { translations: [{ languageCode: 'en', name: newName }] });
      reload();
    } catch (err: any) {
      setActionError(err.message || 'Failed to update subject');
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this subject?')) return;
    try {
      setActionError(null);
      await deleteSubject(id);
      reload();
    } catch (err: any) {
      setActionError(err.message || 'Failed to delete subject');
    }
  };

  const handleManage = (id: number) => {
    navigate(`/admin/subjects/${id}/questions`);
  };

  const handleAddSubject = async () => {
    const newName = prompt('Enter new subject name:');
    if (!newName) return;
    
    try {
      setActionError(null);
      await createSubject({ translations: [{ languageCode: 'en', name: newName }] });
      reload();
    } catch (err: any) {
      setActionError(err.message || 'Failed to create subject');
    }
  };

  if (loading) return <Loader />;
  if (error) return <ErrorMessage message={error} onRetry={() => window.location.reload()} />;

  return (
    <div className="subjects-page">
      <div className="subjects-header">
        <div>
          <h1>Choose a Subject</h1>
          <p className="subjects-subtitle">Select a subject to start your exam.</p>
        </div>
        {isAdmin && (
          <button className="btn btn-primary" onClick={handleAddSubject}>+ Add</button>
        )}
      </div>

      {actionError && <ErrorMessage message={actionError} onRetry={() => setActionError(null)} />}

      <div className="subjects-list">
        {subjects && subjects.length > 0 ? (
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
          <p className="empty-state">No subjects available.</p>
        )}
      </div>
    </div>
  );
}
