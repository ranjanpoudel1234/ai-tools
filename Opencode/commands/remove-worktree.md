---
description: Remove a git worktree and clean up associated branches and remote PRs
---

Remove a git worktree safely with proper cleanup. This command will:
1. List all available worktrees
2. Ask for the worktree name/path (if not provided)
3. Check for uncommitted changes
4. Check for associated remote branches and PRs
5. Ask for confirmation before deletion
6. Remove the worktree and clean up

## Steps to Execute:

1. **List all available worktrees:**
   ```bash
   git worktree list
   ```
   Display the output to help the user identify which worktree to remove.

2. **Ask for the worktree to remove:**
   - If not provided as an argument, ask: "Which worktree would you like to remove?"
   - Accept either the path or the branch name
   - If a branch name is provided, resolve it to the worktree path

3. **Validate the worktree exists:**
   - Check if the worktree path exists in the `git worktree list` output
   - If not found, inform the user and exit

4. **Check for uncommitted changes:**
   ```bash
   cd {worktree-path} && git status --porcelain
   ```
   - If there are uncommitted changes, warn the user:
     - "⚠️ This worktree has uncommitted changes. Are you sure you want to remove it?"
     - List the changes for visibility
     - Ask for explicit confirmation

5. **Check for remote branch and PRs:**
   - Get the branch name from the worktree
   - Check if a remote branch exists:
     ```bash
     git ls-remote --heads origin {branch-name}
     ```
   - If remote branch exists, check for associated PRs:
     ```bash
     gh pr list --head {branch-name}
     ```
   - If there's a remote branch or PR, inform the user:
     - "🔗 This worktree has a remote branch/PR associated with it:"
     - Display PR details if found
     - Ask: "Do you want to:"
       - "1. Remove worktree only (keep remote branch/PR)"
       - "2. Remove worktree and delete remote branch (will close PR if exists)"
       - "3. Cancel operation"

6. **Ask for final confirmation:**
   - "Are you sure you want to remove the worktree at {worktree-path}?"
   - Wait for explicit confirmation (yes/no)

7. **Remove the worktree:**
   ```bash
   git worktree remove {worktree-path}
   ```
   - If the removal fails with "contains modified or untracked files", ask if they want to force removal:
     ```bash
     git worktree remove --force {worktree-path}
     ```

8. **Clean up the branch (if confirmed):**
   - Delete the local branch:
     ```bash
     git branch -D {branch-name}
     ```
   - If user chose to delete remote branch:
     ```bash
     git push origin --delete {branch-name}
     ```

9. **Prune worktree metadata:**
   ```bash
   git worktree prune
   ```

10. **Confirm completion:**
    - "✅ Worktree removed successfully!"
    - "📍 Remaining worktrees:"
    - Show `git worktree list` output
    - If branch was deleted, confirm: "🗑️ Branch {branch-name} deleted (local and remote)"

## Important Notes:

- **Safety First**: Multiple confirmation points prevent accidental deletion
- **Remote Awareness**: Checks for remote branches and PRs before deletion
- **Uncommitted Changes**: Warns about uncommitted work
- **Clean State**: Properly prunes worktree metadata after removal
- **Shared Repository**: Removing a worktree doesn't affect other worktrees

## Example Usage:

### With worktree path provided:
```
You: /remove-worktree ../my-service-feature-old
Claude: [Lists worktrees, validates, checks remote, asks for confirmation, removes]
```

### Without argument (interactive):
```
You: /remove-worktree
Claude: Here are your current worktrees:
        [Lists all worktrees]
        Which worktree would you like to remove?
You: feature/old-feature
Claude: Found worktree at ../my-service-feature-old-feature
        ⚠️ This worktree has uncommitted changes:
        M  src/file.js
        ?? new-file.js
        Are you sure you want to remove it? (yes/no)
You: yes
Claude: 🔗 This worktree has a remote branch with an open PR:
        PR #123: "Add old feature" (open)
        Do you want to:
        1. Remove worktree only (keep remote branch/PR)
        2. Remove worktree and delete remote branch (will close PR)
        3. Cancel operation
You: 1
Claude: Are you sure you want to remove the worktree at ../my-service-feature-old-feature? (yes/no)
You: yes
Claude: ✅ Worktree removed successfully!
        🗑️ Local branch 'feature/old-feature' deleted
        ℹ️ Remote branch and PR preserved
```

## Troubleshooting:

- **"contains modified or untracked files"**: Use force removal after confirmation
- **"is main working tree"**: Cannot remove the main repository directory
- **Remote branch still exists**: Intentional if user chose option 1
- **PR still open**: PRs remain open when only worktree is removed

Try it out! Just type `/remove-worktree` to get started.
