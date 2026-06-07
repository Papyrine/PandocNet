# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

PandocNet is a .NET wrapper library for [Pandoc](https://pandoc.org/) (the universal document converter). It provides strongly-typed C# option classes for each input/output format and uses the standard `System.Diagnostics.Process` API for process execution. Published as the `Pandoc` NuGet package.

## Build & Test Commands

```bash
# Build (from src/)
dotnet build src/PandocNet.slnx

# Run all tests (requires pandoc installed on PATH)
dotnet test src/Tests

# Run a single test
dotnet test src/Tests --filter "Name~TextMethod"
```

Pandoc must be installed locally (`choco install pandoc` on Windows). Tests target net9.0; the library multi-targets net7.0/net8.0/net9.0.

## Code Style

- C# preview language version (currently C# 13)
- `TreatWarningsAsErrors` is enabled — all warnings are build errors
- `EnforceCodeStyleInBuild` is enabled — code style violations fail the build
- Nullable reference types are enabled

## Architecture

**Two entry points to the same engine:**
- `PandocInstance` — static facade wrapping a singleton `PandocEngine`
- `PandocEngine` — instantiable class; accepts optional custom pandoc path

**Core conversion flow:**
1. Caller invokes `Convert<TIn, TOut>()` (stream/file output) or `ConvertToText<TIn, TOut>()` (string output), with generic type params being format option classes (e.g. `CommonMarkIn`, `HtmlOut`)
2. Engine collects CLI arguments from the option objects via `GetArguments()` virtual methods
3. `System.Diagnostics.Process` executes pandoc with piped stdin/stdout (stdin source streamed via `Input.WriteTo`, stdout target streamed via `Output.ReadFrom`)
4. Exit code is checked against `ErrorCodes` mapping; result returned as `Result` or `StringResult`

**Input/Output abstraction:**
- `Input` — polymorphic source (file path, URL, stream, byte[], or text content); auto-detects type. Implicit operators from `string`, `Stream`, `byte[]`.
- `Output` — polymorphic target (file path, stream, or StringBuilder). Implicit operators from `string`, `Stream`, `StringBuilder`.

**Options hierarchy:**
- `Options` — global pandoc options (data directory, defaults file, log file)
- `InOptions` (abstract) — base for all 39 input format classes. Subclasses override `Format` property. Located in `src/PandocNet/Input/`.
- `OutOptions` (abstract) — base for all 63 output format classes. Subclasses override `Format` property. Located in `src/PandocNet/Output/`.
- Format-specific subclasses add typed properties for that format's CLI flags (e.g. `HtmlOut` has section numbering, math rendering options)

## Testing

- Framework: NUnit with Verify.NUnit (snapshot/approval testing)
- Verified output files: `Tests.*.verified.*` with platform-specific variants (Windows vs Linux line endings)
- Tests are non-parallelizable (set in `ModuleInitializer.cs`)
- `Samples.cs` contains code snippets extracted into the README via MarkdownSnippets
- The `InputsAndOutputs` test generates `input.include.md` and `output.include.md` documentation files

## CI/CD

- AppVeyor builds on push (installs pandoc via Chocolatey, runs tests, publishes NuGet)
- GitHub Actions: auto-updates docs on push (`on-push-do-docs.yml`), auto-merges Dependabot PRs, manages milestone-based releases
