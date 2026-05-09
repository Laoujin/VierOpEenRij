# Modernization Demo Session

This repo is the input for an "agentic engineering" demo. The original 2015 VB.NET 2.0 WinForms Connect Four lives on `main`. The autonomous Claude run rewrites it on the `modernize` branch as **C# .NET 10 + Avalonia 11 + minimax bot**, with full TDD coverage and CI.

The demo is three acts.

## Act 1 — Kickoff (paste this into Claude Code)

```text
Execute the implementation plan at docs/superpowers/plans/2026-05-08-vieropeenrij-modernization.md
using the superpowers:subagent-driven-development skill.

The plan is self-contained, fully approved, and uses the spec at
docs/superpowers/specs/2026-05-08-vieropeenrij-modernization-design.md as
its source of truth. Run unattended: do not pause to ask clarifying
questions; rely on the plan's hard-stops (3 fix attempts, coverage gate,
flaky tests, scope cuts) when surfacing blockers via the PR body's
"What I cut" section.

End condition: a draft PR open from `modernize` to `main` with green CI,
marked ready-for-review.
```

Then walk away for ~1.5 hours. The agent dispatches one subagent per task, commits ~25 times, opens a draft PR, watches CI green, marks it ready-for-review.

## Act 2 — Three reviewers post (paste this when the PR is green)

```text
Review PR <PR#> with our three reviewers in parallel. Dispatch them in a
single message so they run concurrently:

  - Hejlsberg (subagent_type: hejlsberg) — .NET / C# 13 / .NET 10 idioms
  - Beck      (subagent_type: beck)      — test quality, coverage gaps, TDD
  - Mondrian  (subagent_type: mondrian)  — Avalonia, XAML, MVVM, UI threading

Each subagent submits a PR review via gh API with one inline comment per
finding, so each finding becomes its own resolvable review thread. The
review's summary body holds the verdict + counts; the inline comments
hold the specific issues.
Once all three return, summarise the verdicts in one line each.
```

Wall-clock ≈ 2–4 minutes (slowest reviewer wins).

Each finding lands as its own inline `reviewThread` — that's what makes Act 3 possible. Without inline comments, the audience can't triage piece-by-piece.

## Act 3 — Triage with the audience, then dispatch Skeet

Open the PR live. For each reviewer thread, **resolve** the conversations you don't want addressed; **leave open** the ones that should be fixed. Then:

```text
Dispatch Skeet (subagent_type: skeet) to read the OPEN review comments on
PR <PR#> and push fixes for each. One concern per commit. Don't touch
closed comments. End condition: all open comments addressed, CI green,
final summary comment posted on the PR.
```

Skeet works through the survivors, commits per concern, pushes to `modernize`, and posts a summary referencing each fix's commit SHA.

## Roles at a glance

| Agent           | Where defined                        | Lane                                             |
|-----------------|--------------------------------------|--------------------------------------------------|
| Plan-runner     | `subagent-driven-development` skill  | Whole spec → green PR (Act 1)                    |
| **Hejlsberg**   | `.claude/agents/hejlsberg.md`        | C# / language / async / nullable                 |
| **Beck**        | `.claude/agents/beck.md`             | Test quality, coverage, brittleness, TDD         |
| **Mondrian**    | `.claude/agents/mondrian.md`         | XAML, MVVM, UI thread, layout, a11y              |
| **Skeet**       | `.claude/agents/skeet.md`            | Implements the survivors of the triage           |

## What lives where

- **`docs/superpowers/specs/2026-05-08-vieropeenrij-modernization-design.md`** — the design spec; source of truth.
- **`docs/superpowers/plans/2026-05-08-vieropeenrij-modernization.md`** — task-by-task implementation plan with full code.
- **`.claude/agents/*.md`** — reviewer and cleaner subagent definitions.

## Original repo

The 2015 VB.NET sources stay on `main`: `Form1.vb`, `vieropeenrij.vb`, `4op1rij.{sln,vbproj}`, `AssemblyInfo.vb`, plus the bmp/gif sprites. They're deleted on `modernize` in a dedicated commit so the PR diff tells a clean before/after story.
