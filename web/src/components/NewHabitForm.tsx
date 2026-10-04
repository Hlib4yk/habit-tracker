"use client";

import { useRef } from "react";
import { useFormStatus } from "react-dom";
import { createHabit } from "@/app/actions";

function SubmitButton() {
  const { pending } = useFormStatus();
  return (
    <button
      type="submit"
      disabled={pending}
      className="rounded-lg bg-emerald-600 px-4 py-2 font-medium text-white hover:bg-emerald-700 disabled:opacity-50"
    >
      {pending ? "Adding…" : "Add"}
    </button>
  );
}

export function NewHabitForm() {
  const formRef = useRef<HTMLFormElement>(null);

  return (
    <form
      ref={formRef}
      action={async (formData) => {
        await createHabit(formData);
        formRef.current?.reset();
      }}
      className="flex flex-col gap-2 sm:flex-row"
    >
      <input
        name="name"
        required
        maxLength={100}
        placeholder="New habit, e.g. Read 20 pages"
        className="flex-1 rounded-lg border border-zinc-300 bg-transparent px-3 py-2 dark:border-zinc-700"
      />
      <input
        name="description"
        placeholder="Description (optional)"
        className="flex-1 rounded-lg border border-zinc-300 bg-transparent px-3 py-2 dark:border-zinc-700"
      />
      <SubmitButton />
    </form>
  );
}
