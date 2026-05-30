# ConnectFour

Modernized from the 2015 VB.NET 2.0 WinForms project on `main`. C# .NET 10 + Avalonia 11 + minimax bot.

[![CI](https://github.com/Laoujin/VierOpEenRij/actions/workflows/ci.yml/badge.svg?branch=modernize)](https://github.com/Laoujin/VierOpEenRij/actions/workflows/ci.yml)

## Build

```sh
dotnet build ConnectFour.slnx
```

## Test

```sh
dotnet test ConnectFour.slnx --settings coverlet.runsettings
```

## Run

```sh
dotnet run --project src/ConnectFour.Desktop
```

## Layout

- `src/ConnectFour.Engine/` — pure-C# game logic and minimax bot
- `src/ConnectFour.Desktop/` — Avalonia 11 MVVM app
- `tests/ConnectFour.Engine.Tests/` — xUnit tests, ≥ 90% coverage on the Engine
