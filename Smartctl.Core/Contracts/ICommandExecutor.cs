namespace Smartctl.Core.Contracts;

public interface ICommandExecutor
{
    string ExecAsSudo(string command);
}