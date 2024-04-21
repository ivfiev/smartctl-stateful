using Smartctl.Core.Contracts;

namespace Smartctl.Core.Directories;

public class DuWrapper(ICommandExecutor cmd) : IDirectoryStatsProvider
{
    public DirectoryStats[] GetStats(string directory)
    {
        directory = directory.TrimEnd('/');
        var du = GetCommand(directory);
        var output = cmd.Exec(du);
        var lines = output.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var stats = lines.Select(Parse).ToArray();
        return stats;
    }

    private string GetCommand(string dir)
    {
        var ls = cmd.Exec($"ls -A {dir}");
        var directories = ls.Split((char[])null!, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var command = $"du -sh {string.Join(" ", directories.Select(sub => $"{dir}/{sub}"))}";
        return command;
    }

    private DirectoryStats Parse(string line)
    {
        var pair = line.Split((char[])null!, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var path = pair[1];

        var size = pair[0].Last() switch
        {
            'K' => ParseNumericPart(pair[0]) / 1000000,
            'M' => ParseNumericPart(pair[0]) / 1000,
            'G' => ParseNumericPart(pair[0]),
            _ => double.Parse(pair[0])
        };

        return new(path, size);

        double ParseNumericPart(string str)
        {
            return double.Parse(str[..^1]);
        }
    }
}
