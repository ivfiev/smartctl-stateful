using Smartctl.Core;
using Smartctl.Core.Formatters;

namespace Smartctl.Tests.Formatters;

public class FormatterTests
{
    private PeriodDeviceStats Stats => new(2.12345, 1.23456, new Dictionary<int, double>
    {
        [1] = 0.34567,
        [7] = 0.7890,
        [30] = 0.8
    }, 1);

    [Fact]
    public void PlainTextFormatter_GivenNoPeriodData_OutputsOnlyTotals()
    {
        var sut = new PlainTextFormatter();

        var result = sut.Format(Stats, 3);

        Assert.Equal("""
                     =========== SMART/Health Information ===========
                     Total read:                             2.123 TB
                     Total written:                          1.235 TB
                     Total errors:                           1
                     Average written over the past 1 days:   0.346 TB
                     Average written over the past 7 days:   0.789 TB
                     Average written over the past 30 days:  0.800 TB

                     """, result);
    }
}
