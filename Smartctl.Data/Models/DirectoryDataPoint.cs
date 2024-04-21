using Microsoft.EntityFrameworkCore;

namespace Smartctl.Data.Models;

[PrimaryKey(nameof(Date), nameof(Path))]
public class DirectoryDataPoint
{
    public DateOnly Date { get; set; }

    public string Path { get; set; }

    public double SizeGb { get; set; }
}
