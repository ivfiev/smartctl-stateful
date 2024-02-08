using Microsoft.EntityFrameworkCore;

namespace Smartctl.Data.Models;

[PrimaryKey(nameof(Device), nameof(Date))]
public class DeviceDataPoint
{
    public string Device { get; set; }

    public DateOnly Date { get; set; }

    public double ReadTb { get; set; }

    public double WrittenTb { get; set; }

    public int Errors { get; set; }
}
