using System.Text;
using Smartctl.Core.Contracts;

namespace Smartctl.Core.Formatters;

public class PlainTextDirectoryDiffFormatter : IDirectoryDiffFormatter
{
    public string Format(DirectoryStats[] stats)
    {
        var sb = new StringBuilder();
        var sorted = stats.Where(s => s.SizeGb != 0).OrderByDescending(s => Math.Abs(s.SizeGb)).ToArray();
        var padLen = stats.MaxBy(s => s.Path.Length)?.Path.Length + 4;

        sb.Append("====== Directory diffs ======\n");

        foreach (var (path, sizeGb) in sorted)
        {
            sb.Append($"{Pad(path)}{(sizeGb > 0 ? "+" : "")}{Round(sizeGb)} GB\n");
        }

        return sb.ToString();

        string Pad(string str)
        {
            return str.PadRight(padLen ?? 0);
        }

        string Round(double d)
        {
            return Math.Round(d, 6).ToString("0.########################");
        }
    }
}
