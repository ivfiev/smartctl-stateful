namespace Smartctl.Core.Contracts;

public record DeviceStats(ulong DataUnitsRead, ulong DataUnitsWritten, uint TotalErrors);

public record DeviceStatsArgs(string DeviceId);
