namespace Smartctl.Core.Contracts;

public record DeviceStats(double ReadTb, double WrittenTb, int Errors);
