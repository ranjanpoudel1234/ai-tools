# opencodework

`opencodework` is a local-only CLI tool to create and manage `git worktree`s for opencode workflows on Windows.

## Install
```bash
dotnet pack -o nupkg
dotnet tool install -g --add-source ./nupkg opencode-worktree-cli
```

## Usage
```bash
opencodework new
opencodework close
opencodework --version
```

## Auto-source (recommended)
To make `opencodework new` automatically `cd` into the new worktree, add this function to your
Git Bash profile (e.g., `~/.bashrc`):

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

## Notes
- `opencodework new` prints the wrapper script path to stdout; use `source "$(opencodework new)"` if you do not add the function above.
- Worktree folder names are prefixed with the repo folder name: `<repo>_<branch>` (slashes in branch names become `_`).
- If `opencode` is not in `PATH`, the wrapper script still changes directories.
- Git Bash must be used to source the wrapper script.
- If `opencodework` is not found, add `.dotnet/tools` to `PATH`:
  - `export PATH="$PATH:$USERPROFILE/.dotnet/tools"`
