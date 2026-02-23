namespace OpencodeWorktreeCli;

/// <summary>
/// Path utilities for Windows / Git Bash compatibility.
/// </summary>
public static class PathHelper
{
    /// <summary>
    /// Converts a Windows path to forward-slash format for display in Git Bash context.
    /// </summary>
    public static string ToDisplayPath(string path) => path.Replace('\\', '/');

    /// <summary>
    /// Normalizes a path to its full absolute form without trailing separators.
    /// </summary>
    public static string NormalizePath(string path)
    {
        var normalized = NormalizeMsysPath(path);
        return Path.GetFullPath(normalized)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Resolves the home directory using the HOME environment variable first,
    /// then falls back to the Windows user profile. Never uses ~ in .NET path strings.
    /// </summary>
    public static string ResolveHome()
    {
        var home = Environment.GetEnvironmentVariable("HOME")
                   ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        return NormalizePath(home);
    }

    /// <summary>
    /// Derives the worktree folder name from a branch name by replacing '/' with '_'.
    /// e.g. "feature/my-work" -> "feature_my-work"
    /// </summary>
    public static string BranchToFolderName(string branchName) =>
        branchName.Replace('/', '_');

    private static string NormalizeMsysPath(string path)
    {
        if (path.Length >= 3 && path[0] == '/' && char.IsLetter(path[1]) && path[2] == '/')
        {
            var drive = char.ToUpperInvariant(path[1]);
            var remainder = path[3..].Replace('/', Path.DirectorySeparatorChar);
            return $"{drive}:{Path.DirectorySeparatorChar}{remainder}";
        }

        return path;
    }
}
