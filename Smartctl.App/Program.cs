using Smartctl.Core.Contracts;
using Smartctl.Core.SmartMonTools;
using Smartctl.Core.Terminal;

var res = new SmartMonToolsWrapper(new CommandExecutor()).GetDiskStats(new DiskStatsArgs("/dev/nvme0"));

Console.WriteLine(res);