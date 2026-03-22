import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { SubjectsPage } from './pages/SubjectsPage';
import { ExamPage } from './pages/ExamPage';
import { ResultPage } from './pages/ResultPage';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <header className="app-header">
          <Link to="/" className="app-logo">
            📝 Exam Trainer
          </Link>
        </header>
        <main className="app-main">
          <Routes>
            <Route path="/" element={<SubjectsPage />} />
            <Route path="/exam/:subjectId" element={<ExamPage />} />
            <Route path="/result" element={<ResultPage />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;
