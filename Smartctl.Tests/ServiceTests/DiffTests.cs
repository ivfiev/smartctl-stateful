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
        PopulateDb((0, "/dir/X", 0.5));
        Sut.GetDiff("/dir");
        Assert.Equal(1, Db.DirectoryDataPoints.Single().SizeGb);
    }

    [Fact]
    public void Diff_WhenYesterdaysDataPointExists_AddsAnotherDataPoint()
    {
        PopulateDb((1, "/dir/X", 1));
        Sut.GetDiff("/dir");
        Assert.Equal(2, Db.DirectoryDataPoints.Count());
    }

    [Fact]
    public void Diff_WhenNewFileAdded_AddsDataPointAlongside()
    {
        PopulateDb((0, "/dir/Y", 1));
        Sut.GetDiff("/dir");
        Assert.Equal(2, Db.DirectoryDataPoints.Count());
    }

    [Fact]
    public void Diff_HavingTodaysData_UpdatesDataPoint()
    {
        PopulateDb((0, "/dir/X", 0.8));
        Sut.GetDiff("/dir");
        Assert.Equal(1, Db.DirectoryDataPoints.Single().SizeGb);
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
        PopulateDb((1, "/dir/X", 1));
        var result = Sut.GetDiff("/dir");
        Assert.Equal(0, result.Single().SizeGb);
        Assert.Equal(2, Db.DirectoryDataPoints.Count());
    }

    [Fact]
    public void Diff_WhenSingleSizeIncreased_ReturnsCorrectDiff()
    {
        PopulateDb((1, "/dir/X", 0.5));
        var diff = Sut.GetDiff("/dir");
        Assert.Equal(0.5, diff.Single().SizeGb);
        Assert.Equal("/dir/X", diff.Single().Path);
    }

    [Fact]
    public void Diff_WhenFileSizeDecreased_ReturnsNegativeDiff()
    {
        PopulateDb((1, "/dir/X", 1.5));
        var diff = Sut.GetDiff("/dir");
        Assert.Equal(-0.5, diff.Single().SizeGb);
        Assert.Equal("/dir/X", diff.Single().Path);
    }

    [Fact]
    public void Diff_WhenFileGotAdded_ReturnsPositiveDiff()
    {
        var diff = Sut.GetDiff("/dir");
        Assert.Equal(1, diff.Single().SizeGb);
    }

    [Fact]
    public void Diff_UsesRecentData()
    {
        PopulateDb((1, "/dir/X", 1.5), (2, "/dir/X", 1.8), (3, "/dir/Z", 0.5));
        var diff = Sut.GetDiff("/dir");
        Assert.Equal("/dir/X", diff.Single().Path);
        Assert.Equal(-0.5, diff.Single().SizeGb);
    }

    [Fact]
    public void Diff_WhenMultipleFiles_ReturnsCorrectDiff()
    {
        PopulateDb((1, "/dir/X", 1.5), (1, "/dir/Y", 1), (1, "/dir/Z", 0.5));
        ConfigureProvider([("/dir/Z", 1.2), ("/dir/X", 0.8), ("/dir/U", 0.001)]);
        var diff = Sut.GetDiff("/dir");
        Assert.Equal([new("/dir/Z", 0.7), new("/dir/X", -0.7), new("/dir/U", 0.001), new("/dir/Y", -1)], diff);
    }

    private void ConfigureProvider((string, double)[]? data = null)
    {
        var stats = data is null
            ? [new("/dir/X", 1)]
            : data.Select(d => new DirectoryStats(d.Item1, d.Item2)).ToArray();
        Provider.Setup(p => p.GetStats("/dir")).Returns(stats);
    }

    private void PopulateDb(params (int, string, double)[] data)
    {
        foreach (var (offset, path, sizeGb) in data)
        {
            Db.DirectoryDataPoints.Add(new DirectoryDataPoint
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-offset)),
                Path = path,
                SizeGb = sizeGb
            });
        }
        Db.SaveChanges();
    }
}
