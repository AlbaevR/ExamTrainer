import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';

export type LanguageCode = 'ru' | 'ky' | 'en';

interface LanguageContextType {
  lang: LanguageCode;
  setLang: (lang: LanguageCode) => void;
}

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

const isValidLang = (lang: string | null): lang is LanguageCode => {
  return lang === 'ru' || lang === 'ky' || lang === 'en';
};

export function LanguageProvider({ children }: { children: ReactNode }) {
  const [lang, setLang] = useState<LanguageCode>(() => {
    const saved = localStorage.getItem('lang');
    return isValidLang(saved) ? saved : 'ru'; // Default to Russian
  });

  useEffect(() => {
    localStorage.setItem('lang', lang);
  }, [lang]);

  return (
    <LanguageContext.Provider value={{ lang, setLang }}>
      {children}
    </LanguageContext.Provider>
  );
}

export function useLanguage() {
  const context = useContext(LanguageContext);
  if (context === undefined) {
    throw new Error('useLanguage must be used within a LanguageProvider');
  }
  return context;
}
