namespace Smartctl.Core.Contracts;

public interface IDeviceStatsFormatter
{
    string Format(PeriodDeviceStats stats, int precision);
}
