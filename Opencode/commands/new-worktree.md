---
description: Create a new git worktree with a new branch for parallel feature development
---

Create a new git worktree with a new branch. Ask the user for:
1. The branch name (e.g., feature/my-feature, bugfix/issue-123)
2. The base branch to branch from (default: main)

Then execute the following steps:

1. Determine the worktree directory path:
   - Take the branch name and replace `/` with `-`
   - Ask for the project name. If the comamnd is running within a specific project, confirm if that is the project user wants to use for creating worktree
   - Create path: `../{project-name}-{sanitized-branch-name}`
   - Example: `feature/my-feature` becomes `../{project-name}-feature-my-feature`

2. Run the git worktree command:
   ```bash
   git worktree add -b "{branch-name}" "{worktree-path}" {base-branch}
   ```

3. After successful creation, inform the user:
   - The worktree location
   - How to navigate to it: `cd {worktree-path}`
   - How to list all worktrees: `git worktree list`
   - How to remove it later: `git worktree remove {worktree-path}`

4. If the user wants to switch to that directory, ask if they'd like you to help them understand what to do next.

5. Automatically open the new worktree directory in the Visual Studio. If not available, ask which editor they would like to open it on.

Important notes:
- Worktrees share the same git repository but allow working on different branches simultaneously
- Each worktree has its own working directory
- Changes in one worktree don't affect others until merged
- The worktree is created in a sibling directory to keep things organized

 Example usage:

  You: /new-worktree
  Claude: What branch name would you like to create?
  You: feature/add-dashboard
  Claude: What base branch should I branch from? (default: main)
  You: main
  Claude: [Creates worktree at ../rome-repairorder-service-feature-add-dashboard]

  When you're done with a worktree, you can remove it with:
  git worktree remove ../rome-repairorder-service-feature-add-dashboard

  Try it out! Just type /new-worktree to get started.
