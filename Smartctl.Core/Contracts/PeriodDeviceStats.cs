namespace Smartctl.Core.Contracts;

public record PeriodDeviceStats(double ReadTb, double WrittenTb, IDictionary<int, double> WrittenTbPerPeriod, int Errors);
