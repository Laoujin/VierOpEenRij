# VierOpEenRij Modernization — Design Spec

- **Date:** 2026-05-08
- **Run mode:** unattended autonomous run, ~1–2 hours, kicked off at session start, reviewed at session end.
- **Repo:** [Laoujin/VierOpEenRij](https://github.com/Laoujin/VierOpEenRij)
- **Original:** VB.NET WinForms Connect-Four with raw-socket networking, .NET Framework 2.0, 2015 school project.
- **Goal:** modernize to C# / .NET 10 / Avalonia / SDK-style projects / `slnx` / CI / TDD, with hot-seat + minimax bot gameplay and modern visual + sound polish.

## Constraints

- Output exit condition: a **draft PR** on `main` from a `modernize` branch with **green CI** and ready-for-review.
- TDD mandatory (CLAUDE.md). One concern per commit. No `Co-Authored-By`. Stage files by name.
- Engine assembly **≥ 90% line coverage**, enforced by CI.
- All warnings as errors. `OptionStrict` equivalent: `Nullable enable`, `TreatWarningsAsErrors=true`.
- Non-interactive: agent does not ask clarifying questions mid-run.

## Architecture

Approach: **Library + App + Tests.** Three SDK-style projects in a single `slnx` solution.

### Repo layout (`modernize` branch)

```
VierOpEenRij/
├── ConnectFour.slnx
├── Directory.Build.props          # net10.0, nullable enable, warnings-as-errors, lang version latest
├── global.json                    # pin .NET 10 SDK
├── coverlet.runsettings           # scope coverage to ConnectFour.Engine assembly
├── .editorconfig
├── .github/workflows/ci.yml
├── README.md                      # minimal functional (build/test/run + badges)
├── docs/superpowers/specs/2026-05-08-vieropeenrij-modernization-design.md
├── src/
│   ├── ConnectFour.Engine/        # pure C#, no UI deps
│   │   └── ConnectFour.Engine.csproj
│   └── ConnectFour.Desktop/       # Avalonia 11 app
│       └── ConnectFour.Desktop.csproj
└── tests/
    └── ConnectFour.Engine.Tests/  # xUnit + FluentAssertions + coverlet.collector
        └── ConnectFour.Engine.Tests.csproj
```

The original VB files (`Form1.vb`, `vieropeenrij.vb`, `4op1rij.sln`, `4op1rij.vbproj`, `AssemblyInfo.vb`, `*.bmp`, `*.gif`, `*.jpg`) and `LICENSE` / `.gitignore` adjustments are removed on the `modernize` branch. They remain on `main` as the "before" state. Removal is its own dedicated commit, performed only after the new project is green.

### Branching & PR

- Branch: `modernize`, created from `main`.
- PR: opened as **draft** with title `Modernize: VB.NET 2.0 → C# .NET 10 + Avalonia`, targeting `main`.
- PR body: Summary bullets + Test Plan checklist (per CLAUDE.md). Includes a `## What I cut` section if any planned scope item was dropped.
- Marked ready-for-review when all CI gates pass.
- **Not merged** — review and merge are user-controlled at demo time.

## ConnectFour.Engine — Domain Model

Pure C# class library, no UI references.

### Public API

```csharp
namespace ConnectFour.Engine;

public enum Player    { Blue, Red }                     // Blue moves first
public enum CellState { Empty, Blue, Red }
public enum GameStatus { InProgress, Won, Draw }

public readonly record struct Position(int Row, int Column);

public sealed class Board
{
    public int Rows { get; }                            // default 6
    public int Columns { get; }                         // default 7
    public CellState this[int row, int col] { get; }
    public bool IsColumnFull(int column);
    public bool IsFull { get; }
    public Board PlaceDisc(int column, Player player, out Position landing);  // immutable; returns new Board
}

public sealed record MoveResult(
    Board Board,
    Position Landing,
    GameStatus Status,
    Player? Winner,
    IReadOnlyList<Position> WinningLine);               // 4 cells, empty if no win

public sealed class Game
{
    public Game(int rows = 6, int columns = 7);
    public Board Board { get; }
    public Player CurrentPlayer { get; }
    public GameStatus Status { get; }
    public Player? Winner { get; }
    public IReadOnlyList<Position> WinningLine { get; }
    public event Action<MoveResult>? MovePlayed;
    public bool TryPlay(int column, out MoveResult result);
    public void Reset();
}

public interface IBot
{
    int ChooseColumn(Game game);
}

public sealed class MinimaxBot : IBot
{
    public MinimaxBot(int depth = 5, Random? rng = null);     // rng for tie-break; defaults to Random.Shared
    public int ChooseColumn(Game game);
}
```

### Bug fixes from the 2015 code

- `vieropeenrij.vb::GeldigeZet` mutates `mGewonnen = True` when the board is full → now cleanly separated as `GameStatus.Draw` vs `GameStatus.Won`.
- `vieropeenrij.vb::Speelzet` falls through with no return when the move is invalid (silenced by `OptionStrict Off`) → modern signature uses `bool TryPlay(int column, out MoveResult result)` returning `false` on rejection.
- Public mutable `Gewonnen` setter exposing internal state → removed; `Status` is read-only.

### TDD test list (drives implementation order)

Engine + Bot tests, written before implementation, one failing test per red-green cycle:

| # | Test |
|--:|---|
|  1 | New game: empty board, Blue to move, status `InProgress` |
|  2 | `TryPlay` lands disc in bottom row of empty column |
|  3 | Stacked discs land on top of each other |
|  4 | Full column → `TryPlay` returns false, board unchanged |
|  5 | Out-of-range column → false |
|  6 | Horizontal 4-in-a-row → `Won` + 4-cell `WinningLine` |
|  7 | Vertical 4-in-a-row → `Won` |
|  8 | Diagonal ↗ 4-in-a-row → `Won` |
|  9 | Diagonal ↘ 4-in-a-row → `Won` |
| 10 | Filled board with no win → `Draw` |
| 11 | Play after `Won`/`Draw` → rejected |
| 12 | `Reset` returns to fresh state, Blue to move |
| 13 | Bot plays the winning move when one exists |
| 14 | Bot blocks opponent's immediate winning move |
| 15 | Bot prefers center column on empty board |
| 16 | Bot never returns invalid column (full or out of range) |
| 17 | Determinism: same board + same depth + same seeded RNG → same move |
| 18 | Performance: depth-5 move on empty board returns in < 1s |
| 19 | Tie-break randomness: with two equal-score moves, seeded RNG selects deterministically; unseeded selects variably across runs |

## ConnectFour.Desktop — Avalonia App

Avalonia 11 + `CommunityToolkit.Mvvm`. MVVM, dependency-injected services.

### Project layout

```
src/ConnectFour.Desktop/
├── App.axaml(.cs)
├── Program.cs
├── ViewModels/
│   ├── MainWindowViewModel.cs       # owns GameViewModel; NewGame command, mode selector
│   ├── GameViewModel.cs             # wraps Engine.Game; exposes Cells, CurrentPlayer, status
│   └── CellViewModel.cs             # Row, Column, State, IsLandingPreview, IsWinningCell
├── Views/
│   ├── MainWindow.axaml             # window chrome, status bar, mode + New Game controls
│   └── BoardView.axaml              # ItemsControl over Cells in a UniformGrid
├── Services/
│   ├── ISoundService.cs
│   ├── WindowsSoundService.cs       # active when OperatingSystem.IsWindows()
│   └── NoOpSoundService.cs          # active otherwise
└── Styles/                          # disc DataTemplates, win-line glow, drop animation
```

### Game modes

- **Hot-seat** (default): two humans on one screen, alternating turns.
- **vs Bot:** human plays Blue, bot plays Red.
- **Bot vs Bot:** both sides controlled by separate `MinimaxBot` instances. Tie-break RNG ensures matches differ between runs.

Mode is selected on a top-bar control; switching mode resets the game. While it's the bot's turn, the status bar shows `"Thinking…"`, `IBot.ChooseColumn` runs on a `Task.Run`, the result drops with the same animation as a human move.

### Data flow

1. User clicks a column → `GameViewModel.PlayColumnCommand(int col)`.
2. `Game.TryPlay(col, out result)` runs in Engine.
3. Engine raises `MovePlayed` with `MoveResult`.
4. ViewModel updates the affected `CellViewModel.State`, calls `ISoundService.PlayDrop()`.
5. If `Status == Won`, ViewModel sets `IsWinningCell=true` on the four winning cells, calls `ISoundService.PlayWin()`, disables column commands.
6. New Game → `Game.Reset()`, ViewModel rebuilds cell observables, re-enables input.

### Visual polish

- **Discs:** pure XAML `Ellipse` with `RadialGradientBrush`. Three states (empty / blue / red) handled by a `DataTemplate` keyed on `CellState`. No raster asset files.
- **Drop animation:** Avalonia `Transitions` on `TranslateTransform` — disc visually drops from top to its landing row over ~300 ms before the cell's bound state flips.
- **Win-line highlight:** pulsing glow on the four winning cells (Avalonia `Animation` on `Effect.DropShadow.OpacityProperty`).
- **Column hover preview:** ghost disc rendered at the landing position when the mouse hovers a column (mirrors the original behaviour).
- **Palette:** dark background, vivid disc colors, subtle column dividers; window resizable, board scales.

### Sound

- `ISoundService` interface: `PlayDrop()`, `PlayWin()`.
- `WindowsSoundService` plays procedurally generated WAVs via `System.Media.SoundPlayer`. WAVs are constructed in-memory at startup as `MemoryStream`s — sine-wave tone for `drop`, two-note chord for `win` — no asset files shipped.
- `NoOpSoundService` is registered when `!OperatingSystem.IsWindows()` so the app launches cross-platform with silent audio.
- Cross-platform audio is explicit v2.

### Tests

- Engine and Bot are fully unit-tested (TDD list above) and gated by 90% coverage.
- View layer (Views, ViewModels) is **not** unit-tested in v1. Verified by manual smoke run during the agent's self-verification gate (see below).

## CI Workflow

`.github/workflows/ci.yml`:

```yaml
name: CI
on:
  push:
    branches: [modernize]
  pull_request:
    branches: [main]
jobs:
  build-test:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v6
      - uses: actions/setup-dotnet@v5
        with:
          global-json-file: global.json
      - run: dotnet restore ConnectFour.slnx
      - name: Verify formatting
        run: dotnet format ConnectFour.slnx --verify-no-changes --severity warn
      - run: dotnet build ConnectFour.slnx --configuration Release --no-restore /warnaserror
      - name: Test with coverage
        run: >
          dotnet test ConnectFour.slnx --configuration Release --no-build
          --logger "trx;LogFileName=results.trx"
          --collect:"XPlat Code Coverage"
          --settings coverlet.runsettings
          --results-directory TestResults
      - uses: actions/upload-artifact@v7
        if: always()
        with:
          name: test-results
          path: TestResults/**/*
      - name: Coverage summary
        uses: irongut/CodeCoverageSummary@v1.3.0
        with:
          filename: TestResults/**/coverage.cobertura.xml
          format: markdown
          output: both
          badge: true
          thresholds: '90 95'
          fail_below_min: true
      - name: Post coverage comment
        if: github.event_name == 'pull_request'
        uses: marocchino/sticky-pull-request-comment@v3
        with:
          recreate: true
          path: code-coverage-results.md
```

`coverlet.runsettings`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <Format>cobertura</Format>
          <Include>[ConnectFour.Engine]*</Include>
          <ExcludeByAttribute>GeneratedCodeAttribute,CompilerGeneratedAttribute</ExcludeByAttribute>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

CI gate: build, format-verify, test, and **Engine coverage ≥ 90%** all green; otherwise the workflow fails.

## Agent Execution Model

### Commit policy

- One concern per commit, imperative subject ≤ 72 chars, no `Co-Authored-By`, no junk in diff.
- Stage files by name (never `git add -A` / `git add .`).
- TDD red-green-refactor; each green is a commit candidate.
- Expected ~15–25 commits across the run.

### Self-verification gates

Each gate runs before the relevant commit/push/PR step. If any gate fails, the agent fixes the cause; it does not paper over with `--no-verify` or warning suppressions.

| Gate                | Command                                                                  | When                                |
|---------------------|--------------------------------------------------------------------------|-------------------------------------|
| Build clean         | `dotnet build ConnectFour.slnx -c Release /warnaserror`                  | Before each commit                  |
| Tests green         | `dotnet test ConnectFour.slnx --no-build`                                | Before each commit                  |
| Format clean        | `dotnet format ConnectFour.slnx --verify-no-changes`                     | Before each commit                  |
| App runs (smoke)    | `dotnet run --project src/ConnectFour.Desktop` (≥ 3 s window, no crash)  | After UI is wired up; before push   |
| CI green            | `gh pr checks <num> --watch` until success                               | Before declaring done               |

### Phased flow

1. **Setup commit:** branch, `Directory.Build.props`, `global.json`, `.editorconfig`, `coverlet.runsettings`, `slnx`, `.github/workflows/ci.yml`, scaffolded `ConnectFour.Engine` project (one minimal public class) and `ConnectFour.Engine.Tests` project (one smoke test asserting that public class exists). This gives CI real data: Engine compiles, the smoke test passes, coverage filter matches the assembly, the workflow is genuinely green end-to-end before any real feature work.
2. **Engine TDD loop:** tests 1–12 from the test list, one red-green cycle per commit. Each commit passes all applicable local gates (build, tests, format; smoke-run does not apply until the Desktop project exists). Push periodically; CI must remain green before continuing to the next phase.
3. **Bot TDD loop:** tests 13–19. Same cadence. Push and confirm CI green.
4. **Desktop project scaffold:** add Avalonia app, wire `MainWindow` + `BoardView` + ViewModels, hook up `Game` and `IBot` injection. Smoke-run gate replaces "tests green" sufficiency for UI-only commits.
5. **Visual polish:** drop animation, win-line glow, hover preview, palette.
6. **Sound:** `ISoundService` + Windows + NoOp implementations + procedural WAV generation.
7. **README** (minimal functional).
8. **Legacy cleanup commit:** delete original VB files in a single dedicated commit, message `Remove legacy VB.NET 2.0 project`.
9. **Open draft PR**, watch CI, mark ready-for-review when green.

### Hard stops

The agent halts and surfaces (does not paper over) on:
- CI red after 3 fix attempts on the same failure.
- Engine coverage < 90% and unable to restore.
- A test is flaky (different result on rerun without code change).
- A planned scope item must be cut → agent updates a `## What I cut` section in the PR body so it is visible at session-end review.

### Forbidden actions

- Pushing to `main`.
- Force-pushing anywhere.
- Skipping pre-commit hooks (`--no-verify`).
- Adding warning suppressions to silence real bugs.
- Editing `.git/config`.
- Modifying `main` branch in any way.
- Merging the PR.

## Out of Scope (v1 non-goals)

- Network multiplayer (raw-socket networking from the original is not reimplemented).
- Cross-platform audio (Windows-only via `System.Media.SoundPlayer`; non-Windows is silent no-op).
- CI matrix across operating systems (Windows-latest only).
- Configurable board size at runtime (constructor supports it; UI ships fixed 6×7).
- Save/load games or replays.
- Localization (English UI strings only; the original Dutch labels are not re-used).
- Accessibility audit beyond Avalonia's default `AutomationProperties`.
- Theming / dark-light toggle (single dark theme).
- Animations beyond drop, win-line glow, and hover preview.
- Bot difficulty levels (single fixed depth 5).
- Bot-vs-Bot tournament / stats / autoplay loop (mode plays one match at a time).
- View-layer unit tests (Engine fully tested; UI verified by manual smoke run).
- Documentation site or before-after narrative README — the README is minimal functional only.
- Code coverage on Desktop project (90% gate is Engine-only via `coverlet.runsettings` `Include` filter).
- Refactoring or improving the original VB files prior to deletion — they are deleted as-is.

## Definition of Done

All of the following hold at session-end review:

1. `modernize` branch exists on the remote with a clear commit history (~15–25 commits, TDD cadence).
2. Original VB files removed on `modernize`, preserved on `main`.
3. Draft PR open from `modernize` to `main`, marked ready-for-review.
4. CI green on the PR: build, format-verify, test, and Engine coverage ≥ 90%.
5. PR body contains Summary bullets + Test Plan checklist + (if applicable) `## What I cut`.
6. Coverage summary comment posted on the PR.
7. `dotnet run --project src/ConnectFour.Desktop` launches the app on Windows with a playable Connect Four game in all three modes (Hot-seat / vs Bot / Bot vs Bot).
