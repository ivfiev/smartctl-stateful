using System.Text;
using Smartctl.Core.Contracts;

namespace Smartctl.Core.Formatters;

public class PlainTextDirectoryDiffFormatter : IDirectoryDiffFormatter
{
    public string Format(DirectoryStats[] stats)
    {
        var sb = new StringBuilder();
        var sorted = stats.Where(s => s.SizeKb != 0).OrderByDescending(s => Math.Abs(s.SizeKb)).ToArray();
        var padLen = stats.MaxBy(s => s.Path.Length)?.Path.Length + 4;

        sb.Append("====== Directory diffs ======\n");

        foreach (var (path, sizeKb) in sorted)
        {
            sb.Append($"{Pad(path)}{(sizeKb > 0 ? "+" : "")}{GetValue(sizeKb)} MB\n");
        }

        return sb.ToString();

        string Pad(string str)
        {
            return str.PadRight(padLen ?? 0);
        }

        string GetValue(long kb)
        {
            return $"{kb / 1000.0 * 1.024:n3}";
        }
    }
}
