using OpencodeWorktreeCli.Models;

namespace OpencodeWorktreeCli.Ui;

/// <summary>
/// Interactive terminal prompts. Abstracted for testability.
/// </summary>
public interface IInteractivePrompt
{
    Task<string> PromptForBranchNameAsync();
    Task<WorktreeInfo> PickWorktreeAsync(IReadOnlyList<WorktreeInfo> worktrees);
    Task<bool> ConfirmAsync(string message);
}
