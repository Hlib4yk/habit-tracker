"use server";

import { refresh } from "next/cache";
import { API_URL } from "@/lib/api";

async function call(path: string, init: RequestInit) {
  const res = await fetch(`${API_URL}${path}`, init);
  if (!res.ok) throw new Error(`${init.method} ${path} failed: ${res.status}`);
}

export async function createHabit(formData: FormData) {
  const name = String(formData.get("name") ?? "").trim();
  const description = String(formData.get("description") ?? "").trim() || null;
  if (!name) return;

  await call("/api/habits", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name, description }),
  });
  refresh();
}

export async function updateHabit(id: number, formData: FormData) {
  const name = String(formData.get("name") ?? "").trim();
  const description = String(formData.get("description") ?? "").trim() || null;
  if (!name) return;

  await call(`/api/habits/${id}`, { 
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name, description})
  });

  refresh();
}

export async function deleteHabit(id: number) {
  await call(`/api/habits/${id}`, { method: "DELETE" });
  refresh();
}

export async function setCheckIn(id: number, date: string, done: boolean) {
  await call(`/api/habits/${id}/checkins/${date}`, { method: done ? "PUT" : "DELETE" });
  refresh();
}
