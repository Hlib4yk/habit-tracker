// Mirrors HabitResponse in api/Endpoints/HabitEndpoints.cs
export type Habit = {
  id: number;
  name: string;
  description: string | null;
  createdAt: string;
  currentStreak: number;
  longestStreak: number;
  checkIns: string[]; // "YYYY-MM-DD", sorted ascending
};

export const API_URL = process.env.API_URL ?? "http://localhost:5172";

export async function getHabits(): Promise<Habit[]> {
  const res = await fetch(`${API_URL}/api/habits`);
  if (!res.ok) throw new Error(`GET /api/habits failed: ${res.status}`);
  return res.json();
}

// Local date as "YYYY-MM-DD" (toISOString() would use UTC and can be off by a day).
export function toDateKey(date: Date): string {
  const y = date.getFullYear();
  const m = String(date.getMonth() + 1).padStart(2, "0");
  const d = String(date.getDate()).padStart(2, "0");
  return `${y}-${m}-${d}`;
}
