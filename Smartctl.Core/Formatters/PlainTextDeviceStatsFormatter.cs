using System.Text;
using Smartctl.Core.Contracts;

namespace Smartctl.Core.Formatters;

public class PlainTextDeviceStatsFormatter : IDeviceStatsFormatter
{
    public string Format(PeriodDeviceStats stats, int precision)
    {
        var sb = new StringBuilder();

        sb.Append("=== SMART/Health Information ===\n");
        sb.Append($"{Pad("Reads:")}{Round(stats.ReadTb)} TB\n");
        sb.Append($"{Pad("Writes:")}{Round(stats.WrittenTb)} TB\n");
        sb.Append($"{Pad("Errors:")}{stats.Errors}\n");

        foreach (var (days, avg) in stats.WrittenTbPerPeriod)
        {
            sb.Append($"{Pad($"Writes ({days} days):")}{Round(avg)} TB\n");
        }

        return sb.ToString();

        string Pad(string str)
        {
            return str.PadRight(20);
        }

        string Round(double d)
        {
            return Math.Round(d, precision).ToString($"F{precision}");
        }
    }
}
