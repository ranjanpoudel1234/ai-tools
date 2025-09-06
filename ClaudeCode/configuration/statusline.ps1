# Check for debug flag
$debugMode = $args -contains "--debug"

# Parse JSON
$input = [Console]::In.ReadToEnd()
$json = $input | ConvertFrom-Json

# Log input if debug mode is enabled
if ($debugMode) {
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logEntry = "[$timestamp] Input JSON: $input`n"
    $logPath = Join-Path (Split-Path $PSScriptRoot) ".claude\statusline.log"
    Add-Content -Path $logPath -Value $logEntry -Encoding UTF8
}

$MODEL_DISPLAY = $json.model.display_name
$CURRENT_DIR = $json.workspace.current_dir

# Show git branch if in a git repo
$GIT_BRANCH = ""

try {
    $gitDir = git rev-parse --git-dir 2>$null
    if ($LASTEXITCODE -eq 0) {
        $branch = git branch --show-current 2>$null
        if (![string]::IsNullOrWhiteSpace($branch)) {
            $GIT_BRANCH = $branch
        }
    }
} catch {
    # Silently handle git errors
}

$folderName = Split-Path $CURRENT_DIR -Leaf

# Get last user message from transcript file
$lastUserMessage = ""
if ($json.transcript_path) {
    $transcriptPath = $json.transcript_path
    
    if (Test-Path $transcriptPath) {
        try {
            # Read the last few lines and find the most recent user message
            $lines = Get-Content $transcriptPath -Tail 50
            for ($i = $lines.Length - 1; $i -ge 0; $i--) {
                try {
                    $entry = $lines[$i] | ConvertFrom-Json
                    if ($entry.type -eq "user" -and $entry.message -and $entry.message.role -eq "user" -and $entry.message.content) {
                        if ($entry.message.content -is [string]) {
                            $lastUserMessage = $entry.message.content
                        } elseif ($entry.message.content -is [array] -and $entry.message.content.Count -gt 0) {
                            if ($entry.message.content[0].text) {
                                $lastUserMessage = $entry.message.content[0].text
                            } elseif ($entry.message.content[0].content) {
                                $lastUserMessage = $entry.message.content[0].content
                            } else {
                                $lastUserMessage = $entry.message.content[0]
                            }
                        }
                        if ($lastUserMessage -and $lastUserMessage.Length -gt 150) {
                            $lastUserMessage = $lastUserMessage.Substring(0, 150) + "..."
                        }
                        break
                    }
                } catch {
                    # Skip malformed JSON lines
                    continue
                }
            }
        } catch {
            # Silently handle file read errors
        }
    }
}

# Direct console colors that work better with VS Code
[Console]::Write([char]27 + "[32m" + "[DIR] " + $folderName + [char]27 + "[0m")
[Console]::Write(" | ")
[Console]::Write([char]27 + "[33m" + "[GIT] " + $GIT_BRANCH + [char]27 + "[0m")
[Console]::Write(" | ")
[Console]::Write([char]27 + "[34m" + "[MODEL] " + $MODEL_DISPLAY + [char]27 + "[0m")
[Console]::Write(" | ")
[Console]::Write([char]27 + "[35m" + "[VER] v1.0.108" + [char]27 + "[0m")
[Console]::Write(" | ")
[Console]::Write([char]27 + "[36m" + "[STYLE] enterprise-dotnet" + [char]27 + "[0m")
[Console]::WriteLine("")
if ($lastUserMessage) {
    [Console]::WriteLine([char]27 + "[37m" + "--> " + $lastUserMessage + [char]27 + "[0m")
}