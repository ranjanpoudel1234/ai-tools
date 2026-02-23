using OpencodeWorktreeCli.Models;

namespace OpencodeWorktreeCli.Services;

public class GitWorktreeService : IGitWorktreeService
{
    private readonly IProcessRunner _runner;

    public GitWorktreeService(IProcessRunner runner)
    {
        _runner = runner;
    }

    public async Task<string> GetRepoRootAsync(string workingDirectory)
    {
        var result = await _runner.RunAsync("git", "rev-parse --show-toplevel", workingDirectory);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Not a git repository. {CliFormatting.GetError(result)}");

        return PathHelper.NormalizePath(result.Output);
    }

    public async Task<string> CreateWorktreeAsync(string repoRoot, string branchName, string targetPath)
    {
        var normalizedTarget = PathHelper.NormalizePath(targetPath);
        if (Directory.Exists(normalizedTarget) || File.Exists(normalizedTarget))
            throw new InvalidOperationException($"Target path already exists: {normalizedTarget}");

        var args = $"worktree add -b {CliFormatting.Quote(branchName)} {CliFormatting.Quote(normalizedTarget)}";
        var result = await _runner.RunAsync("git", args, repoRoot);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to create worktree. {CliFormatting.GetError(result)}");

        return normalizedTarget;
    }

    public async Task<IReadOnlyList<WorktreeInfo>> ListWorktreesAsync(string repoRoot)
    {
        var result = await _runner.RunAsync("git", "worktree list --porcelain", repoRoot);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to list worktrees. {CliFormatting.GetError(result)}");

        var normalizedRoot = PathHelper.NormalizePath(repoRoot);
        var entries = ParsePorcelain(result.Output)
            .Select(entry => entry with { IsMain = PathsEqual(entry.Path, normalizedRoot) })
            .Where(entry => !entry.IsMain)
            .ToList();

        return entries;
    }

    public async Task RemoveWorktreeAsync(string repoRoot, WorktreeInfo worktree)
    {
        var normalizedPath = PathHelper.NormalizePath(worktree.Path);
        var removeResult = await _runner.RunAsync("git", $"worktree remove {CliFormatting.Quote(normalizedPath)}", repoRoot);
        if (!removeResult.Succeeded)
            throw new InvalidOperationException($"Failed to remove worktree. {CliFormatting.GetError(removeResult)}");

        var pruneResult = await _runner.RunAsync("git", "worktree prune", repoRoot);
        if (!pruneResult.Succeeded)
            throw new InvalidOperationException($"Failed to prune worktrees. {CliFormatting.GetError(pruneResult)}");
    }

    private static List<WorktreeInfo> ParsePorcelain(string output)
    {
        var entries = new List<WorktreeInfo>();
        string? path = null;
        string? branch = null;
        string headSha = string.Empty;
        var isDetached = false;

        foreach (var rawLine in output.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r');
            if (string.IsNullOrWhiteSpace(line))
            {
                AddEntry(entries, path, branch, headSha, isDetached);
                path = null;
                branch = null;
                headSha = string.Empty;
                isDetached = false;
                continue;
            }

            if (line.StartsWith("worktree ", StringComparison.Ordinal))
            {
                AddEntry(entries, path, branch, headSha, isDetached);
                path = PathHelper.NormalizePath(line["worktree ".Length..].Trim());
                branch = null;
                headSha = string.Empty;
                isDetached = false;
                continue;
            }

            if (line.StartsWith("HEAD ", StringComparison.Ordinal))
            {
                headSha = line["HEAD ".Length..].Trim();
                continue;
            }

            if (line.StartsWith("branch ", StringComparison.Ordinal))
            {
                var rawBranch = line["branch ".Length..].Trim();
                branch = rawBranch.StartsWith("refs/heads/", StringComparison.Ordinal)
                    ? rawBranch["refs/heads/".Length..]
                    : rawBranch;
                continue;
            }

            if (line.Equals("detached", StringComparison.Ordinal))
            {
                isDetached = true;
                branch = null;
            }
        }

        AddEntry(entries, path, branch, headSha, isDetached);
        return entries;
    }

    private static void AddEntry(
        ICollection<WorktreeInfo> entries,
        string? path,
        string? branch,
        string headSha,
        bool isDetached)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        entries.Add(new WorktreeInfo(path, branch, headSha, false, isDetached));
    }

    private static bool PathsEqual(string left, string right) =>
        string.Equals(PathHelper.NormalizePath(left), PathHelper.NormalizePath(right), StringComparison.OrdinalIgnoreCase);
}
