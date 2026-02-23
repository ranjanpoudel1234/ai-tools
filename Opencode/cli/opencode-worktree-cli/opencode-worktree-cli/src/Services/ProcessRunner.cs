using System.Diagnostics;
using OpencodeWorktreeCli.Models;

namespace OpencodeWorktreeCli.Services;

/// <summary>
/// Executes external processes and captures stdout/stderr.
/// This is the ONLY place in the codebase where Process.Start is called.
/// </summary>
public class ProcessRunner : IProcessRunner
{
    public async Task<ProcessResult> RunAsync(string executable, string arguments, string? workingDirectory = null)
    {
        var psi = new ProcessStartInfo
        {
            FileName = executable,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        if (workingDirectory is not null)
            psi.WorkingDirectory = workingDirectory;

        Process process;
        try
        {
            process = Process.Start(psi)
                ?? throw new InvalidOperationException($"Failed to start process: {executable}");
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            throw new InvalidOperationException($"Executable not found: '{executable}'. {ex.Message}", ex);
        }

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var output = await outputTask;
        var error = await errorTask;

        return new ProcessResult(process.ExitCode, output.Trim(), error.Trim());
    }
}
