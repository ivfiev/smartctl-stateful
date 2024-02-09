using System.Text;

namespace Smartctl.Core.Formatters;

public class PlainTextFormatter : IStatsFormatter
{
    public string Format(PeriodDeviceStats stats, int precision)
    {
        var sb = new StringBuilder();

        sb.Append("=========== SMART/Health Information ===========\n");
        sb.Append($"{Pad("Total read:")}{Round(stats.ReadTb)} TB\n");
        sb.Append($"{Pad("Total written:")}{Round(stats.WrittenTb)} TB\n");
        sb.Append($"{Pad("Total errors:")}{stats.Errors}\n");

        foreach (var (days, avg) in stats.WrittenTbPerPeriod)
        {
            sb.Append($"{Pad($"Average written over the past {days} days:")}{Round(avg)} TB\n");
        }

        return sb.ToString();

        string Pad(string str)
        {
            return str.PadRight(40);
        }

        string Round(double d)
        {
            return Math.Round(d, precision).ToString($"F{precision}");
        }
    }
}
