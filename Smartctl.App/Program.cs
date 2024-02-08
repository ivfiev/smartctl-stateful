using Smartctl.Data;

// var res = new SmartMonToolsWrapper(new CommandExecutor()).GetDiskStats(new DiskStatsArgs("/dev/nvme0"));
//
// Console.WriteLine(res);

using var db = new SmartctlContext();

//db.Database.Migrate();
db.Database.EnsureCreated();

// db.DiskDataPoints.Add(new DiskDataPoint
// {
//     Device = "dev",
//     Date = DateTime.Now,
//     RawJson = ""
// });

//db.SaveChanges();
