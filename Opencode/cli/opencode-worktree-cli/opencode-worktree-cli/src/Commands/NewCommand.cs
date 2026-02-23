using System.CommandLine;
using System.Text;
using OpencodeWorktreeCli.Services;
using OpencodeWorktreeCli.Ui;

namespace OpencodeWorktreeCli.Commands;

public class NewCommand
{
    private readonly IGitWorktreeService _git;
    private readonly IInteractivePrompt _prompt;
    private readonly IProcessRunner _runner;

    public NewCommand(IGitWorktreeService git, IInteractivePrompt prompt, IProcessRunner runner)
    {
        _git = git;
        _prompt = prompt;
        _runner = runner;
        Command = new Command("new", "Create a new git worktree.");
        Command.SetAction(_ => ExecuteAsync());
    }

    public Command Command { get; }

    public async Task ExecuteAsync()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var repoRoot = await _git.GetRepoRootAsync(currentDirectory);

        var branchName = await _prompt.PromptForBranchNameAsync();
        await ValidateBranchNameAsync(repoRoot, branchName);

        var repoFolderName = Path.GetFileName(repoRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (string.IsNullOrWhiteSpace(repoFolderName))
            throw new InvalidOperationException("Unable to determine repo folder name.");

        var folderName = $"{repoFolderName}_{PathHelper.BranchToFolderName(branchName)}";
        var parentDirectory = Directory.GetParent(repoRoot)?.FullName
            ?? throw new InvalidOperationException("Unable to determine repo parent directory.");
        var targetPath = Path.Combine(parentDirectory, folderName);

        var worktreePath = await _git.CreateWorktreeAsync(repoRoot, branchName, targetPath);
        var opencodeAvailable = await CheckOpencodeAvailableAsync();

        if (!opencodeAvailable)
            Console.Error.WriteLine("[warn] 'opencode' not found in PATH. Wrapper will only change directories.");

        var home = PathHelper.ResolveHome();
        var scriptPath = await WriteWrapperScriptAsync(worktreePath, folderName, opencodeAvailable, home);
        var displayScriptPath = PathHelper.ToDisplayPath(PathHelper.NormalizePath(scriptPath));
        var displayScriptPathWithTilde = ToDisplayPathWithTilde(scriptPath, home);
        await WriteLastScriptPathAsync(displayScriptPath, home);

        Console.Error.WriteLine($"Worktree created at {PathHelper.ToDisplayPath(worktreePath)}");
        Console.Error.WriteLine($"Run: source {displayScriptPathWithTilde}");
        Console.WriteLine(displayScriptPath);
    }

    private async Task ValidateBranchNameAsync(string repoRoot, string branchName)
    {
        var result = await _runner.RunAsync("git", $"check-ref-format --branch {CliFormatting.Quote(branchName)}", repoRoot);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Invalid branch name '{branchName}'. {CliFormatting.GetError(result)}");
    }

    private async Task<bool> CheckOpencodeAvailableAsync()
    {
        var commands = OperatingSystem.IsWindows()
            ? new[] { "where", "which" }
            : new[] { "which" };

        foreach (var command in commands)
        {
            try
            {
                var result = await _runner.RunAsync(command, "opencode");
                if (result.Succeeded)
                    return true;
            }
            catch (InvalidOperationException)
            {
            }
        }

        return false;
    }

    private static async Task<string> WriteWrapperScriptAsync(
        string worktreePath,
        string folderName,
        bool opencodeAvailable,
        string homePath)
    {
        var scriptDirectory = Path.Combine(homePath, ".local", "tmp", "opencode-worktree");
        Directory.CreateDirectory(scriptDirectory);

        var safeName = SanitizeFileName(folderName);
        var scriptPath = Path.Combine(scriptDirectory, $"ocw_{safeName}.sh");
        var displayPath = PathHelper.ToDisplayPath(PathHelper.NormalizePath(worktreePath));

        var commandSuffix = opencodeAvailable ? " && opencode" : string.Empty;
        var scriptContent = $"cd \"{displayPath}\"{commandSuffix}\n";
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        await File.WriteAllTextAsync(scriptPath, scriptContent, encoding);

        return scriptPath;
    }

    private static async Task WriteLastScriptPathAsync(string displayScriptPath, string homePath)
    {
        var scriptDirectory = Path.Combine(homePath, ".local", "tmp", "opencode-worktree");
        Directory.CreateDirectory(scriptDirectory);

        var pathFile = Path.Combine(scriptDirectory, "last-script-path.txt");
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        await File.WriteAllTextAsync(pathFile, displayScriptPath + "\n", encoding);
    }

    private static string ToDisplayPathWithTilde(string path, string home)
    {
        var displayPath = PathHelper.ToDisplayPath(PathHelper.NormalizePath(path));
        var displayHome = PathHelper.ToDisplayPath(PathHelper.NormalizePath(home));

        if (displayPath.StartsWith(displayHome, StringComparison.OrdinalIgnoreCase))
        {
            var remainder = displayPath[displayHome.Length..];
            if (remainder.Length == 0)
                return "~";

            if (remainder.StartsWith("/", StringComparison.Ordinal))
                return "~" + remainder;
        }

        return displayPath;
    }

    private static string SanitizeFileName(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return new string(value.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
    }
}
