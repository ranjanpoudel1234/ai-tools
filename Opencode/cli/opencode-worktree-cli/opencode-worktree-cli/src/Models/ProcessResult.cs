namespace OpencodeWorktreeCli.Models;

/// <summary>
/// Result of running an external process via IProcessRunner.
/// </summary>
public record ProcessResult(int ExitCode, string Output, string Error)
{
    public bool Succeeded => ExitCode == 0;
}
