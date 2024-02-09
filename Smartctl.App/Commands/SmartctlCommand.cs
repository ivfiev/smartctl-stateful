using Cocona;
using Smartctl.Core;
using Smartctl.Core.Formatters;

namespace Smartctl.App.Commands;

public class SmartctlCommand(SmartctlService svc, IStatsFormatter fmt)
{
    public void Run([Option('d')] string deviceId, [Option('p')] int precision = 3)
    {
        var result = svc.GetPeriodDeviceStats(deviceId);
        var formatted = fmt.Format(result, precision);
        Console.WriteLine(formatted);
    }
}
