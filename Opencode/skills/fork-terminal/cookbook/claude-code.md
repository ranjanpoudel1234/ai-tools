# Purpose

Create a new Claude code agent to execute the command.

## Variables

DEFAULT_MODEL: opus
HEAVY_MODEL: opus
BASE_MODEL: sonnet
FAST_MODEL: haiku
CONTEXT_FILE: C:\Users\261906\.claude\skills\fork-terminal\temp-fork-context.md

## Instructions

### For Forks WITH Conversation Context/Summary:

1. **Create Context File**: Write conversation history to `C:\Users\261906\.claude\skills\fork-terminal\temp-fork-context.md`
   - Use YAML format with conversation summary
   - Include current status and next steps
   - Overwrite this file each time (keeps context fresh)

2. **Launch Interactive Session**: Fork terminal with basic command (NO -p parameter)
   ```
   claude --model <MODEL> --dangerously-skip-permissions
   ```
   - Do NOT use `-p` parameter (avoids Windows batch escaping issues)
   - Model: DEFAULT_MODEL unless user specifies 'fast' (FAST_MODEL) or 'heavy' (HEAVY_MODEL)

3. **Provide Copy/Paste Instruction**: Tell user to paste this in the new terminal once it loads:
   ```
   Read C:\Users\261906\.claude\skills\fork-terminal\temp-fork-context.md for the full conversation history, then help me continue working.
   ```

### For Forks WITHOUT Context (Simple Forks):

- Launch with: `claude --model <MODEL> --dangerously-skip-permissions`
- Use interactive mode (no -p parameter)
- No context file needed
