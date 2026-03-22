import { useT } from '../i18n/strings';
import './Loader.css';

export function Loader() {
  const t = useT();
  return (
    <div className="loader-container">
      <div className="loader-spinner" />
      <p>{t.loading}</p>
    </div>
  );
}
