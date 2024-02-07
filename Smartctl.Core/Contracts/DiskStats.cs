namespace Smartctl.Core.Contracts;

public record DiskStats(ulong DataUnitsWritten, string Raw);

public record DiskStatsArgs(string Device);