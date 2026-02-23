# opencode-worktree-cli

## Project Purpose
`opencodework` is a .NET 10 CLI tool that streamlines `git worktree` creation and cleanup for opencode-based workflows.
It is built for a single developer on Windows using Git Bash.

## Build & Verification
1. `dotnet format opencode-worktree-cli.csproj`
2. `dotnet format tests/opencode-worktree-cli.Tests/opencode-worktree-cli.Tests.csproj`
3. `dotnet build opencode-worktree-cli.csproj`
4. `dotnet test tests/opencode-worktree-cli.Tests/opencode-worktree-cli.Tests.csproj`
5. `dotnet pack -o nupkg`
6. `dotnet tool uninstall -g opencode-worktree-cli`
7. `dotnet tool install -g --add-source ./nupkg opencode-worktree-cli`
8. `opencodework --help`

## Project Structure
- `src/Commands` — CLI subcommands and orchestration (`new`, `close`)
- `src/Services` — Git + process execution abstractions
- `src/Models` — Small immutable records
- `src/Ui` — Interactive prompts via Spectre.Console
- `tests/` — Unit tests and integration tests

## Architecture Notes
- `IProcessRunner` is the single seam for external processes. Never call `Process.Start` outside `ProcessRunner.cs`.
- CLI parsing uses `System.CommandLine` with manual service graph wiring in `Program.cs`.
- Bash wrapper scripts are written with LF line endings and no BOM for Git Bash compatibility.

## Coding Conventions
- Async-first; avoid sync blocking.
- Prefer records for immutable models.
- Keep git command strings centralized in `GitWorktreeService`.
- Paths printed to users should use `/` (Git Bash); internal paths use `Path` APIs.

## Worktree Workflow
- `opencodework new` prompts for a branch name, creates a worktree, and prints the wrapper script path to stdout.
- `opencodework close` lists worktrees, asks for confirmation, and removes the selected worktree.

## Auto-source Setup (Git Bash)
To make `opencodework new` automatically `cd` into the new worktree, add this function to `~/.bashrc`:

```bash
opencodework() {
  if [ "$1" = "new" ]; then
    command opencodework new
    local status=$?
    if [ $status -ne 0 ]; then
      return $status
    fi
    local path_file="$HOME/.local/tmp/opencode-worktree/last-script-path.txt"
    if [ ! -f "$path_file" ]; then
      echo "[warn] last script path not found: $path_file" >&2
      return 1
    fi
    local script
    script="$(cat "$path_file")"
    if [ -z "$script" ]; then
      echo "[warn] last script path file is empty" >&2
      return 1
    fi
    source "$script"
  else
    command opencodework "$@"
  fi
}
```

## Testing Guidance
- Unit tests must mock `IProcessRunner` (no real git calls).
- `ProcessRunnerTests` are integration tests (use `[Trait("Category", "Integration")]`).

## Known Gotchas
- Never use `~` in .NET paths; use `$HOME` or `Environment.SpecialFolder.UserProfile`.
- Bash scripts must use LF (`\n`) and no BOM.
- `git worktree add` uses `-b <branch>` before the path argument.
- `git worktree list --porcelain` can emit `detached` instead of `branch`.
- `Spectre.Console` prompts require a TTY; guard with `AnsiConsole.Profile.Capabilities.Interactive`.
- Use `git check-ref-format --branch` for branch name validation.
- Ensure `.dotnet/tools` is on `PATH` for `opencodework`.

## Commit Standards
- Use conventional commits (`feat:`, `fix:`, `test:`, `docs:`)
