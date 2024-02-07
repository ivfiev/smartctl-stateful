namespace Smartctl.Core.Contracts;

public interface IDiskStatsProvider
{
    DiskStats GetDiskStats(DiskStatsArgs args);
}