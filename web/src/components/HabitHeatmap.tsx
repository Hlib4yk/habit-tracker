type Props = {
  checkIns: string[]; // "YYYY-MM-DD"
};

// TODO(you): GitHub-style heatmap for the last ~12 weeks.
//   - One small square per day, columns = weeks, rows = weekdays (grid-flow-col + grid-rows-7 in Tailwind).
//   - Green if the day is in checkIns, grey otherwise; today could get a ring.
//   - toDateKey(date) from "@/lib/api" gives you the same "YYYY-MM-DD" format as the API — use it to compare.
//   - A Set makes "is this day checked in?" O(1).
//   - Bonus: title={dateKey} on each square for a hover tooltip.
export function HabitHeatmap({ checkIns }: Props) {
  return (
    <div className="rounded-lg border border-dashed border-zinc-300 p-3 text-sm text-zinc-500 dark:border-zinc-700">
      Heatmap goes here — {checkIns.length} check-ins so far
    </div>
  );
}
