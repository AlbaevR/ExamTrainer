const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5062';

async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
    },
    ...options,
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Request failed with status ${response.status}`);
  }

  return response.json();
}

import type { Subject, ExamStart, SubmitExamRequest, ExamResult } from '../types';

export function getSubjects(): Promise<Subject[]> {
  return apiFetch<Subject[]>('/api/subjects');
}

export function startExam(subjectId: number, lang?: string): Promise<ExamStart> {
  const query = lang ? `?lang=${encodeURIComponent(lang)}` : '';
  return apiFetch<ExamStart>(`/api/exam/start/${subjectId}${query}`);
}

export function submitExam(request: SubmitExamRequest): Promise<ExamResult> {
  return apiFetch<ExamResult>('/api/exam/submit', {
    method: 'POST',
    body: JSON.stringify(request),
  });
}
