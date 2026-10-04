"use client";

import { useOptimistic, useTransition } from "react";
import { deleteHabit, setCheckIn } from "@/app/actions";
import { toDateKey, type Habit } from "@/lib/api";
import { HabitHeatmap } from "./HabitHeatmap";

export function HabitCard({ habit }: { habit: Habit }) {
  const today = toDateKey(new Date());
  const [isPending, startTransition] = useTransition();

  // Flip the button instantly; the real value arrives when the server action refreshes the page.
  const [doneToday, setDoneToday] = useOptimistic(habit.checkIns.includes(today));

  function toggleToday() {
    startTransition(async () => {
      setDoneToday(!doneToday);
      await setCheckIn(habit.id, today, !doneToday);
    });
  }

  function remove() {
    if (!confirm(`Delete "${habit.name}"?`)) return;
    startTransition(() => deleteHabit(habit.id));
  }

  return (
    <li className="rounded-xl border border-zinc-200 p-4 dark:border-zinc-800">
      <div className="flex items-start justify-between gap-4">
        <div className="min-w-0">
          <h2 className="truncate text-lg font-semibold">{habit.name}</h2>
          {habit.description && (
            <p className="text-sm text-zinc-500">{habit.description}</p>
          )}
          <p className="mt-1 text-sm">
            🔥 <b>{habit.currentStreak}</b> day streak
            <span className="ml-3 text-zinc-500">best: {habit.longestStreak}</span>
          </p>
        </div>

        <div className="flex shrink-0 items-center gap-2">
          <button
            onClick={toggleToday}
            className={`rounded-lg px-3 py-2 text-sm font-medium transition-colors ${
              doneToday
                ? "bg-emerald-600 text-white hover:bg-emerald-700"
                : "border border-zinc-300 hover:bg-zinc-100 dark:border-zinc-700 dark:hover:bg-zinc-900"
            }`}
          >
            {doneToday ? "✓ Done today" : "Mark today"}
          </button>
          <button
            onClick={remove}
            disabled={isPending}
            aria-label={`Delete ${habit.name}`}
            className="rounded-lg px-2 py-2 text-zinc-400 hover:text-red-600"
          >
            ✕
          </button>
        </div>
      </div>

      <div className="mt-4">
        <HabitHeatmap checkIns={habit.checkIns} />
      </div>
    </li>
  );
}
