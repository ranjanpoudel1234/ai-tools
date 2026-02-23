using OpencodeWorktreeCli.Models;
using Spectre.Console;

namespace OpencodeWorktreeCli.Ui;

public class InteractivePrompt : IInteractivePrompt
{
    public Task<string> PromptForBranchNameAsync()
    {
        EnsureInteractive();

        var prompt = new TextPrompt<string>("Branch name:")
            .Validate(value =>
                string.IsNullOrWhiteSpace(value)
                    ? ValidationResult.Error("Branch name is required.")
                    : ValidationResult.Success());

        var result = AnsiConsole.Prompt(prompt).Trim();
        return Task.FromResult(result);
    }

    public Task<WorktreeInfo> PickWorktreeAsync(IReadOnlyList<WorktreeInfo> worktrees)
    {
        EnsureInteractive();

        var prompt = new SelectionPrompt<WorktreeInfo>()
            .Title("Select a worktree to remove")
            .PageSize(10)
            .UseConverter(FormatWorktree);

        prompt.AddChoices(worktrees);

        var selection = AnsiConsole.Prompt(prompt);
        return Task.FromResult(selection);
    }

    public Task<bool> ConfirmAsync(string message)
    {
        EnsureInteractive();
        return Task.FromResult(AnsiConsole.Confirm(message, false));
    }

    private static string FormatWorktree(WorktreeInfo worktree)
    {
        var label = worktree.IsDetached
            ? $"detached HEAD at {CliFormatting.ShortSha(worktree.HeadSha)}"
            : worktree.Branch ?? "(unknown)";

        var displayPath = PathHelper.ToDisplayPath(worktree.Path);
        return $"{label} ({displayPath})";
    }

    private static void EnsureInteractive()
    {
        if (!AnsiConsole.Profile.Capabilities.Interactive)
            throw new InvalidOperationException("Interactive prompts require a TTY.");
    }
}
