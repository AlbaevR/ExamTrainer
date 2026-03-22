import { useLocation, useNavigate } from 'react-router-dom';
import type { ExamResult } from '../types';
import './ResultPage.css';

interface ResultState {
  result: ExamResult;
  subjectName: string;
}

export function ResultPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const state = location.state as ResultState | null;

  if (!state) {
    return (
      <div className="result-page">
        <p>No result data. Please take an exam first.</p>
        <button className="btn btn-primary" onClick={() => navigate('/')}>
          Go to Subjects
        </button>
      </div>
    );
  }

  const { result, subjectName } = state;
  const passed = result.percentage >= 70;

  return (
    <div className="result-page">
      <div className="result-card">
        <div className={`result-badge ${passed ? 'pass' : 'fail'}`}>
          {passed ? '✓ Passed' : '✗ Failed'}
        </div>

        <h1>Exam Results</h1>
        {subjectName && <p className="result-subject">{subjectName}</p>}

        <div className="result-stats">
          <div className="stat">
            <span className="stat-value">{result.score}</span>
            <span className="stat-label">Correct</span>
          </div>
          <div className="stat">
            <span className="stat-value">{result.totalQuestions}</span>
            <span className="stat-label">Total</span>
          </div>
          <div className="stat">
            <span className="stat-value">{result.percentage}%</span>
            <span className="stat-label">Score</span>
          </div>
        </div>

        <div className="result-actions">
          <button className="btn btn-primary" onClick={() => navigate('/')}>
            Back to Subjects
          </button>
        </div>
      </div>
    </div>
  );
}
