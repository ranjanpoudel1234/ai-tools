using OpencodeWorktreeCli.Models;

namespace OpencodeWorktreeCli;

internal static class CliFormatting
{
    public static string Quote(string value) => $"\"{value.Replace("\"", "\\\"")}\"";

    public static string ShortSha(string sha) =>
        string.IsNullOrWhiteSpace(sha) ? "unknown" : sha[..Math.Min(8, sha.Length)];

    public static string GetError(ProcessResult result) =>
        !string.IsNullOrWhiteSpace(result.Error) ? result.Error : result.Output;
}
