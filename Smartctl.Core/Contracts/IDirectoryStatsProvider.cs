namespace Smartctl.Core.Contracts;

public interface IDirectoryStatsProvider
{
    DirectoryStats[] GetStats(string directory);
}
