How opencodework Works - Quick Summary
The Big Picture
You have a hybrid Windows/WSL setup where different parts run in different environments:
┌─────────────────────────────────────────────────────────┐
│ YOU (in WSL terminal)                                   │
│ Type: opencodework new                                  │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│ BASH WRAPPER FUNCTION (WSL)                             │
│ Intercepts "new" command                                │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│ .NET TOOL (Windows)                                     │
│ /mnt/c/Users/261906/.dotnet/tools/opencodework.exe      │
│                                                          │
│ 1. Prompts for branch name                              │
│ 2. Runs: git worktree add -b <branch> <path>           │
│ 3. Creates worktree at ../repo_branch                   │
│ 4. Writes bash script to:                               │
│    C:\Users\261906\.local\tmp\opencode-worktree\        │
│    ocw_repo_branch.sh                                   │
│    Content: cd "C:/Projects/ERO/repo_branch"            │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│ BASH WRAPPER FUNCTION (WSL) - Part 2                    │
│                                                          │
│ 1. Reads: last-script-path.txt                          │
│ 2. Converts: C:/ → /mnt/c/                              │
│ 3. Reads script, converts paths inside                  │
│ 4. Executes: cd "/mnt/c/Projects/ERO/repo_branch"       │
│ 5. Runs: opencode (from WSL)                            │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│ RESULT                                                   │
│ ✅ You're in the new worktree                           │
│ ✅ OpenCode is running                                  │
└─────────────────────────────────────────────────────────┘
---
The Key Components
1. The .NET Tool (Windows)
- Location: C:\Users\261906\.dotnet\tools\opencodework.exe
- Language: C# / .NET 10
- What it does: 
  - Creates git worktrees
  - Generates bash scripts
  - Writes to Windows filesystem
- Limitation: Can't see WSL executables like opencode
2. The Bash Wrapper (WSL)
- Location: Two parts:
  - /mnt/c/Users/261906/.dotnet/tools/opencodework (executable wrapper)
  - Function in ~/.bashrc (the smart wrapper)
- What it does:
  - Calls the Windows .NET tool
  - Converts Windows paths to WSL paths
  - CD's into the worktree
  - Launches opencode from WSL
3. The Path Conversion
This is the magic that bridges Windows ↔ WSL:
# Windows format (what .NET tool writes):
C:/Projects/ERO/vehicle-services-service_feature_login
# WSL format (what bash needs):
/mnt/c/Projects/ERO/vehicle-services-service_feature_login
# Conversion done by:
sed 's|C:/|/mnt/c/|g'
---
The Wrapper Function (Simplified)
Located in: ~/.bashrc
opencodework() {
  if [ "$1" = "new" ]; then
    # Step 1: Call the Windows .NET tool
    command opencodework new
    
    # Step 2: Read the script path it created
    script_path=$(cat last-script-path.txt)
    
    # Step 3: Convert Windows path to WSL path
    script_path=$(echo "$script_path" | sed 's|C:/|/mnt/c/|g')
    
    # Step 4: Read script content and convert paths
    script_content=$(sed 's|C:/|/mnt/c/|g' "$script_path")
    
    # Step 5: Execute the cd command
    eval "$script_content"  # This does: cd "/mnt/c/Projects/ERO/repo_branch"
    
    # Step 6: Launch opencode from WSL
    opencode
  fi
}
---
Why This Design?
The Problem
- You need: Git worktrees + Auto-cd + Launch opencode
- Challenge: Windows .NET tool can't see WSL's opencode
- Old approach: Tool tries to find opencode in Windows PATH (fails)
The Solution
Divide and conquer:
1. Let Windows .NET tool do what it's good at: create worktrees
2. Let WSL bash do what it's good at: cd and launch WSL executables
3. Bridge them with path conversion
Why Not Just Rewrite in Bash?
- .NET tool already exists and works well
- Has nice features (validation, interactive prompts, etc.)
- Easier to add a wrapper than rebuild from scratch
---
File Locations
Windows Side
C:\Users\261906\.dotnet\tools\
├── opencodework.exe          ← .NET tool
└── opencodework              ← Bash wrapper script
C:\Users\261906\.local\tmp\opencode-worktree\
├── last-script-path.txt      ← Points to latest script
└── ocw_repo_branch.sh        ← Generated cd scripts
WSL Side
/home/ranjan-unix/
└── .bashrc                   ← Contains wrapper function
/home/ranjan-unix/.opencode/bin/
└── opencode                  ← Your OpenCode installation
---
What Happens Step-by-Step
You Run: opencodework new
Step 1: Bash wrapper function activates
opencodework() function in ~/.bashrc intercepts
Step 2: Calls Windows .NET tool
command opencodework new
  ↓
Executes: /mnt/c/Users/261906/.dotnet/tools/opencodework.exe new
Step 3: .NET tool creates worktree
Prompts: Branch name: feature/login
Runs: git worktree add -b feature/login C:/Projects/ERO/repo_feature_login
Creates worktree folder
Step 4: .NET tool writes script
Writes to: C:\Users\261906\.local\tmp\opencode-worktree\ocw_repo_feature_login.sh
Content: cd "C:/Projects/ERO/repo_feature_login"
Updates: last-script-path.txt
Step 5: Bash wrapper reads and converts
Reads: last-script-path.txt → "C:/Users/261906/.local/.../ocw_repo_feature_login.sh"
Converts to: /mnt/c/Users/261906/.local/.../ocw_repo_feature_login.sh
Reads script content: cd "C:/Projects/ERO/repo_feature_login"
Converts to: cd "/mnt/c/Projects/ERO/repo_feature_login"
Step 6: Bash wrapper executes
eval 'cd "/mnt/c/Projects/ERO/repo_feature_login"'
  ↓
You're now in the worktree!
Step 7: Bash wrapper launches opencode
Checks: command -v opencode (found in WSL)
Executes: opencode
  ↓
OpenCode opens in your worktree!
---
The Genius Part
The wrapper function runs after the .NET tool completes, so:
1. ✅ .NET tool doesn't need to know about WSL
2. ✅ WSL bash doesn't need to reimplement git worktree logic
3. ✅ Path conversion happens transparently
4. ✅ Each environment does what it's best at
---
One-Line Summary
Windows .NET tool creates worktrees → WSL bash wrapper translates paths and launches opencode → You get auto-cd + auto-launch in one command! 🎉How opencodework Works - Quick Summary
The Big Picture
You have a hybrid Windows/WSL setup where different parts run in different environments:
┌─────────────────────────────────────────────────────────┐
│ YOU (in WSL terminal)                                   │
│ Type: opencodework new                                  │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│ BASH WRAPPER FUNCTION (WSL)                             │
│ Intercepts "new" command                                │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│ .NET TOOL (Windows)                                     │
│ /mnt/c/Users/261906/.dotnet/tools/opencodework.exe      │
│                                                          │
│ 1. Prompts for branch name                              │
│ 2. Runs: git worktree add -b <branch> <path>           │
│ 3. Creates worktree at ../repo_branch                   │
│ 4. Writes bash script to:                               │
│    C:\Users\261906\.local\tmp\opencode-worktree\        │
│    ocw_repo_branch.sh                                   │
│    Content: cd "C:/Projects/ERO/repo_branch"            │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│ BASH WRAPPER FUNCTION (WSL) - Part 2                    │
│                                                          │
│ 1. Reads: last-script-path.txt                          │
│ 2. Converts: C:/ → /mnt/c/                              │
│ 3. Reads script, converts paths inside                  │
│ 4. Executes: cd "/mnt/c/Projects/ERO/repo_branch"       │
│ 5. Runs: opencode (from WSL)                            │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│ RESULT                                                   │
│ ✅ You're in the new worktree                           │
│ ✅ OpenCode is running                                  │
└─────────────────────────────────────────────────────────┘
---
The Key Components
1. The .NET Tool (Windows)
- Location: C:\Users\261906\.dotnet\tools\opencodework.exe
- Language: C# / .NET 10
- What it does: 
  - Creates git worktrees
  - Generates bash scripts
  - Writes to Windows filesystem
- Limitation: Can't see WSL executables like opencode
2. The Bash Wrapper (WSL)
- Location: Two parts:
  - /mnt/c/Users/261906/.dotnet/tools/opencodework (executable wrapper)
  - Function in ~/.bashrc (the smart wrapper)
- What it does:
  - Calls the Windows .NET tool
  - Converts Windows paths to WSL paths
  - CD's into the worktree
  - Launches opencode from WSL
3. The Path Conversion
This is the magic that bridges Windows ↔ WSL:
# Windows format (what .NET tool writes):
C:/Projects/ERO/vehicle-services-service_feature_login
# WSL format (what bash needs):
/mnt/c/Projects/ERO/vehicle-services-service_feature_login
# Conversion done by:
sed 's|C:/|/mnt/c/|g'
---
The Wrapper Function (Simplified)
Located in: ~/.bashrc
opencodework() {
  if [ "$1" = "new" ]; then
    # Step 1: Call the Windows .NET tool
    command opencodework new
    
    # Step 2: Read the script path it created
    script_path=$(cat last-script-path.txt)
    
    # Step 3: Convert Windows path to WSL path
    script_path=$(echo "$script_path" | sed 's|C:/|/mnt/c/|g')
    
    # Step 4: Read script content and convert paths
    script_content=$(sed 's|C:/|/mnt/c/|g' "$script_path")
    
    # Step 5: Execute the cd command
    eval "$script_content"  # This does: cd "/mnt/c/Projects/ERO/repo_branch"
    
    # Step 6: Launch opencode from WSL
    opencode
  fi
}
---
Why This Design?
The Problem
- You need: Git worktrees + Auto-cd + Launch opencode
- Challenge: Windows .NET tool can't see WSL's opencode
- Old approach: Tool tries to find opencode in Windows PATH (fails)
The Solution
Divide and conquer:
1. Let Windows .NET tool do what it's good at: create worktrees
2. Let WSL bash do what it's good at: cd and launch WSL executables
3. Bridge them with path conversion
Why Not Just Rewrite in Bash?
- .NET tool already exists and works well
- Has nice features (validation, interactive prompts, etc.)
- Easier to add a wrapper than rebuild from scratch
---
File Locations
Windows Side
C:\Users\261906\.dotnet\tools\
├── opencodework.exe          ← .NET tool
└── opencodework              ← Bash wrapper script
C:\Users\261906\.local\tmp\opencode-worktree\
├── last-script-path.txt      ← Points to latest script
└── ocw_repo_branch.sh        ← Generated cd scripts
WSL Side
/home/ranjan-unix/
└── .bashrc                   ← Contains wrapper function
/home/ranjan-unix/.opencode/bin/
└── opencode                  ← Your OpenCode installation
---
What Happens Step-by-Step
You Run: opencodework new
Step 1: Bash wrapper function activates
opencodework() function in ~/.bashrc intercepts
Step 2: Calls Windows .NET tool
command opencodework new
  ↓
Executes: /mnt/c/Users/261906/.dotnet/tools/opencodework.exe new
Step 3: .NET tool creates worktree
Prompts: Branch name: feature/login
Runs: git worktree add -b feature/login C:/Projects/ERO/repo_feature_login
Creates worktree folder
Step 4: .NET tool writes script
Writes to: C:\Users\261906\.local\tmp\opencode-worktree\ocw_repo_feature_login.sh
Content: cd "C:/Projects/ERO/repo_feature_login"
Updates: last-script-path.txt
Step 5: Bash wrapper reads and converts
Reads: last-script-path.txt → "C:/Users/261906/.local/.../ocw_repo_feature_login.sh"
Converts to: /mnt/c/Users/261906/.local/.../ocw_repo_feature_login.sh
Reads script content: cd "C:/Projects/ERO/repo_feature_login"
Converts to: cd "/mnt/c/Projects/ERO/repo_feature_login"
Step 6: Bash wrapper executes
eval 'cd "/mnt/c/Projects/ERO/repo_feature_login"'
  ↓
You're now in the worktree!
Step 7: Bash wrapper launches opencode
Checks: command -v opencode (found in WSL)
Executes: opencode
  ↓
OpenCode opens in your worktree!
---
The Genius Part
The wrapper function runs after the .NET tool completes, so:
1. ✅ .NET tool doesn't need to know about WSL
2. ✅ WSL bash doesn't need to reimplement git worktree logic
3. ✅ Path conversion happens transparently
4. ✅ Each environment does what it's best at
---
One-Line Summary
Windows .NET tool creates worktrees → WSL bash wrapper translates paths and launches opencode → You get auto-cd + auto-launch in one command! 🎉