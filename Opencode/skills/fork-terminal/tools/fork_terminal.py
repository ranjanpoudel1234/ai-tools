#!/usr/bin/env -S uv run
"""Fork a new terminal window with a command."""

import os
import platform
import subprocess
import tempfile
import time


def fork_terminal(command: str) -> str:
    """Open a new Terminal window and run the specified command."""
    system = platform.system()
    cwd = os.getcwd()

    if system == "Darwin":  # macOS
        # Build shell command - use single quotes for cd to avoid escaping issues
        # Then escape everything for AppleScript
        shell_command = f"cd '{cwd}' && {command}"
        # Escape for AppleScript: backslashes first, then quotes
        escaped_shell_command = shell_command.replace("\\", "\\\\").replace('"', '\\"')

        try:
            result = subprocess.run(
                ["osascript", "-e", f'tell application "Terminal" to do script "{escaped_shell_command}"'],
                capture_output=True,
                text=True,
            )
            output = f"stdout: {result.stdout.strip()}\nstderr: {result.stderr.strip()}\nreturn_code: {result.returncode}"
            return output
        except Exception as e:
            return f"Error: {str(e)}"

    elif system == "Windows":
        # Use /d flag to change drives if necessary
        # For complex commands with quotes, use a temporary batch file
        # This avoids quote-escaping issues with nested cmd shells

        # Create a temporary batch file
        fd, bat_path = tempfile.mkstemp(suffix='.bat', text=True)
        try:
            with os.fdopen(fd, 'w') as bat_file:
                # Write batch file content
                bat_file.write('@echo off\n')
                bat_file.write(f'cd /d "{cwd}"\n')
                bat_file.write(f'{command}\n')
                # Keep window open if command fails
                bat_file.write('if errorlevel 1 pause\n')

            # Launch new terminal and execute the batch file
            # 'start' is a cmd.exe internal command, so we need shell=True
            # Use string format with proper quoting for the batch file path
            # Properly detach the subprocess to avoid freezing the current terminal
            subprocess.Popen(
                f'start "Claude Code Terminal" cmd /k "{bat_path}"',
                shell=True,
                stdin=subprocess.DEVNULL,
                stdout=subprocess.DEVNULL,
                stderr=subprocess.DEVNULL,
                close_fds=True
            )

            # Give it a moment to start before returning
            time.sleep(0.5)
            return f"Windows terminal launched (using temp batch file)"

        except Exception as e:
            # Clean up on error
            try:
                os.unlink(bat_path)
            except:
                pass
            return f"Error: {str(e)}"

    else:  # Linux and others
        raise NotImplementedError(f"Platform {system} not supported")


if __name__ == "__main__":
    import sys
    if len(sys.argv) > 1:
        output = fork_terminal(" ".join(sys.argv[1:]))
        print(output)
