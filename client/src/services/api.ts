export const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5041';

const GENERIC_ERROR = 'Something went wrong. Please try again.';

// Keeps the generic message for display, but exposes status and machine-readable
// error codes (e.g. Identity's "PasswordTooShort") so pages can show specific messages.
export class ApiError extends Error {
  status: number;
  codes: string[];

  constructor(status: number, codes: string[]) {
    super(GENERIC_ERROR);
    this.status = status;
    this.codes = codes;
  }
}

async function toApiError(response: Response): Promise<ApiError> {
  if (response.status === 401) {
    window.dispatchEvent(new Event('api:unauthorized'));
  }
  let codes: string[] = [];
  try {
    const body = JSON.parse(await response.text());
    if (Array.isArray(body?.errors)) {
      codes = body.errors.filter((c: unknown): c is string => typeof c === 'string');
    }
  } catch {
    // non-JSON body: keep only the status
  }
  return new ApiError(response.status, codes);
}

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
    throw await toApiError(response);
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
    throw await toApiError(response);
  }

  const text = await response.text();
  return text ? JSON.parse(text) : ({} as T);
}

import type {
  Subject,
  SubmitExamRequest,
  ExamResult,
  LoginResponse,
  Question,
  RegisterRequest,
  SubjectWithTranslations,
  QuestionWithTranslations,
  ExamAttemptHistory,
  ExamAttemptDetail,
  ExamFeedback,
  PagedResult,
  MultilingualExamStart,
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

export function getAdminQuestions(
  subjectId: number,
  lang?: string,
  page = 1,
  pageSize = 20,
): Promise<PagedResult<Question>> {
  const params = new URLSearchParams();
  if (lang) params.set('lang', lang);
  params.set('page', String(page));
  params.set('pageSize', String(pageSize));
  return apiFetch<PagedResult<Question>>(`/api/subjects/${subjectId}/questions?${params}`);
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

export function startExam(subjectId: number): Promise<MultilingualExamStart> {
  return apiFetch<MultilingualExamStart>(`/api/exam/start/${subjectId}`);
}

export function submitExam(request: SubmitExamRequest): Promise<ExamResult> {
  return apiFetch<ExamResult>('/api/exam/submit', {
    method: 'POST',
    body: JSON.stringify(request),
  });
}

export function getExamHistory(lang?: string): Promise<ExamAttemptHistory[]> {
  const query = lang ? `?lang=${encodeURIComponent(lang)}` : '';
  return apiFetch<ExamAttemptHistory[]>(`/api/exam/history${query}`);
}

export function getAttemptDetail(id: number, lang?: string): Promise<ExamAttemptDetail> {
  const query = lang ? `?lang=${encodeURIComponent(lang)}` : '';
  return apiFetch<ExamAttemptDetail>(`/api/exam/attempts/${id}${query}`);
}

export function getExamFeedback(attemptId: number, lang?: string, force = false): Promise<ExamFeedback> {
  const params = new URLSearchParams();
  if (lang) params.set('lang', lang);
  if (force) params.set('force', 'true');
  const query = params.toString() ? `?${params.toString()}` : '';
  return apiFetch<ExamFeedback>(`/api/exam/attempts/${attemptId}/feedback${query}`);
}
