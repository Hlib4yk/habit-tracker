# Habit Tracker

Pet project: habit tracker with streaks and a calendar heatmap. 2-day timebox.

## Stack
- `api/` — ASP.NET Core Web API (.NET), EF Core, PostgreSQL
- `api.Tests/` — xUnit tests
- `web/` — Next.js 16 (App Router, TypeScript, Tailwind). See `web/AGENTS.md`.
- Postgres runs in Docker: `docker compose up -d`

## Commands
- DB: `docker compose up -d`
- API: `dotnet run --project api` · tests: `dotnet test`
- Web: `cd web && npm run dev`

## How we work (learning project)
- The user is experienced with React/Next.js and is learning .NET for the first time.
- Backend: the user writes most C# by hand. Claude gives one worked example, then leaves
  `// TODO(you): ...` stubs with hints instead of writing the code. Explain .NET concepts
  briefly in comparison to Node/TS equivalents when they first appear.
- Streak logic is done TDD-style: Claude writes the tests, the user makes them pass.
- Frontend: Claude may write freely; the user reviews.
- Small steps, one commit per feature. Run build/tests before saying something works.
- Communicate with the user in Ukrainian; code, comments and commits in English.
