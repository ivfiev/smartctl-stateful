using Smartctl.Core.Contracts;
using Smartctl.Core.Formatters;

namespace Smartctl.Tests.Formatters;

public class FormatterTests
{
    [Fact]
    public void DeviceStatsFormatter()
    {
        var sut = new PlainTextDeviceStatsFormatter();

        PeriodDeviceStats stats = new(2.12345, 1.23456, new Dictionary<int, double>
        {
            [1] = 0.34567,
            [7] = 0.7890,
            [30] = 0.8
        }, 1);

        var result = sut.Format(stats, 3);

        Assert.Equal("""
                     === SMART/Health Information ===
                     Reads:              2.123 TB
                     Writes:             1.235 TB
                     Errors:             1
                     Writes (1 days):    0.346 TB
                     Writes (7 days):    0.789 TB
                     Writes (30 days):   0.800 TB

                     """, result);
    }

    [Fact]
    public void DirectoryDiffFormatter()
    {
        var sut = new PlainTextDirectoryDiffFormatter();

        DirectoryStats[] stats = [new("/dir/X", 0.000005), new("/dir/Y", 3.2), new("/dir/Zhsch", -1.8873), new("/dir/U", 0)];

        var result = sut.Format(stats);

        Assert.Equal("""
                     ====== Directory diffs ======
                     /dir/Y        +3.2 GB
                     /dir/Zhsch    -1.8873 GB
                     /dir/X        +0.000005 GB

                     """, result);
    }
}
