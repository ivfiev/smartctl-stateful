using Cocona;
using Smartctl.Core;

namespace Smartctl.App.Commands;

public class SmartctlCommand(SmartctlService svc)
{
    public void Run([Option('d')] string deviceId)
    {
        var result = svc.GetPeriodDeviceStats(deviceId);
        Console.WriteLine(result);
    }
}
