namespace Smartctl.Core.Formatters;

public interface IStatsFormatter
{
    string Format(PeriodDeviceStats stats, int precision);
}
