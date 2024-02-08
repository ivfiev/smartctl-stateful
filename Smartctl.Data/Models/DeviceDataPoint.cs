using Microsoft.EntityFrameworkCore;

namespace Smartctl.Data.Models;

[PrimaryKey(nameof(Date), nameof(Device))]
public class DeviceDataPoint
{
    public DateOnly Date { get; set; }

    public string Device { get; set; }

    public double ReadTb { get; set; }

    public double WrittenTb { get; set; }

    public int Errors { get; set; }
}
