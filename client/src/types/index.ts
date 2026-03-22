export interface Subject {
  id: number;
  name: string;
}

export interface AnswerOption {
  id: number;
  text: string;
  isCorrect?: boolean; // For admin forms
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
  translations: TranslationEntry[];
}

export interface QuestionWithTranslations {
  id: number;
  translations: TranslationEntry[];
  answerOptions: AnswerOptionWithTranslations[];
}
