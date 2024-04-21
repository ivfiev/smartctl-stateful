namespace Smartctl.Core.Contracts;

public interface IDirectoryDiffFormatter
{
    string Format(DirectoryStats[] stats);
}
