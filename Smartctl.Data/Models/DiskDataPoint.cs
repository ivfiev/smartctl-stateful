using Microsoft.EntityFrameworkCore;

namespace Smartctl.Data.Models;

[PrimaryKey(nameof(Date), nameof(Device))]
public class DiskDataPoint
{
    public DateTime Date { get; set; }

    public string Device { get; set; }

    public ulong DataUnitsWritten { get; set; }

    public string RawJson { get; set; }
}
