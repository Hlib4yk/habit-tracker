import { toDateKey } from "@/lib/api";

type Props = {
  checkIns: string[]; // "YYYY-MM-DD"
};

export function HabitHeatmap({ checkIns }: Props) {

  const set = new Set(checkIns);
  const today = new Date();
  const start = new Date(today);
  start.setDate(start.getDate() - 83);
  start.setDate(start.getDate() - start.getDay());
  const days: Date[] = [];
  for ( let day = new Date(start); day <= today; day.setDate(day.getDate() + 1)) {
    days.push(new Date(day));
  }
  const todayKey = toDateKey(today);
  
  return (
    <div className="grid grid-flow-col grid-rows-7 gap-1 w-fit">
      {days.map((day) => {
        const label = toDateKey(day);
        return <div key={label} title={label} className={`size-3 rounded-sm ${set.has(label) ? "bg-green-200" : "bg-zinc-500"} ${ label === todayKey ? "ring-2 ring-emerald-400" : ""}`}/>
      })}
    </div>
  );
}
