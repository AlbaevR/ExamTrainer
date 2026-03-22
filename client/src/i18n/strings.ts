import { useLanguage, type LanguageCode } from '../contexts/LanguageContext';

export interface I18nStrings {
  // App
  appName: string;
  login: string;
  logout: string;
  hi: string;

  // SubjectsPage
  chooseSubject: string;
  chooseSubjectSubtitle: string;
  addBtn: string;
  editBtn: string;
  manageQsBtn: string;
  deleteBtn: string;
  noSubjects: string;

  // ExamPage
  questionCount: string;
  answered: string;
  previous: string;
  next: string;
  submitExam: string;
  submitting: string;
  failedLoadExam: string;

  // ResultPage
  noResultData: string;
  goToSubjects: string;
  passed: string;
  failed: string;
  examResults: string;
  correct: string;
  total: string;
  score: string;
  backToSubjects: string;

  // LoginPage & RegisterPage
  welcomeBack: string;
  signInAccount: string;
  username: string;
  password: string;
  signInBtn: string;
  signingIn: string;
  dontHaveAccount: string;
  registerLink: string;
  createAccount: string;
  registerAccount: string;
  email: string;
  registerBtn: string;
  registering: string;
  alreadyHaveAccount: string;

  // Admin / General
  save: string;
  cancel: string;
  saving: string;
  loading: string;
  tryAgain: string;
  back: string;
  adminOnly: string;
}

const strings: Record<LanguageCode, I18nStrings> = {
  ru: {
    appName: 'ГАК Тренажер',
    login: 'Войти',
    logout: 'Выйти',
    hi: 'Привет',
    chooseSubject: 'Выберите предмет',
    chooseSubjectSubtitle: 'Выберите предмет для начала экзамена.',
    addBtn: '+ Добавить',
    editBtn: 'Изменить',
    manageQsBtn: 'Вопросы',
    deleteBtn: 'Удалить',
    noSubjects: 'Нет доступных предметов.',
    questionCount: 'Вопрос {0} из {1}',
    answered: 'отвечено',
    previous: '← Назад',
    next: 'Вперед →',
    submitExam: 'Завершить экзамен',
    submitting: 'Отправка…',
    failedLoadExam: 'Не удалось загрузить экзамен.',
    noResultData: 'Нет данных о результате. Пожалуйста, сначала сдайте экзамен.',
    goToSubjects: 'К предметам',
    passed: '✓ Сдан',
    failed: '✗ Не сдан',
    examResults: 'Результаты экзамена',
    correct: 'Верно',
    total: 'Всего',
    score: 'Балл',
    backToSubjects: 'Вернуться к предметам',
    welcomeBack: 'С возвращением',
    signInAccount: 'Войдите в свой аккаунт',
    username: 'Имя пользователя',
    password: 'Пароль',
    signInBtn: 'Войти',
    signingIn: 'Вход…',
    dontHaveAccount: 'Нет аккаунта?',
    registerLink: 'Зарегистрироваться',
    createAccount: 'Создать аккаунт',
    registerAccount: 'Зарегистрировать новый аккаунт',
    email: 'Email',
    registerBtn: 'Регистрация',
    registering: 'Регистрация…',
    alreadyHaveAccount: 'Уже есть аккаунт?',
    save: 'Сохранить',
    cancel: 'Отмена',
    saving: 'Сохранение...',
    loading: 'Загрузка…',
    tryAgain: 'Повторить',
    back: '← Назад',
    adminOnly: 'Доступ закрыт. Только для администраторов.',
  },
  ky: {
    appName: 'ГАК Тренажеру',
    login: 'Кирүү',
    logout: 'Чыгуу',
    hi: 'Салам',
    chooseSubject: 'Предметти тандаңыз',
    chooseSubjectSubtitle: 'Экзаменди баштоо үчүн предметти тандаңыз.',
    addBtn: '+ Кошуу',
    editBtn: 'Өзгөртүү',
    manageQsBtn: 'Суроолор',
    deleteBtn: 'Өчүрүү',
    noSubjects: 'Жеткиликтүү предметтер жок.',
    questionCount: 'Суроо {0} / {1}',
    answered: 'жооп берилди',
    previous: '← Артка',
    next: 'Алга →',
    submitExam: 'Экзаменди бүтүрүү',
    submitting: 'Жөнөтүлүүдө…',
    failedLoadExam: 'Экзаменди жүктөөгө болбоду.',
    noResultData: 'Жыйынтык жок. Алгач экзаменди тапшырыңыз.',
    goToSubjects: 'Предметтерге',
    passed: '✓ Өттү',
    failed: '✗ Өткөн жок',
    examResults: 'Экзамендин жыйынтыктары',
    correct: 'Туура',
    total: 'Жалпы',
    score: 'Балл',
    backToSubjects: 'Предметтерге кайтуу',
    welcomeBack: 'Кайрадан салам',
    signInAccount: 'Аккаунтуңузга кириңиз',
    username: 'Колдонуучунун аты',
    password: 'Сыр сөз',
    signInBtn: 'Кирүү',
    signingIn: 'Кирүүдө…',
    dontHaveAccount: 'Аккаунтуңуз жокпу?',
    registerLink: 'Каттоо',
    createAccount: 'Аккаунт түзүү',
    registerAccount: 'Жаңы аккаунтту каттоо',
    email: 'Email',
    registerBtn: 'Каттоо',
    registering: 'Каттоодо…',
    alreadyHaveAccount: 'Аккаунтуңуз барбы?',
    save: 'Сактоо',
    cancel: 'Жокко чыгаруу',
    saving: 'Сакталууда...',
    loading: 'Жүктөлүүдө…',
    tryAgain: 'Кайра аракет кылуу',
    back: '← Артка',
    adminOnly: 'Кирүүгө тыюу салынган. Админдер үчүн гана.',
  },
  en: {
    appName: 'GAK Trainer',
    login: 'Login',
    logout: 'Logout',
    hi: 'Hi',
    chooseSubject: 'Choose a Subject',
    chooseSubjectSubtitle: 'Select a subject to start your exam.',
    addBtn: '+ Add',
    editBtn: 'Edit',
    manageQsBtn: 'Manage Qs',
    deleteBtn: 'Delete',
    noSubjects: 'No subjects available.',
    questionCount: 'Question {0} of {1}',
    answered: 'answered',
    previous: '← Previous',
    next: 'Next →',
    submitExam: 'Submit Exam',
    submitting: 'Submitting…',
    failedLoadExam: 'Failed to load exam.',
    noResultData: 'No result data. Please take an exam first.',
    goToSubjects: 'Go to Subjects',
    passed: '✓ Passed',
    failed: '✗ Failed',
    examResults: 'Exam Results',
    correct: 'Correct',
    total: 'Total',
    score: 'Score',
    backToSubjects: 'Back to Subjects',
    welcomeBack: 'Welcome Back',
    signInAccount: 'Sign in to your account',
    username: 'Username',
    password: 'Password',
    signInBtn: 'Sign In',
    signingIn: 'Signing in…',
    dontHaveAccount: 'Don\'t have an account?',
    registerLink: 'Register',
    createAccount: 'Create Account',
    registerAccount: 'Register a new account',
    email: 'Email',
    registerBtn: 'Register',
    registering: 'Registering…',
    alreadyHaveAccount: 'Already have an account?',
    save: 'Save',
    cancel: 'Cancel',
    saving: 'Saving...',
    loading: 'Loading…',
    tryAgain: 'Try Again',
    back: '← Back',
    adminOnly: 'Access denied. Admin only.',
  }
};

export function useT() {
  const { lang } = useLanguage();
  return strings[lang];
}

// Utility to format strings like "Question {0} of {1}"
export function formatString(str: string, ...args: (string | number)[]) {
  return str.replace(/{(\d+)}/g, (match, number) => {
    return typeof args[number] !== 'undefined' ? String(args[number]) : match;
  });
}
