import { useT } from '../i18n/strings';
import './Loader.css';

export function Loader() {
  const t = useT();
  return (
    <div className="loader">
      <div className="loader-spinner" />
      <p className="loader-text">{t.loading}</p>
    </div>
  );
}
