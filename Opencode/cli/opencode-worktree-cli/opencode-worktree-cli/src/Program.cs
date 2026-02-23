using System.CommandLine;
using OpencodeWorktreeCli.Commands;
using OpencodeWorktreeCli.Services;
using OpencodeWorktreeCli.Ui;

namespace OpencodeWorktreeCli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var processRunner = new ProcessRunner();
        var gitService = new GitWorktreeService(processRunner);
        var prompt = new InteractivePrompt();

        var root = new RootCommand("Create and manage git worktrees for opencode workflows.");
        root.Add(new VersionOption());
        root.Add(new NewCommand(gitService, prompt, processRunner).Command);
        root.Add(new CloseCommand(gitService, prompt).Command);

        var parseResult = root.Parse(args);
        return await parseResult.InvokeAsync();
    }
}
