using System.Diagnostics;
using Smartctl.Core.Contracts;

namespace Smartctl.Core.Terminal;

public class CommandExecutor : ICommandExecutor
{
    public string ExecAsSudo(string command)
    {
        var process = new Process
        {
            StartInfo = new()
            {
                FileName = "pkexec",
                Arguments = command,
                RedirectStandardOutput = true
            }
        };

        process.Start();
        process.WaitForExit();

        var result = process.StandardOutput.ReadToEnd();

        return result;
    }
}