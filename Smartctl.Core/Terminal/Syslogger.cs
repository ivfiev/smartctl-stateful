using Smartctl.Core.Contracts;

namespace Smartctl.Core.Terminal;

public class Syslogger
{
    private readonly ICommandExecutor _cmd;

    public Syslogger(ICommandExecutor cmd)
    {
        _cmd = cmd;
    }

    public void Log(string message)
    {
        _cmd.Exec($"logger '{message}'");
    }
}
