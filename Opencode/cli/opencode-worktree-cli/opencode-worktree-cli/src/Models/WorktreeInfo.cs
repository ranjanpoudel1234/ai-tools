namespace OpencodeWorktreeCli.Models;

/// <summary>
/// Represents a git worktree discovered via `git worktree list --porcelain`.
/// Branch is null when the worktree is in a detached HEAD state.
/// </summary>
public record WorktreeInfo(
    string Path,
    string? Branch,
    string HeadSha,
    bool IsMain,
    bool IsDetached
);
