using OpencodeWorktreeCli.Models;

namespace OpencodeWorktreeCli.Services;

/// <summary>
/// Abstraction over external process execution.
/// This is the single seam used for all git and shell calls.
/// Never call Process.Start directly outside of ProcessRunner.cs.
/// </summary>
public interface IProcessRunner
{
    Task<ProcessResult> RunAsync(string executable, string arguments, string? workingDirectory = null);
}
