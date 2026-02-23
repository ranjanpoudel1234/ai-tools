using OpencodeWorktreeCli.Models;
using OpencodeWorktreeCli.Services;
using OpencodeWorktreeCli.Ui;

namespace OpencodeWorktreeCli.Tests;

internal sealed class FakeProcessRunner : IProcessRunner
{
    private readonly Queue<ProcessResult> _results = new();
    public List<(string Executable, string Arguments, string? WorkingDirectory)> Calls { get; } = new();

    public void Enqueue(ProcessResult result) => _results.Enqueue(result);

    public Task<ProcessResult> RunAsync(string executable, string arguments, string? workingDirectory = null)
    {
        Calls.Add((executable, arguments, workingDirectory));
        return Task.FromResult(_results.Dequeue());
    }
}

internal sealed class FakeGitWorktreeService : IGitWorktreeService
{
    public string RepoRoot { get; set; } = string.Empty;
    public string? LastBranchName { get; private set; }
    public string? LastTargetPath { get; private set; }
    public WorktreeInfo? RemovedWorktree { get; private set; }
    public Exception? RemoveException { get; set; }
    public IReadOnlyList<WorktreeInfo> WorktreesToReturn { get; set; } = Array.Empty<WorktreeInfo>();

    public Task<string> GetRepoRootAsync(string workingDirectory) => Task.FromResult(RepoRoot);

    public Task<string> CreateWorktreeAsync(string repoRoot, string branchName, string targetPath)
    {
        LastBranchName = branchName;
        LastTargetPath = targetPath;
        return Task.FromResult(targetPath);
    }

    public Task<IReadOnlyList<WorktreeInfo>> ListWorktreesAsync(string repoRoot) =>
        Task.FromResult(WorktreesToReturn);

    public Task RemoveWorktreeAsync(string repoRoot, WorktreeInfo worktree)
    {
        if (RemoveException is not null)
            throw RemoveException;
        RemovedWorktree = worktree;
        return Task.CompletedTask;
    }
}

internal sealed class FakePrompt : IInteractivePrompt
{
    public string BranchName { get; set; } = "";
    public WorktreeInfo? WorktreeSelection { get; set; }
    public bool ConfirmResult { get; set; } = true;

    public Task<string> PromptForBranchNameAsync() => Task.FromResult(BranchName);

    public Task<WorktreeInfo> PickWorktreeAsync(IReadOnlyList<WorktreeInfo> worktrees) =>
        Task.FromResult(WorktreeSelection ?? worktrees[0]);

    public Task<bool> ConfirmAsync(string message) => Task.FromResult(ConfirmResult);
}

internal sealed class TempEnvVar : IDisposable
{
    private readonly string _name;
    private readonly string? _originalValue;

    public TempEnvVar(string name, string value)
    {
        _name = name;
        _originalValue = Environment.GetEnvironmentVariable(name);
        Environment.SetEnvironmentVariable(name, value);
    }

    public void Dispose() => Environment.SetEnvironmentVariable(_name, _originalValue);
}

internal static class ConsoleCapture
{
    private static readonly SemaphoreSlim ConsoleLock = new(1, 1);

    public static async Task<string> CaptureAsync(Func<Task> action)
    {
        await ConsoleLock.WaitAsync();
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        try
        {
            Console.SetOut(writer);
            await action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
            ConsoleLock.Release();
        }
    }
}
