import { useT } from '../i18n/strings';
import './ErrorMessage.css';

interface ErrorMessageProps {
  message: string;
  onRetry?: () => void;
}

export function ErrorMessage({ message, onRetry }: ErrorMessageProps) {
  const t = useT();
  return (
    <div className="error-container">
      <p className="error-text">⚠ {message}</p>
      {onRetry && (
        <button className="error-retry" onClick={onRetry}>
          {t.tryAgain}
        </button>
      )}
    </div>
  );
}
