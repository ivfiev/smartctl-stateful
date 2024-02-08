using Microsoft.EntityFrameworkCore;

namespace Smartctl.Data.Models;

[PrimaryKey(nameof(Date), nameof(Device))]
public class DeviceDataPoint
{
    public DateTime Date { get; set; }

    public string Device { get; set; }

    public ulong DataUnitsRead { get; set; }

    public ulong DataUnitsWritten { get; set; }

    public uint TotalErrors { get; set; }
}
