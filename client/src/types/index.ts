export interface Subject {
  id: number;
  name: string;
}

export interface AnswerOption {
  id: number;
  text: string;
  isCorrect?: boolean; // For admin forms
  imageUrl?: string;
}

export interface Question {
  id: number;
  text: string;
  answers: AnswerOption[];
}

export interface ExamStart {
  subjectId: number;
  subjectName: string;
  questions: Question[];
}

export interface MultilingualAnswerOption {
  id: number;
  translations: Record<string, string>;
  isCorrect: boolean;
  imageUrl?: string;
}

export interface MultilingualQuestion {
  id: number;
  translations: Record<string, string>;
  answers: MultilingualAnswerOption[];
}

export interface MultilingualExamStart {
  subjectId: number;
  subjectNames: Record<string, string>;
  questions: MultilingualQuestion[];
}

export interface SubmittedAnswer {
  questionId: number;
  selectedAnswerOptionId: number;
}

export interface SubmitExamRequest {
  studentName?: string;
  subjectId: number;
  answers: SubmittedAnswer[];
}

export interface ExamResult {
  attemptId: number;
  score: number;
  totalQuestions: number;
  percentage: number;
}

export interface LoginResponse {
  username: string;
  email: string;
  token: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

// Admin translation models
export interface TranslationEntry {
  languageCode: string;
  text: string;
}

export interface SubjectWithTranslations {
  id: number;
  translations: TranslationEntry[];
}

export interface AnswerOptionWithTranslations {
  id: number;
  isCorrect: boolean;
  imageUrl?: string;
  translations: TranslationEntry[];
}

export interface QuestionWithTranslations {
  id: number;
  translations: TranslationEntry[];
  answerOptions: AnswerOptionWithTranslations[];
}

// Exam history
export interface ExamAttemptHistory {
  id: number;
  subjectName: string;
  subjectId: number;
  score: number;
  totalQuestions: number;
  percentage: number;
  takenAt: string; // ISO date string from backend
}

export interface AnswerOptionDetail {
  id: number;
  text: string;
  imageUrl?: string;
  isCorrect: boolean;
}

export interface UserAnswerDetail {
  questionId: number;
  questionText: string;
  selectedAnswerOptionId: number;
  selectedAnswerText: string;
  selectedAnswerImageUrl?: string;
  isCorrect: boolean;
  allOptions: AnswerOptionDetail[];
}

export interface ExamAttemptDetail extends ExamAttemptHistory {
  answers: UserAnswerDetail[];
}

// AI Feedback
export interface MistakeExplanation {
  questionId: number;
  explanation: string;
}

export interface FeedbackData {
  mistakes: MistakeExplanation[];
  studyRoadmap: string;
}

export interface ExamFeedback {
  attemptId: number;
  feedback: FeedbackData;
  generatedAt: string;
  // AI generations left today; null when nothing was generated or the user is an admin
  remainingToday: number | null;
  dailyLimit: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
