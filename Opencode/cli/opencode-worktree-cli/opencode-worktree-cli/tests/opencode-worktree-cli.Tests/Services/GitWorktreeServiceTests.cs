using OpencodeWorktreeCli.Models;
using OpencodeWorktreeCli.Services;
using Xunit;

namespace OpencodeWorktreeCli.Tests.Services;

public class GitWorktreeServiceTests
{
    [Fact]
    public async Task GetRepoRoot_WhenInGitRepo_ReturnsAbsolutePath()
    {
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, "C:/repo", ""));
        var service = new GitWorktreeService(runner);

        var result = await service.GetRepoRootAsync("C:/repo/sub");

        Assert.Equal(PathHelper.NormalizePath("C:/repo"), result);
    }

    [Fact]
    public async Task GetRepoRoot_WhenNotInGitRepo_ThrowsInvalidOperationException()
    {
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(128, "", "fatal: not a git repository"));
        var service = new GitWorktreeService(runner);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetRepoRootAsync("C:/repo"));

        Assert.Contains("Not a git repository", exception.Message);
    }

    [Fact]
    public async Task CreateWorktree_ConstructsCorrectGitAddCommand()
    {
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, "", ""));
        var service = new GitWorktreeService(runner);

        var targetPath = "C:/repos/feature_one";
        await service.CreateWorktreeAsync("C:/repo", "feature/one", targetPath);

        Assert.Single(runner.Calls);
        var call = runner.Calls[0];
        Assert.Equal("git", call.Executable);
        Assert.Contains("worktree add -b", call.Arguments);
        Assert.Contains("\"feature/one\"", call.Arguments);
        var normalizedTarget = PathHelper.NormalizePath(targetPath);
        Assert.Contains($"\"{normalizedTarget}\"", call.Arguments);
    }

    [Fact]
    public async Task CreateWorktree_WhenTargetPathAlreadyExists_ThrowsInvalidOperationException()
    {
        var runner = new FakeProcessRunner();
        var service = new GitWorktreeService(runner);
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateWorktreeAsync("C:/repo", "feature/one", tempDir));

            Assert.Contains("Target path already exists", exception.Message);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task ListWorktrees_ParsesPorcelainOutput_ReturnsWorktreeInfoList()
    {
        var output = string.Join("\n", new[]
        {
            "worktree C:/repo",
            "HEAD 1111111111111111111111111111111111111111",
            "branch refs/heads/main",
            "",
            "worktree C:/repo/../feature_one",
            "HEAD 2222222222222222222222222222222222222222",
            "branch refs/heads/feature/one",
            "",
        });

        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, output, ""));
        var service = new GitWorktreeService(runner);

        var worktrees = await service.ListWorktreesAsync("C:/repo");

        Assert.Single(worktrees);
        Assert.Equal("feature/one", worktrees[0].Branch);
    }

    [Fact]
    public async Task ListWorktrees_WhenNoLinkedWorktrees_ReturnsEmptyList()
    {
        var output = string.Join("\n", new[]
        {
            "worktree C:/repo",
            "HEAD 1111111111111111111111111111111111111111",
            "branch refs/heads/main",
            "",
        });

        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, output, ""));
        var service = new GitWorktreeService(runner);

        var worktrees = await service.ListWorktreesAsync("C:/repo");

        Assert.Empty(worktrees);
    }

    [Fact]
    public async Task ListWorktrees_WhenDetachedHead_ParsesCorrectly()
    {
        var output = string.Join("\n", new[]
        {
            "worktree C:/repo",
            "HEAD 1111111111111111111111111111111111111111",
            "branch refs/heads/main",
            "",
            "worktree C:/repo/../detached",
            "HEAD abcdefabcdefabcdefabcdefabcdefabcdefabcd",
            "detached",
            "",
        });

        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, output, ""));
        var service = new GitWorktreeService(runner);

        var worktrees = await service.ListWorktreesAsync("C:/repo");

        Assert.Single(worktrees);
        Assert.True(worktrees[0].IsDetached);
        Assert.Null(worktrees[0].Branch);
    }

    [Fact]
    public async Task ListWorktrees_WhenPathContainsSpaces_ParsesCorrectly()
    {
        var output = string.Join("\n", new[]
        {
            "worktree C:/repo",
            "HEAD 1111111111111111111111111111111111111111",
            "branch refs/heads/main",
            "",
            "worktree C:/Users/John Smith/feature_one",
            "HEAD 2222222222222222222222222222222222222222",
            "branch refs/heads/feature/one",
            "",
        });

        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, output, ""));
        var service = new GitWorktreeService(runner);

        var worktrees = await service.ListWorktreesAsync("C:/repo");

        Assert.Single(worktrees);
        Assert.Contains("John Smith", worktrees[0].Path);
    }

    [Fact]
    public async Task ListWorktrees_WhenPorcelainOutputHasCrLf_ParsesCorrectly()
    {
        var output = "worktree C:/repo\r\nHEAD 1111111111111111111111111111111111111111\r\nbranch refs/heads/main\r\n\r\n" +
                     "worktree C:/repo/../feature_one\r\nHEAD 2222222222222222222222222222222222222222\r\nbranch refs/heads/feature/one\r\n";

        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, output, ""));
        var service = new GitWorktreeService(runner);

        var worktrees = await service.ListWorktreesAsync("C:/repo");

        Assert.Single(worktrees);
    }

    [Fact]
    public async Task RemoveWorktree_RunsRemoveThenPrune()
    {
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, "", ""));
        runner.Enqueue(new ProcessResult(0, "", ""));
        var service = new GitWorktreeService(runner);

        var worktree = new WorktreeInfo("C:/repo/../feature_one", "feature/one", "", false, false);
        await service.RemoveWorktreeAsync("C:/repo", worktree);

        Assert.Equal(2, runner.Calls.Count);
        Assert.Contains("worktree remove", runner.Calls[0].Arguments);
        Assert.Contains("worktree prune", runner.Calls[1].Arguments);
    }

    [Fact]
    public async Task RemoveWorktree_WhenRemoveFails_ThrowsInvalidOperationException()
    {
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(1, "", "error"));
        var service = new GitWorktreeService(runner);
        var worktree = new WorktreeInfo("C:/repo/../feature_one", "feature/one", "", false, false);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RemoveWorktreeAsync("C:/repo", worktree));

        Assert.Contains("Failed to remove worktree", exception.Message);
    }

    [Fact]
    public async Task CreateWorktree_WhenGitFails_ThrowsInvalidOperationException()
    {
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(1, "", "fatal: branch already exists"));
        var service = new GitWorktreeService(runner);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateWorktreeAsync("C:/repo", "feature/one", "C:/repo/../feature_one"));

        Assert.Contains("Failed to create worktree", exception.Message);
    }
}
