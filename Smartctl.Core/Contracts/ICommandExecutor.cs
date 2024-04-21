namespace Smartctl.Core.Contracts;

public interface ICommandExecutor
{
    string Exec(string command);

    string SudoExec(string command);
}
