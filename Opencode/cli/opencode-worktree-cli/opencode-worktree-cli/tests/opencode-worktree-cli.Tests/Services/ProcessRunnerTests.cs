using OpencodeWorktreeCli.Services;
using Xunit;

namespace OpencodeWorktreeCli.Tests.Services;

public class ProcessRunnerTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task RunAsync_WhenCommandSucceeds_ReturnsZeroExitCodeAndOutput()
    {
        var runner = new ProcessRunner();

        var result = await runner.RunAsync("dotnet", "--version");

        Assert.True(result.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(result.Output));
    }

    [Fact]
    public async Task RunAsync_WhenExecutableNotFound_ThrowsInvalidOperationException()
    {
        var runner = new ProcessRunner();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            runner.RunAsync("nonexistent-executable-12345", ""));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task RunAsync_WhenCommandFails_ReturnsNonZeroExitCode()
    {
        var runner = new ProcessRunner();

        var result = await runner.RunAsync("dotnet", "--definitely-not-a-valid-option");

        Assert.False(result.Succeeded);
        Assert.NotEqual(0, result.ExitCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task RunAsync_RespectsWorkingDirectory()
    {
        var runner = new ProcessRunner();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            var result = await runner.RunAsync("cmd", "/c cd", tempDir);

            Assert.True(result.Succeeded);
            Assert.Equal(PathHelper.NormalizePath(tempDir), PathHelper.NormalizePath(result.Output), ignoreCase: true);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}
