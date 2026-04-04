export const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5041';

async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const token = localStorage.getItem('token');
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await fetch(`${BASE_URL}${path}`, {
    ...options,
    headers: {
      ...headers,
      ...options?.headers,
    },
  });

  if (!response.ok) {
    if (response.status === 401) {
      window.dispatchEvent(new Event('api:unauthorized'));
    }
    const message = await response.text();
    throw new Error(message || `Request failed with status ${response.status}`);
  }

  // Handle 204 No Content
  if (response.status === 204) {
    return {} as T;
  }

  const text = await response.text();
  return text ? JSON.parse(text) : ({} as T);
}

// Multipart upload helper – lets the browser set the correct Content-Type boundary
async function apiFetchMultipart<T>(path: string, body: FormData): Promise<T> {
  const token = localStorage.getItem('token');
  const headers: HeadersInit = {};
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await fetch(`${BASE_URL}${path}`, {
    method: 'POST',
    headers,
    body,
  });

  if (!response.ok) {
    if (response.status === 401) {
      window.dispatchEvent(new Event('api:unauthorized'));
    }
    const message = await response.text();
    throw new Error(message || `Request failed with status ${response.status}`);
  }

  const text = await response.text();
  return text ? JSON.parse(text) : ({} as T);
}

import type { 
  Subject, 
  ExamStart, 
  SubmitExamRequest, 
  ExamResult, 
  LoginResponse, 
  Question, 
  RegisterRequest,
  SubjectWithTranslations,
  QuestionWithTranslations
} from '../types';

// --- Auth ---

export function login(credentials: { email: string; password: string }): Promise<LoginResponse> {
  return apiFetch<LoginResponse>('/api/account/login', {
    method: 'POST',
    body: JSON.stringify(credentials),
  });
}

export function register(data: RegisterRequest): Promise<LoginResponse> {
  return apiFetch<LoginResponse>('/api/account/register', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

// --- Subjects ---

export function getSubjects(lang?: string): Promise<Subject[]> {
  const query = lang ? `?lang=${encodeURIComponent(lang)}` : '';
  return apiFetch<Subject[]>(`/api/subjects${query}`);
}

export function getAdminSubject(id: number): Promise<SubjectWithTranslations> {
  return apiFetch<SubjectWithTranslations>(`/api/subjects/${id}/admin`);
}

export function createSubject(data: { translations: { languageCode: string; name: string }[] }): Promise<Subject> {
  return apiFetch<Subject>('/api/subjects', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export function updateSubject(id: number, data: { translations: { languageCode: string; name: string }[] }): Promise<Subject> {
  return apiFetch<Subject>(`/api/subjects/${id}`, {
    method: 'PATCH',
    body: JSON.stringify(data),
  });
}

export function deleteSubject(id: number): Promise<void> {
  return apiFetch<void>(`/api/subjects/${id}`, {
    method: 'DELETE',
  });
}

// --- Questions ---

export function getAdminQuestions(subjectId: number, lang?: string): Promise<Question[]> {
  const query = lang ? `?lang=${encodeURIComponent(lang)}` : '';
  return apiFetch<Question[]>(`/api/subjects/${subjectId}/questions${query}`);
}

export function getAdminQuestion(subjectId: number, id: number): Promise<QuestionWithTranslations> {
  return apiFetch<QuestionWithTranslations>(`/api/subjects/${subjectId}/questions/${id}/admin`);
}

export function uploadAnswerOptionImage(file: File): Promise<{ url: string }> {
  const form = new FormData();
  form.append('file', file);
  return apiFetchMultipart<{ url: string }>('/api/upload/answer-option-image', form);
}

export function createQuestion(subjectId: number, data: {
  translations: { languageCode: string; text: string }[];
  answerOptions: { isCorrect: boolean; imageUrl?: string; translations: { languageCode: string; text: string }[] }[];
}): Promise<Question> {
  return apiFetch<Question>(`/api/subjects/${subjectId}/questions`, {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export function updateQuestion(subjectId: number, id: number, data: {
  translations: { languageCode: string; text: string }[];
  answerOptions: { id: number; isCorrect: boolean; imageUrl?: string; translations: { languageCode: string; text: string }[] }[];
}): Promise<Question> {
  return apiFetch<Question>(`/api/subjects/${subjectId}/questions/${id}`, {
    method: 'PATCH',
    body: JSON.stringify(data),
  });
}

export function deleteQuestion(subjectId: number, id: number): Promise<void> {
  return apiFetch<void>(`/api/subjects/${subjectId}/questions/${id}`, {
    method: 'DELETE',
  });
}

// --- Exam ---

export function startExam(subjectId: number, lang?: string): Promise<ExamStart> {
  const query = lang ? `?lang=${encodeURIComponent(lang)}` : '';
  // Correct endpoint: GET /api/exam/start/{subjectId}
  return apiFetch<ExamStart>(`/api/exam/start/${subjectId}${query}`);
}

export function submitExam(request: SubmitExamRequest): Promise<ExamResult> {
  return apiFetch<ExamResult>('/api/exam/submit', {
    method: 'POST',
    body: JSON.stringify(request),
  });
}
