using Moq;
using Smartctl.Core;
using Smartctl.Core.Contracts;
using Smartctl.Data.Models;

namespace Smartctl.Tests.ServiceTests;

public class DiffTests : BaseTest
{
    public DiffTests()
    {
        Db = GetDb();
        Provider = new Mock<IDirectoryStatsProvider>();
        Sut = new DiffService(Db, Provider.Object);

        ConfigureProvider();
    }

    private DiffService Sut { get; set; }
    private Mock<IDirectoryStatsProvider> Provider { get; set; }

    [Fact]
    public void Diff_WhenCalled_AddsSingleDataPoint()
    {
        Sut.GetDiff("/dir");
        Assert.Single(Db.DirectoryDataPoints);
    }

    [Fact]
    public void Diff_WhenTodaysDataExists_UpdatesTodaysDataPoint()
    {
        PopulateDb((0, "/dir/X", 500));
        Sut.GetDiff("/dir");
        Assert.Equal(1000, Db.DirectoryDataPoints.Single().SizeKb);
    }

    [Fact]
    public void Diff_WhenYesterdaysDataPointExists_AddsAnotherDataPoint()
    {
        PopulateDb((1, "/dir/X", 1000));
        Sut.GetDiff("/dir");
        Assert.Equal(2, Db.DirectoryDataPoints.Count());
    }

    [Fact]
    public void Diff_WhenNewFileAdded_AddsDataPointAlongside()
    {
        PopulateDb((0, "/dir/Y", 1000));
        Sut.GetDiff("/dir");
        Assert.Equal(2, Db.DirectoryDataPoints.Count());
    }

    [Fact]
    public void Diff_WhenProviderThrows_DoesNotAddDataPoints()
    {
        Provider.Setup(p => p.GetStats(It.IsAny<string>())).Throws(new Exception());
        Assert.Throws<Exception>(() => Sut.GetDiff("/dir"));
        Assert.Empty(Db.DirectoryDataPoints);
    }

    [Fact]
    public void Diff_WhenNoPreviousData_ReturnsCurrentData()
    {
        Assert.Single(Sut.GetDiff("/dir"));
    }

    [Fact]
    public void Diff_WhenSizesDidNotChange_ReturnsTheZeroDiff()
    {
        PopulateDb((1, "/dir/X", 1000));
        var result = Sut.GetDiff("/dir");
        Assert.Equal(0, result.Single().SizeKb);
        Assert.Equal(2, Db.DirectoryDataPoints.Count());
    }

    [Fact]
    public void Diff_WhenSingleSizeIncreased_ReturnsCorrectDiff()
    {
        PopulateDb((1, "/dir/X", 500));
        var diff = Sut.GetDiff("/dir");
        Assert.Equal(500, diff.Single().SizeKb);
        Assert.Equal("/dir/X", diff.Single().Path);
    }

    [Fact]
    public void Diff_WhenFileSizeDecreased_ReturnsNegativeDiff()
    {
        PopulateDb((1, "/dir/X", 1500));
        var diff = Sut.GetDiff("/dir");
        Assert.Equal(-500, diff.Single().SizeKb);
        Assert.Equal("/dir/X", diff.Single().Path);
    }

    [Fact]
    public void Diff_WhenLastPointWeekAgo_ReturnsNegativeDiff()
    {
        PopulateDb((7, "/dir/X", 1500));
        var diff = Sut.GetDiff("/dir");
        Assert.Equal(-500, diff.Single().SizeKb);
        Assert.Equal("/dir/X", diff.Single().Path);
    }

    [Fact]
    public void Diff_WhenLastPointWeekAgo_ReturnsPositiveDiff()
    {
        PopulateDb((7, "/dir/X", 500));
        var diff = Sut.GetDiff("/dir");
        Assert.Equal(500, diff.Single().SizeKb);
        Assert.Equal("/dir/X", diff.Single().Path);
    }

    [Fact]
    public void Diff_WhenLastPointWeekAgo_ReturnsZeroDiff()
    {
        PopulateDb((7, "/dir/X", 1000));
        var diff = Sut.GetDiff("/dir");
        Assert.Equal(0, diff.Single().SizeKb);
        Assert.Equal("/dir/X", diff.Single().Path);
    }

    [Fact]
    public void Diff_WhenFileGotAdded_ReturnsPositiveDiff()
    {
        var diff = Sut.GetDiff("/dir");
        Assert.Equal(1000, diff.Single().SizeKb);
    }

    [Fact]
    public void Diff_UsesRecentData()
    {
        PopulateDb((1, "/dir/X", 1500), (2, "/dir/X", 1800), (3, "/dir/Z", 500));
        var diff = Sut.GetDiff("/dir");
        Assert.Equal("/dir/X", diff.Single().Path);
        Assert.Equal(-500, diff.Single().SizeKb);
    }

    [Fact]
    public void Diff_WhenMultipleFiles_ReturnsCorrectDiff()
    {
        PopulateDb((1, "/dir/X", 1500), (1, "/dir/Y", 1000), (1, "/dir/Z", 500));
        ConfigureProvider([("/dir/Z", 1200), ("/dir/X", 800), ("/dir/U", 1)]);
        var diff = Sut.GetDiff("/dir");
        Assert.Equal([new("/dir/Z", 700), new("/dir/X", -700), new("/dir/U", 1), new("/dir/Y", -1000)], diff);
    }

    private void ConfigureProvider((string, long)[]? data = null)
    {
        var stats = data is null
            ? [new("/dir/X", 1000)]
            : data.Select(d => new DirectoryStats(d.Item1, d.Item2)).ToArray();
        Provider.Setup(p => p.GetStats("/dir")).Returns(stats);
    }

    private void PopulateDb(params (int, string, long)[] data)
    {
        foreach (var (offset, path, sizeKb) in data)
        {
            Db.DirectoryDataPoints.Add(new DirectoryDataPoint
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-offset)),
                Path = path,
                SizeKb = sizeKb
            });
        }
        Db.SaveChanges();
    }
}
