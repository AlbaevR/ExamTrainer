const STORAGE_KEY = 'subject_emojis';

const FALLBACK_ICONS = [
  '📖', '🔬', '🏛️', '📐', '🌍', '💻', '🎨', '⚖️', '🏥', '🌱',
];

function readMap(): Record<number, string> {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : {};
  } catch {
    return {};
  }
}

function writeMap(map: Record<number, string>) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(map));
}

export function getSubjectEmoji(id: number): string {
  const map = readMap();
  return map[id] ?? FALLBACK_ICONS[id % FALLBACK_ICONS.length];
}

export function setSubjectEmoji(id: number, emoji: string): void {
  const map = readMap();
  map[id] = emoji;
  writeMap(map);
}
