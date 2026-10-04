import { getHabits, type Habit } from "@/lib/api";
import { HabitCard } from "@/components/HabitCard";
import { NewHabitForm } from "@/components/NewHabitForm";

export default async function Home() {
  let habits: Habit[] = [];
  let error: string | null = null;

  try {
    habits = await getHabits();
  } catch {
    error = "Can't reach the API. Is it running? (dotnet run --project api)";
  }

  return (
    <main className="mx-auto w-full max-w-2xl flex-1 px-4 py-10">
      <h1 className="mb-6 text-3xl font-bold">Habit Tracker</h1>

      <NewHabitForm />

      {error && (
        <p className="mt-6 rounded-lg bg-red-50 p-4 text-red-700 dark:bg-red-950 dark:text-red-300">
          {error}
        </p>
      )}

      {!error && habits.length === 0 && (
        <p className="mt-10 text-center text-zinc-500">No habits yet. Add your first one above.</p>
      )}

      <ul className="mt-6 flex flex-col gap-4">
        {habits.map((habit) => (
          <HabitCard key={habit.id} habit={habit} />
        ))}
      </ul>
    </main>
  );
}
