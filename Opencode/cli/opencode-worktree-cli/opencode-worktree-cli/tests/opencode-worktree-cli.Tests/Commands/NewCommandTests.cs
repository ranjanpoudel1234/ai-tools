using OpencodeWorktreeCli.Commands;
using OpencodeWorktreeCli.Models;
using Xunit;

namespace OpencodeWorktreeCli.Tests.Commands;

public class NewCommandTests
{
    [Fact]
    public async Task Execute_DerivesFolderName_ReplacesSlashWithUnderscore()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var git = new FakeGitWorktreeService { RepoRoot = repoRoot };
        var prompt = new FakePrompt { BranchName = "feature/my-work" };
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, "", ""));
        runner.Enqueue(new ProcessResult(0, "", ""));

        using var home = new TempEnvVar("HOME", tempRoot);

        var command = new NewCommand(git, prompt, runner);

        await ConsoleCapture.CaptureAsync(command.ExecuteAsync);

        Assert.Equal("feature/my-work", git.LastBranchName);
        var expectedTarget = Path.Combine(Directory.GetParent(repoRoot)!.FullName, "repo_feature_my-work");
        Assert.Equal(expectedTarget, git.LastTargetPath);

        var scriptPath = Path.Combine(tempRoot, ".local", "tmp", "opencode-worktree", "ocw_repo_feature_my-work.sh");
        Assert.True(File.Exists(scriptPath));
        var scriptBytes = File.ReadAllBytes(scriptPath);
        var script = File.ReadAllText(scriptPath);
        Assert.DoesNotContain("\r", script);
        Assert.Contains("&& opencode", script);
        Assert.EndsWith("\n", script);
        Assert.DoesNotContain("\\", script);
        Assert.False(scriptBytes.Length >= 3 && scriptBytes[0] == 0xEF && scriptBytes[1] == 0xBB && scriptBytes[2] == 0xBF);
    }

    [Fact]
    public async Task Execute_WritesBashWrapperScript_WithCorrectCdAndOpencode()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var git = new FakeGitWorktreeService { RepoRoot = repoRoot };
        var prompt = new FakePrompt { BranchName = "feature/one" };
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, "", ""));
        runner.Enqueue(new ProcessResult(0, "", ""));

        using var home = new TempEnvVar("HOME", tempRoot);

        var command = new NewCommand(git, prompt, runner);
        await ConsoleCapture.CaptureAsync(command.ExecuteAsync);

        var scriptPath = Path.Combine(tempRoot, ".local", "tmp", "opencode-worktree", "ocw_repo_feature_one.sh");
        var script = File.ReadAllText(scriptPath);

        Assert.Contains("cd \"", script);
        Assert.Contains("&& opencode", script);
    }

    [Fact]
    public async Task Execute_WhenOpencodeMissing_WritesCdOnlyScript()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var git = new FakeGitWorktreeService { RepoRoot = repoRoot };
        var prompt = new FakePrompt { BranchName = "feature/one" };
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, "", ""));
        runner.Enqueue(new ProcessResult(1, "", ""));
        runner.Enqueue(new ProcessResult(1, "", ""));

        using var home = new TempEnvVar("HOME", tempRoot);

        var command = new NewCommand(git, prompt, runner);
        await ConsoleCapture.CaptureAsync(command.ExecuteAsync);

        var scriptPath = Path.Combine(tempRoot, ".local", "tmp", "opencode-worktree", "ocw_repo_feature_one.sh");
        var script = File.ReadAllText(scriptPath);

        Assert.Contains("cd \"", script);
        Assert.DoesNotContain("&& opencode", script);

        var lastPathFile = Path.Combine(tempRoot, ".local", "tmp", "opencode-worktree", "last-script-path.txt");
        Assert.True(File.Exists(lastPathFile));
    }

    [Fact]
    public async Task Execute_WhenBranchNameInvalid_ThrowsInvalidOperationException()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var git = new FakeGitWorktreeService { RepoRoot = repoRoot };
        var prompt = new FakePrompt { BranchName = "invalid branch" };
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(1, "", "fatal: invalid"));

        using var home = new TempEnvVar("HOME", tempRoot);

        var command = new NewCommand(git, prompt, runner);

        await Assert.ThrowsAsync<InvalidOperationException>(() => command.ExecuteAsync());
    }

    [Fact]
    public async Task Execute_WhenBranchHasMultipleSlashes_ReplacesAllWithUnderscore()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var git = new FakeGitWorktreeService { RepoRoot = repoRoot };
        var prompt = new FakePrompt { BranchName = "feature/team/my-work" };
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, "", ""));
        runner.Enqueue(new ProcessResult(0, "", ""));

        using var home = new TempEnvVar("HOME", tempRoot);

        var command = new NewCommand(git, prompt, runner);
        await ConsoleCapture.CaptureAsync(command.ExecuteAsync);

        var expectedTarget = Path.Combine(Directory.GetParent(repoRoot)!.FullName, "repo_feature_team_my-work");
        Assert.Equal(expectedTarget, git.LastTargetPath);
    }

    [Fact]
    public async Task Execute_PrintsScriptPath_ToConsole()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var git = new FakeGitWorktreeService { RepoRoot = repoRoot };
        var prompt = new FakePrompt { BranchName = "feature/one" };
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, "", ""));
        runner.Enqueue(new ProcessResult(0, "", ""));

        using var home = new TempEnvVar("HOME", tempRoot);

        var command = new NewCommand(git, prompt, runner);
        var output = await ConsoleCapture.CaptureAsync(command.ExecuteAsync);

        var scriptPath = Path.Combine(tempRoot, ".local", "tmp", "opencode-worktree", "ocw_repo_feature_one.sh");
        var expected = PathHelper.ToDisplayPath(PathHelper.NormalizePath(scriptPath));
        Assert.Equal(expected, output.Trim());

        var lastPathFile = Path.Combine(tempRoot, ".local", "tmp", "opencode-worktree", "last-script-path.txt");
        Assert.True(File.Exists(lastPathFile));
        Assert.Equal(expected, File.ReadAllText(lastPathFile).Trim());
    }

    [Fact]
    public async Task Execute_WhenHomeIsMsysPath_WritesScriptUnderNormalizedHome()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        var drive = Path.GetPathRoot(tempRoot)![0];
        var remainder = tempRoot[2..].Replace('\\', '/');
        var msysHome = $"/{char.ToLowerInvariant(drive)}{remainder}";

        var git = new FakeGitWorktreeService { RepoRoot = repoRoot };
        var prompt = new FakePrompt { BranchName = "feature/one" };
        var runner = new FakeProcessRunner();
        runner.Enqueue(new ProcessResult(0, "", ""));
        runner.Enqueue(new ProcessResult(0, "", ""));

        using var home = new TempEnvVar("HOME", msysHome);

        var command = new NewCommand(git, prompt, runner);
        await ConsoleCapture.CaptureAsync(command.ExecuteAsync);

        var scriptPath = Path.Combine(tempRoot, ".local", "tmp", "opencode-worktree", "ocw_repo_feature_one.sh");
        Assert.True(File.Exists(scriptPath));
    }

    [Fact]
    public async Task Execute_WritesUniqueScriptNames_PerBranch()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var repoRoot = Path.Combine(tempRoot, "repo");
        Directory.CreateDirectory(repoRoot);

        using var home = new TempEnvVar("HOME", tempRoot);

        var git = new FakeGitWorktreeService { RepoRoot = repoRoot };

        var runnerOne = new FakeProcessRunner();
        runnerOne.Enqueue(new ProcessResult(0, "", ""));
        runnerOne.Enqueue(new ProcessResult(0, "", ""));
        var commandOne = new NewCommand(git, new FakePrompt { BranchName = "feature/one" }, runnerOne);
        await ConsoleCapture.CaptureAsync(commandOne.ExecuteAsync);

        var runnerTwo = new FakeProcessRunner();
        runnerTwo.Enqueue(new ProcessResult(0, "", ""));
        runnerTwo.Enqueue(new ProcessResult(0, "", ""));
        var commandTwo = new NewCommand(git, new FakePrompt { BranchName = "feature/two" }, runnerTwo);
        await ConsoleCapture.CaptureAsync(commandTwo.ExecuteAsync);

        var scriptOne = Path.Combine(tempRoot, ".local", "tmp", "opencode-worktree", "ocw_repo_feature_one.sh");
        var scriptTwo = Path.Combine(tempRoot, ".local", "tmp", "opencode-worktree", "ocw_repo_feature_two.sh");

        Assert.True(File.Exists(scriptOne));
        Assert.True(File.Exists(scriptTwo));
    }
}
