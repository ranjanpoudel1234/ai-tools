using OpencodeWorktreeCli.Models;

namespace OpencodeWorktreeCli.Services;

/// <summary>
/// All git worktree operations. Depends only on IProcessRunner for testability.
/// </summary>
public interface IGitWorktreeService
{
    Task<string> GetRepoRootAsync(string workingDirectory);
    Task<string> CreateWorktreeAsync(string repoRoot, string branchName, string targetPath);
    Task<IReadOnlyList<WorktreeInfo>> ListWorktreesAsync(string repoRoot);
    Task RemoveWorktreeAsync(string repoRoot, WorktreeInfo worktree);
}
