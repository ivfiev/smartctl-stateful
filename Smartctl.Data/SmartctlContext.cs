using Microsoft.EntityFrameworkCore;
using Smartctl.Data.Models;

namespace Smartctl.Data;

public class SmartctlContext : DbContext
{
    public DbSet<DeviceDataPoint> DeviceDataPoints { get; set; }

    public DbSet<DirectoryDataPoint> DirectoryDataPoints { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (Environment.GetEnvironmentVariable("SMARTCTL_SQLITE_INMEMORY") == "1")
        {
            options.UseSqlite("DataSource=:memory:");
        }
        else
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            options.UseSqlite($"DataSource={Path.Combine(home, ".smartctl-stateful", "data.db")}");
        }
    }
}
