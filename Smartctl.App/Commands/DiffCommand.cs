using Cocona;
using Smartctl.Core;
using Smartctl.Core.Contracts;

namespace Smartctl.App.Commands;

public class DiffCommand(DiffService svc, IDirectoryDiffFormatter fmt)
{
    [Command("diff")]
    public void Run([Option('d')] string directory)
    {
        var diff = svc.GetDiff(directory);
        var formatted = fmt.Format(diff);
        Console.WriteLine(formatted);
    }
}
