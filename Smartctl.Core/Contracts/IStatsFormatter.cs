namespace Smartctl.Core.Contracts;

public interface IStatsFormatter
{
    string Format(PeriodDeviceStats stats, int precision);
}
