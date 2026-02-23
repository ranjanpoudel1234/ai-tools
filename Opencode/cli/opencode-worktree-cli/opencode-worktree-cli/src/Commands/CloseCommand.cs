using System.CommandLine;
using OpencodeWorktreeCli.Models;
using OpencodeWorktreeCli.Services;
using OpencodeWorktreeCli.Ui;

namespace OpencodeWorktreeCli.Commands;

public class CloseCommand
{
    private readonly IGitWorktreeService _git;
    private readonly IInteractivePrompt _prompt;

    public CloseCommand(IGitWorktreeService git, IInteractivePrompt prompt)
    {
        _git = git;
        _prompt = prompt;
        Command = new Command("close", "Remove an existing git worktree.");
        Command.SetAction(_ => ExecuteAsync());
    }

    public Command Command { get; }

    public async Task ExecuteAsync()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var repoRoot = await _git.GetRepoRootAsync(currentDirectory);
        var worktrees = await _git.ListWorktreesAsync(repoRoot);

        if (worktrees.Count == 0)
        {
            Console.WriteLine("No linked worktrees found.");
            return;
        }

        var selected = await _prompt.PickWorktreeAsync(worktrees);
        var description = DescribeWorktree(selected);
        var confirm = await _prompt.ConfirmAsync($"Remove {description}?");

        if (!confirm)
        {
            Console.WriteLine("Cancelled.");
            return;
        }

        await _git.RemoveWorktreeAsync(repoRoot, selected);
        Console.WriteLine("Worktree removed.");
    }

    private static string DescribeWorktree(WorktreeInfo worktree)
    {
        var label = worktree.IsDetached
            ? $"detached HEAD at {CliFormatting.ShortSha(worktree.HeadSha)}"
            : worktree.Branch ?? "(unknown)";

        return $"'{label}' at '{PathHelper.ToDisplayPath(worktree.Path)}'";
    }
}
