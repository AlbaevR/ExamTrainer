export interface Subject {
  id: number;
  name: string;
}

export interface AnswerOption {
  id: number;
  text: string;
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
