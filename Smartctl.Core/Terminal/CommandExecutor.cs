using System.Diagnostics;
using Smartctl.Core.Contracts;

namespace Smartctl.Core.Terminal;

public class CommandExecutor : ICommandExecutor
{
    public string Exec(string command)
    {
        using var process = new Process();

        var words = command.Split(' ');
        var fileName = words[0];
        var args = string.Join(" ", words[1..]);

        process.StartInfo = new()
        {
            FileName = fileName,
            Arguments = args,
            RedirectStandardOutput = true
        };

        process.Start();
        process.WaitForExit();

        var result = process.StandardOutput.ReadToEnd();

        return result;
    }

    public string SudoExec(string command)
    {
        using var process = new Process();

        process.StartInfo = new()
        {
            FileName = "pkexec",
            Arguments = command,
            RedirectStandardOutput = true
        };

        process.Start();
        process.WaitForExit();

        var result = process.StandardOutput.ReadToEnd();

        return result;
    }
}
