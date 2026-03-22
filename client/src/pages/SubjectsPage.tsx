import { useNavigate } from 'react-router-dom';
import { getSubjects } from '../services/api';
import { useApi } from '../hooks/useApi';
import { SubjectCard } from '../components/SubjectCard';
import { Loader } from '../components/Loader';
import { ErrorMessage } from '../components/ErrorMessage';
import './SubjectsPage.css';

export function SubjectsPage() {
  const { data: subjects, loading, error } = useApi(() => getSubjects(), []);
  const navigate = useNavigate();

  if (loading) return <Loader />;
  if (error) return <ErrorMessage message={error} onRetry={() => window.location.reload()} />;

  return (
    <div className="subjects-page">
      <h1>Choose a Subject</h1>
      <p className="subjects-subtitle">Select a subject to start your exam.</p>
      <div className="subjects-list">
        {subjects && subjects.length > 0 ? (
          subjects.map((subject) => (
            <SubjectCard
              key={subject.id}
              subject={subject}
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
