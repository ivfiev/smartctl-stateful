using Microsoft.EntityFrameworkCore;
using Smartctl.Data.Models;

namespace Smartctl.Data;

public class SmartctlContext : DbContext
{
    public DbSet<DeviceDataPoint> DeviceDataPoints { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (Environment.GetEnvironmentVariable("SMARTCTL_SQLITE_INMEMORY") == "1")
        {
            options.UseSqlite("DataSource=:memory:?cache=shared");
        }
        else
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            options.UseSqlite($"Data Source={home}/.smartctl-stateful/data.db");
        }
    }
}
