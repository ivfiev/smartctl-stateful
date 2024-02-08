using Microsoft.EntityFrameworkCore;
using Smartctl.Data.Models;

namespace Smartctl.Data;

public class SmartctlContext : DbContext
{
    public DbSet<DeviceDataPoint> DeviceDataPoints { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var home = Environment.GetEnvironmentVariable("HOME");
        options.UseSqlite($"Data Source={home}/.smartctl-stateful/data.db");
    }
}
