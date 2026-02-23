using OpencodeWorktreeCli.Commands;
using OpencodeWorktreeCli.Models;
using Xunit;

namespace OpencodeWorktreeCli.Tests.Commands;

public class CloseCommandTests
{
    [Fact]
    public async Task Execute_WhenNoLinkedWorktrees_PrintsFriendlyMessage()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var git = new FakeGitWorktreeService { RepoRoot = repoRoot, WorktreesToReturn = Array.Empty<WorktreeInfo>() };
        var prompt = new FakePrompt();
        var command = new CloseCommand(git, prompt);

        var output = await ConsoleCapture.CaptureAsync(command.ExecuteAsync);

        Assert.Contains("No linked worktrees", output);
        Assert.Null(git.RemovedWorktree);
    }

    [Fact]
    public async Task Execute_CallsRemove_WithUserSelectedWorktree()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var worktree = new WorktreeInfo("C:/repo/../feature_one", "feature/one", "", false, false);
        var git = new FakeGitWorktreeService { RepoRoot = repoRoot, WorktreesToReturn = new[] { worktree } };
        var prompt = new FakePrompt { WorktreeSelection = worktree, ConfirmResult = true };
        var command = new CloseCommand(git, prompt);

        await ConsoleCapture.CaptureAsync(command.ExecuteAsync);

        Assert.Equal(worktree, git.RemovedWorktree);
    }

    [Fact]
    public async Task Execute_WhenConfirmationDeclined_DoesNotRemove()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var worktree = new WorktreeInfo("C:/repo/../feature_one", "feature/one", "", false, false);
        var git = new FakeGitWorktreeService { RepoRoot = repoRoot, WorktreesToReturn = new[] { worktree } };
        var prompt = new FakePrompt { WorktreeSelection = worktree, ConfirmResult = false };
        var command = new CloseCommand(git, prompt);

        await ConsoleCapture.CaptureAsync(command.ExecuteAsync);

        Assert.Null(git.RemovedWorktree);
    }

    [Fact]
    public async Task Execute_WhenRemoveFails_ThrowsInvalidOperationException()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var worktree = new WorktreeInfo("C:/repo/../feature_one", "feature/one", "", false, false);
        var git = new FakeGitWorktreeService
        {
            RepoRoot = repoRoot,
            WorktreesToReturn = new[] { worktree },
            RemoveException = new InvalidOperationException("remove failed")
        };
        var prompt = new FakePrompt { WorktreeSelection = worktree, ConfirmResult = true };
        var command = new CloseCommand(git, prompt);

        await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync());
    }
}
