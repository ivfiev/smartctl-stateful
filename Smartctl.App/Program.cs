using Smartctl.Data;
using Smartctl.Data.Models;

// var res = new SmartMonToolsWrapper(new CommandExecutor()).GetDiskStats(new DiskStatsArgs("/dev/nvme0"));
//
// Console.WriteLine(res);

using var db = new SmartctlContext();

db.Database.EnsureCreated();

db.DiskDataPoints.Add(new DiskDataPoint
{
    Device = "dev",
    Date = DateTime.Now,
    RawJson = ""
});

db.SaveChanges();
