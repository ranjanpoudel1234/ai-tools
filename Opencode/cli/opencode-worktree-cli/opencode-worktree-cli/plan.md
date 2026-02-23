# opencodework — Implementation Plan

## Overview

`opencodework` is a .NET 10 C# global CLI tool installed via `dotnet tool install -g`. It wraps
`git worktree` operations with an interactive, ergonomic interface for a single developer working
in Git Bash on Windows. Two primary commands: `new` (creates a worktree one level above the repo
root, names the folder by replacing `/` with `_` in the branch name, then emits a per-worktree
bash script the user sources to `cd` in and launch `opencode`) and `close` (interactive list
picker with confirmation to remove a worktree). Includes `--version`, full `--help`, and an
`AGENTS.md` specific to this repo.
root, names the folder using the repo folder name as a prefix (`<repo>_<branch>` with `/` in the
branch name replaced by `_`), then emits a per-worktree bash script the user sources to `cd` in and
launch `opencode`) and `close` (interactive list picker with confirmation to remove a worktree).
Includes `--version`, full `--help`, and an `AGENTS.md` specific to this repo.

---

## Language: C# / .NET 10

Chosen because:
- .NET 10.0.102 is installed; developer has C# experience
- `dotnet tool install -g` provides clean single-command install with `opencodework` as the invocation name
- `System.CommandLine` (stable) provides first-class CLI parsing with auto-generated help
- Strong process/shell interop via `System.Diagnostics.Process`
- `Spectre.Console` provides polished interactive selection prompts

---

## Project Structure

```
opencode-worktree-cli/
├── opencode-worktree-cli.csproj
├── src/
│   ├── Program.cs
│   ├── Commands/
│   │   ├── NewCommand.cs
│   │   └── CloseCommand.cs
│   ├── Services/
│   │   ├── IProcessRunner.cs
│   │   ├── ProcessRunner.cs
│   │   ├── IGitWorktreeService.cs
│   │   └── GitWorktreeService.cs
│   ├── Models/
│   │   ├── WorktreeInfo.cs
│   │   └── ProcessResult.cs
│   └── Ui/
│       ├── IInteractivePrompt.cs
│       └── InteractivePrompt.cs
├── tests/
│   └── opencode-worktree-cli.Tests/
│       ├── opencode-worktree-cli.Tests.csproj
│       ├── Services/
│       │   ├── GitWorktreeServiceTests.cs
│       │   └── ProcessRunnerTests.cs
│       └── Commands/
│           ├── NewCommandTests.cs
│           └── CloseCommandTests.cs
├── AGENTS.md
├── README.md
└── plan.md
```

---

## NuGet Dependencies

| Package | Purpose |
|---|---|
| `System.CommandLine` | CLI argument/option parsing, auto-generated `--help` |
| `Spectre.Console` | Interactive list picker, colored output |
| `xunit` | Unit test framework (test project only) |
| `xunit.runner.visualstudio` | Test runner integration (test project only) |
| `Moq` | Mocking `IProcessRunner` and `IGitWorktreeService` in tests |
| `Microsoft.NET.Test.Sdk` | MSBuild test runner support |

---

## Models

### `Models/WorktreeInfo.cs`
```csharp
public record WorktreeInfo(
    string Path,
    string? Branch,    // null when detached HEAD
    string HeadSha,
    bool IsMain,
    bool IsDetached
);
```

### `Models/ProcessResult.cs`
```csharp
public record ProcessResult(int ExitCode, string Output, string Error);
```

---

## Interfaces

### `Services/IProcessRunner.cs`
```csharp
public interface IProcessRunner
{
    Task<ProcessResult> RunAsync(string executable, string arguments, string? workingDirectory = null);
}
```

### `Services/IGitWorktreeService.cs`
```csharp
public interface IGitWorktreeService
{
    Task<string> GetRepoRootAsync(string workingDirectory);
    Task<string> CreateWorktreeAsync(string repoRoot, string branchName, string targetPath);
    Task<IReadOnlyList<WorktreeInfo>> ListWorktreesAsync(string repoRoot);
    Task RemoveWorktreeAsync(string repoRoot, WorktreeInfo worktree);
}
```

### `Ui/IInteractivePrompt.cs`
```csharp
public interface IInteractivePrompt
{
    Task<string> PromptForBranchNameAsync();
    Task<WorktreeInfo> PickWorktreeAsync(IEnumerable<WorktreeInfo> worktrees);
    Task<bool> ConfirmAsync(string message);
}
```

---

## Functions

### `Program.cs`
- **`Main(string[] args)`** — Manually constructs the service graph (`ProcessRunner` → `GitWorktreeService` → commands), wires each subcommand's `SetHandler` lambda, attaches `--version`, and calls `rootCommand.InvokeAsync(args)`. No `IServiceCollection`; `System.CommandLine` uses lambda captures.

### `Commands/NewCommand.cs`
- **`Build(IGitWorktreeService, IInteractivePrompt, IProcessRunner)`** — Static factory; returns a configured `Command` with description and `SetHandler` bound to `ExecuteAsync`.
- **`ExecuteAsync(IGitWorktreeService, IInteractivePrompt, IProcessRunner)`** — Full `new` flow:
  1. `GetRepoRootAsync` from CWD — throws with friendly message if not in a git repo
  2. `PromptForBranchNameAsync` — user enters branch name
  3. Validate via `git check-ref-format --branch <name>` — surfaces git's own validation
  4. Derive folder name: `<repoFolder>_<branch>` where `/` becomes `_`
  5. Build target path: `Path.GetFullPath(Path.Combine(repoRoot, "..", folderName))`
  6. `CreateWorktreeAsync(repoRoot, branchName, targetPath)`
  7. Warn if `opencode` not found in PATH (`which opencode` / `where opencode`)
  8. Write per-worktree bash script `ocw_<folderName>.sh` to `$HOME/.local/tmp/opencode-worktree/` using LF line endings and no BOM — use `new UTF8Encoding(false)`
  9. Print `source ~/.local/tmp/opencode-worktree/ocw_<folderName>.sh` with forward-slash path

### `Commands/CloseCommand.cs`
- **`Build(IGitWorktreeService, IInteractivePrompt)`** — Static factory; returns configured `Command` with `SetHandler`.
- **`ExecuteAsync(IGitWorktreeService, IInteractivePrompt)`** — Close flow:
  1. `GetRepoRootAsync` from CWD
  2. `ListWorktreesAsync` — if empty, print friendly message and return
  3. `PickWorktreeAsync` — user selects from list showing `branch (path)` or `(detached HEAD at sha8) (path)`
  4. `ConfirmAsync("Remove 'branchName'? [y/N]")` — exit without error if declined
  5. `RemoveWorktreeAsync(repoRoot, worktree)` — runs `git worktree remove` then `git worktree prune`

### `Services/ProcessRunner.cs`
- **`RunAsync(executable, arguments, workingDirectory?)`** — Creates `ProcessStartInfo` with `RedirectStandardOutput/Error`, launches process, captures stdout+stderr, returns `ProcessResult`. Always explicit `workingDirectory`; throws `InvalidOperationException` if executable not found.

### `Services/GitWorktreeService.cs`
- **`GetRepoRootAsync(workingDirectory)`** — Runs `git rev-parse --show-toplevel`; trims output; throws `InvalidOperationException("Not inside a git repository.")` on non-zero exit.
- **`CreateWorktreeAsync(repoRoot, branchName, targetPath)`** — Runs `git worktree add -b <branchName> <targetPath>` (flag before path — correct git syntax); returns `targetPath` on success; throws on non-zero exit with git's error text.
- **`ListWorktreesAsync(repoRoot)`** — Runs `git worktree list --porcelain`; parses output blocks defensively (skips unknown keywords, handles `detached` instead of `branch`, handles CRLF in output, handles paths with spaces); filters `IsMain = true`; returns remaining list.
- **`RemoveWorktreeAsync(repoRoot, worktree)`** — Runs `git worktree remove <path>` then `git worktree prune` from `repoRoot`; on remove failure, surfaces git error text in exception message.

### `Ui/InteractivePrompt.cs`
- **`PromptForBranchNameAsync()`** — `AnsiConsole.Ask<string>` prompt; delegates validation to caller (caller uses `git check-ref-format`).
- **`PickWorktreeAsync(worktrees)`** — Guards `AnsiConsole.Profile.Capabilities.Interactive` before showing `SelectionPrompt`; displays `branch (path)` or `(detached HEAD at sha8) (path)` per item; throws `InvalidOperationException` if non-interactive.
- **`ConfirmAsync(message)`** — `AnsiConsole.Confirm`; returns bool.

### `PathHelper` (static utility in `Ui/` or root namespace)
- **`ToDisplayPath(string path)`** — Replaces `\` with `/` for paths printed in Git Bash context.
- **`ResolveHome()`** — Returns `Environment.GetEnvironmentVariable("HOME") ?? Environment.GetFolderPath(SpecialFolder.UserProfile)`. Never uses `~` in .NET path strings.

---

## Bash Wrapper Script (emitted at runtime)

File: `~/.local/tmp/opencode-worktree/ocw_<repo>_<branch>.sh`

Contents (LF endings, UTF-8 no BOM):
```bash
cd "/c/repos/abc_feature_my-work" && opencode
```

- Path uses forward slashes (Git Bash format)
- Written with `new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)`
- One script per worktree — no collision on repeated `new` calls
- Directory created if it doesn't exist (`Directory.CreateDirectory`)
- User runs: `source ~/.local/tmp/opencode-worktree/ocw_abc_feature_my-work.sh`

---

## Critical Implementation Rules (from plan review)

1. **`git worktree add` argument order**: Flag `-b <branch>` MUST come before `<path>`. Wrong order silently fails.
2. **No `~` in .NET paths**: Always use `ResolveHome()`.
3. **LF endings + no BOM** on all bash scripts: Use `new UTF8Encoding(false)` explicitly.
4. **Detached HEAD**: `ListWorktreesAsync` parser MUST handle `detached` keyword — set `Branch = null`, `IsDetached = true`.
5. **`System.CommandLine` wiring**: Lambda capture, NOT `IServiceProvider`. No `IServiceCollection`.
6. **Paths to user**: Always `ToDisplayPath()` (forward slashes). Internal paths use `Path.DirectorySeparatorChar`.
7. **`.dotnet/tools` in PATH**: Document `export PATH="$PATH:$USERPROFILE/.dotnet/tools"` in README.
8. **Spectre.Console TTY guard**: Check `AnsiConsole.Profile.Capabilities.Interactive` before `SelectionPrompt`.

---

## Test Names and Behaviors

### `GitWorktreeServiceTests.cs`

| Test | Behavior |
|---|---|
| `GetRepoRoot_WhenInGitRepo_ReturnsAbsolutePath` | Returns correct trimmed root from `git rev-parse` |
| `GetRepoRoot_WhenNotInGitRepo_ThrowsInvalidOperationException` | Friendly error when git exits non-zero |
| `CreateWorktree_ConstructsCorrectGitAddCommand` | `-b <branch> <path>` order is correct |
| `CreateWorktree_WhenBranchAlreadyExists_ThrowsMeaningfulError` | Surfaces git's branch-exists error |
| `CreateWorktree_WhenTargetPathAlreadyExists_ThrowsFriendlyError` | Folder collision surfaced cleanly |
| `ListWorktrees_ParsesPorcelainOutput_ReturnsWorktreeInfoList` | Multi-block porcelain parsed correctly |
| `ListWorktrees_ExcludesMainWorktree_ReturnsOnlyLinkedWorktrees` | Main entry filtered out |
| `ListWorktrees_WhenNoLinkedWorktrees_ReturnsEmptyList` | Empty list, no exception |
| `ListWorktrees_WhenWorktreeIsDetachedHead_ParsesCorrectly` | Branch=null, IsDetached=true, HeadSha set |
| `ListWorktrees_WhenPathContainsSpaces_ParsesCorrectly` | Spaces in Windows path handled |
| `ListWorktrees_WhenPorcelainOutputHasCRLF_ParsesCorrectly` | Windows line endings in git output handled |
| `RemoveWorktree_RunsRemoveThenPrune` | Both commands called in sequence |
| `RemoveWorktree_WhenWorktreeHasDirtyState_ThrowsMeaningfulError` | Git dirty-worktree error surfaced |

### `ProcessRunnerTests.cs` — `[Trait("Category", "Integration")]`

| Test | Behavior |
|---|---|
| `RunAsync_WhenCommandSucceeds_ReturnsZeroExitCodeAndOutput` | Captures stdout on success |
| `RunAsync_WhenCommandFails_ReturnsNonZeroExitCode` | Non-zero exit code returned without throwing |
| `RunAsync_WhenExecutableNotFound_ThrowsInvalidOperationException` | Missing executable throws cleanly |
| `RunAsync_RespectsWorkingDirectory` | Process launched from specified directory |

### `NewCommandTests.cs`

| Test | Behavior |
|---|---|
| `Execute_DerivesFolderName_ReplacesSlashWithUnderscore` | `feature/my-work` → `repo_feature_my-work` |
| `Execute_DerivesFolderName_ReplacesMultipleSlashesWithUnderscores` | `feature/team/my-work` → `repo_feature_team_my-work` |
| `Execute_PlacesWorktree_OneLevelAboveRepoRoot` | Target path is `../folderName` relative to repo root |
| `Execute_WritesBashScript_WithLFLineEndings` | Script file has `\n` not `\r\n` |
| `Execute_WritesBashScript_WithNoBOM` | Script bytes do not start with UTF-8 BOM |
| `Execute_WritesBashScript_WithForwardSlashPath` | Path in script uses `/` not `\` |
| `Execute_WritesBashScript_PerWorktree_UniqueNames` | Each worktree gets distinct `ocw_<name>.sh` |
| `Execute_PrintsSourceInstruction_ToConsole` | Prints `source ~/.local/tmp/...` instruction with repo prefix |

### `CloseCommandTests.cs`

| Test | Behavior |
|---|---|
| `Execute_WhenNoLinkedWorktrees_PrintsFriendlyMessage` | Graceful output, no exception |
| `Execute_CallsRemove_WithUserSelectedWorktree` | Selected entry passed to `RemoveWorktreeAsync` |
| `Execute_WhenUserDeclinesToConfirm_DoesNotCallRemove` | Confirmation=no skips remove |
| `Execute_WhenMultipleWorktrees_PresentsAllInPicker` | All non-main worktrees appear in list |
| `Execute_WhenRemoveFails_DirtyWorktree_SurfacesGitError` | Dirty-state error propagates to user |

---

## AGENTS.md Structure

1. **Project purpose** — What `opencodework` is and the problem it solves
2. **Build & verification checklist** — `dotnet build` → `dotnet test` → `dotnet pack` → uninstall/reinstall → `opencodework --help`
3. **Project structure** — Folder responsibilities
4. **Architecture decisions** — `IProcessRunner` seam; `System.CommandLine` lambda wiring; bash wrapper pattern
5. **Seam boundary rule** — Never call `Process.Start` outside of `ProcessRunner.cs`
6. **Coding conventions** — Async-first, records for models, interfaces on all services, `ToDisplayPath` for all user-facing paths
7. **Known gotchas** — `~` expansion, CRLF scripts, BOM, `git worktree add` arg order, detached HEAD, Spectre TTY guard, `.dotnet/tools` PATH
8. **Testing guidance** — Mock `IProcessRunner`; no live git calls in unit tests; integration tests use `[Trait("Category", "Integration")]`
9. **Worktree workflow** — How to use `opencodework new/close` when developing this tool itself
10. **Commit standards** — Conventional commits (`feat:`, `fix:`, `test:`, `docs:`, `chore:`)
