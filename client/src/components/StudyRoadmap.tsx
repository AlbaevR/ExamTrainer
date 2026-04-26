import ReactMarkdown from 'react-markdown';
import { useT } from '../i18n/strings';
import './StudyRoadmap.css';

interface StudyRoadmapProps {
  text: string;
}

export function StudyRoadmap({ text }: StudyRoadmapProps) {
  const t = useT();
  return (
    <section className="study-roadmap">
      <div className="roadmap-header">
        <h2 className="roadmap-title">{t.studyRoadmap}</h2>
      </div>
      <div className="roadmap-body roadmap-markdown">
        <ReactMarkdown>{text}</ReactMarkdown>
      </div>
    </section>
  );
}
