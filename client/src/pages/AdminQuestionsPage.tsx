import { useState } from 'react';
import type { FormEvent } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getAdminQuestions, createQuestion, updateQuestion, deleteQuestion, getAdminQuestion } from '../services/api';
import { useApi } from '../hooks/useApi';
import { useAuth } from '../contexts/AuthContext';
import { useLanguage } from '../contexts/LanguageContext';
import { useT } from '../i18n/strings';
import { Loader } from '../components/Loader';
import { ErrorMessage } from '../components/ErrorMessage';
import type { TranslationEntry } from '../types';
import './AdminQuestionsPage.css';

interface TranslationForm {
  ru: string;
  ky: string;
  en: string;
}

interface OptionInput {
  id?: number;
  text: TranslationForm;
  isCorrect: boolean;
}

const emptyTranslations = (): TranslationForm => ({ ru: '', ky: '', en: '' });

export function AdminQuestionsPage() {
  const { subjectId } = useParams<{ subjectId: string }>();
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const { lang } = useLanguage();
  const t = useT();
  const id = Number(subjectId);

  const { data: questions, loading, error, reload } = useApi(() => getAdminQuestions(id, lang), [id, lang]);

  const [isEditing, setIsEditing] = useState<number | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [loadingForm, setLoadingForm] = useState(false);
  const [formText, setFormText] = useState<TranslationForm>(emptyTranslations());
  const [formOptions, setFormOptions] = useState<OptionInput[]>([]);
  const [actionError, setActionError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  if (!isAdmin) {
    return <ErrorMessage message={t.adminOnly} />;
  }

  if (loading) return <Loader />;
  if (error || !questions) return <ErrorMessage message={error || 'Failed to load questions'} onRetry={reload} />;

  const resetForm = () => {
    setIsEditing(null);
    setShowForm(false);
    setFormText(emptyTranslations());
    setFormOptions([
      { text: emptyTranslations(), isCorrect: true }, 
      { text: emptyTranslations(), isCorrect: false }
    ]);
    setActionError(null);
  };

  const startAdd = () => {
    resetForm();
    setShowForm(true);
  };

  const startEdit = async (qId: number) => {
    resetForm();
    setIsEditing(qId);
    setLoadingForm(true);
    setShowForm(true);
    
    try {
      const q = await getAdminQuestion(id, qId);
      
      const qTranslations = emptyTranslations();
      q.translations.forEach(tr => {
        if (tr.languageCode === 'ru') qTranslations.ru = tr.text;
        if (tr.languageCode === 'ky') qTranslations.ky = tr.text;
        if (tr.languageCode === 'en') qTranslations.en = tr.text;
      });
      setFormText(qTranslations);

      const opts: OptionInput[] = q.answerOptions.map(o => {
        const oTrans = emptyTranslations();
        o.translations.forEach(tr => {
          if (tr.languageCode === 'ru') oTrans.ru = tr.text;
          if (tr.languageCode === 'ky') oTrans.ky = tr.text;
          if (tr.languageCode === 'en') oTrans.en = tr.text;
        });
        return { id: o.id, isCorrect: o.isCorrect, text: oTrans };
      });
      setFormOptions(opts);
    } catch (err: any) {
      setActionError(err.message || 'Failed to load question details for editing');
    } finally {
      setLoadingForm(false);
    }
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

  const toPayloadTranslations = (tf: TranslationForm): TranslationEntry[] => {
    const list: TranslationEntry[] = [
      { languageCode: 'ru', text: tf.ru.trim() },
      { languageCode: 'ky', text: tf.ky.trim() },
    ];
    if (tf.en.trim()) {
      list.push({ languageCode: 'en', text: tf.en.trim() });
    }
    return list;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!formText.ru.trim() || !formText.ky.trim()) {
      setActionError('Russian and Kyrgyz question text are required.');
      return;
    }
    if (formOptions.length < 2) {
      setActionError('At least 2 options are required.');
      return;
    }
    for (const opt of formOptions) {
      if (!opt.text.ru.trim() || !opt.text.ky.trim()) {
        setActionError('Russian and Kyrgyz text are required for all answer options.');
        return;
      }
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
          translations: toPayloadTranslations(formText),
          answerOptions: formOptions.map(o => ({
            id: (o.id ?? 0) as number,
            isCorrect: o.isCorrect,
            translations: toPayloadTranslations(o.text),
          })),
        });
      } else {
        await createQuestion(id, {
          translations: toPayloadTranslations(formText),
          answerOptions: formOptions.map(o => ({
            isCorrect: o.isCorrect,
            translations: toPayloadTranslations(o.text),
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
          <button className="btn btn-secondary btn-sm mb-2" onClick={() => navigate('/')}>{t.back}</button>
          <h1>Manage Questions</h1>
          <p className="aq-subtitle">Total Questions: {questions.length}</p>
        </div>
        {!showForm && <button className="btn btn-primary" onClick={startAdd}>+ Add Question</button>}
      </div>

      {actionError && <ErrorMessage message={actionError} onRetry={() => setActionError(null)} />}

      {showForm && (
        <form onSubmit={handleSubmit} className="aq-form-card">
          <h2>{isEditing ? 'Edit Question' : 'Add Question'}</h2>
          
          {loadingForm ? <Loader /> : (
            <>
              <div className="form-group lang-group">
                <label>Question Text (RU) *</label>
                <input type="text" value={formText.ru} onChange={e => setFormText({...formText, ru: e.target.value})} required />
                <label>Question Text (KY) *</label>
                <input type="text" value={formText.ky} onChange={e => setFormText({...formText, ky: e.target.value})} required />
                <label>Question Text (EN)</label>
                <input type="text" value={formText.en} onChange={e => setFormText({...formText, en: e.target.value})} />
              </div>
              
              <div className="aq-options-list">
                <label>Answer Options</label>
                {formOptions.map((opt, i) => (
                  <div key={i} className="aq-option-row multipart">
                    <div className="aq-option-header">
                      <label>
                        <input 
                          type="radio" 
                          checked={opt.isCorrect} 
                          onChange={() => {
                            const next = [...formOptions];
                            next.forEach((o, idx) => o.isCorrect = (idx === i));
                            setFormOptions(next);
                          }} 
                        />
                        Correct Answer
                      </label>
                      <button 
                        type="button" 
                        className="btn btn-danger btn-sm"
                        onClick={() => setFormOptions(opts => opts.filter((_, idx) => idx !== i))}
                        disabled={formOptions.length <= 2}
                      >Delete</button>
                    </div>
                    <input type="text" placeholder="RU *" value={opt.text.ru} onChange={e => {
                      const next = [...formOptions];
                      next[i].text.ru = e.target.value;
                      setFormOptions(next);
                    }} required />
                    <input type="text" placeholder="KY *" value={opt.text.ky} onChange={e => {
                      const next = [...formOptions];
                      next[i].text.ky = e.target.value;
                      setFormOptions(next);
                    }} required />
                    <input type="text" placeholder="EN" value={opt.text.en} onChange={e => {
                      const next = [...formOptions];
                      next[i].text.en = e.target.value;
                      setFormOptions(next);
                    }} />
                  </div>
                ))}
                <button 
                  type="button" 
                  className="btn btn-secondary btn-sm aq-add-opt"
                  onClick={() => setFormOptions([...formOptions, { text: emptyTranslations(), isCorrect: false }])}
                >+ Add Option</button>
              </div>

              <div className="aq-form-actions">
                <button type="button" className="btn btn-secondary" disabled={submitting} onClick={resetForm}>{t.cancel}</button>
                <button type="submit" className="btn btn-primary" disabled={submitting}>
                  {submitting ? t.saving : t.save}
                </button>
              </div>
            </>
          )}
        </form>
      )}

      {!showForm && (
        <div className="aq-list">
          {questions.map((q, idx) => (
            <div key={q.id} className="aq-card">
              <div className="aq-card-header">
                <strong>Q{idx + 1}. {q.text}</strong>
                <div className="aq-card-actions">
                  <button className="btn btn-secondary btn-sm" onClick={() => startEdit(q.id)}>{t.editBtn}</button>
                  <button className="btn btn-danger btn-sm" onClick={() => handleDelete(q.id)}>{t.deleteBtn}</button>
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
