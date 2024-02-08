using Moq;
using Smartctl.Core;
using Smartctl.Core.Contracts;
using Smartctl.Data;
using Smartctl.Data.Models;

namespace Smartctl.Tests.ServiceTests;

public class SmartctlServiceTests
{
    public SmartctlServiceTests()
    {
        Db = GetDb();
        Provider = new Mock<IDeviceStatsProvider>();
        Sut = new SmartctlService(Db, Provider.Object);
    }

    private SmartctlContext Db { get; set; }
    private SmartctlService Sut { get; set; }
    private Mock<IDeviceStatsProvider> Provider { get; set; }

    [Fact]
    public void Smartctl_WhenCalled_AddsSingleDataPoint()
    {
        ConfigureProvider();

        Sut.GetPeriodDeviceStats("/device1");

        Assert.Single(Db.DeviceDataPoints);
    }

    [Fact]
    public void Smartctl_WhenErrorHappens_RethrowsAndDoesNotAddAnyDataPoints()
    {
        Provider.Setup(p => p.GetDeviceStats(It.IsAny<string>())).Throws(new Exception());

        Assert.Throws<Exception>(() => Sut.GetPeriodDeviceStats("device1"));
        Assert.Empty(Db.DeviceDataPoints);
    }

    [Fact]
    public void Smartctl_WhenCalled_OutputsCorrectTotalData()
    {
        ConfigureProvider();

        var result = Sut.GetPeriodDeviceStats("/device1");

        Assert.Equal(20, result.ReadTb);
        Assert.Equal(10, result.WrittenTb);
        Assert.Equal(0, result.Errors);
    }

    [Fact]
    public void Smartctl_HavingTodaysData_ReplacesTheDataPoint()
    {
        PopulateDb((0, 19, 9, 0));
        ConfigureProvider(20, 10, 1);

        var result = Sut.GetPeriodDeviceStats("/device1");
        var model = Db.DeviceDataPoints.Single();

        Assert.Equal(20, model.ReadTb);
        Assert.Equal(10, model.WrittenTb);
        Assert.Equal(1, model.Errors);
        Assert.Equal(20, result.ReadTb);
        Assert.Equal(10, result.WrittenTb);
        Assert.Equal(1, result.Errors);
    }

    private void ConfigureProvider(double read = 20, double write = 10, int err = 0, string device = "/device1")
    {
        Provider
            .Setup(p => p.GetDeviceStats(It.Is<string>(dev => dev.Contains(device))))
            .Returns(new DeviceStats(read, write, err));
    }

    private void PopulateDb(params (int, double, double, int)[] data)
    {
        foreach (var (days, read, write, err) in data)
        {
            Db.DeviceDataPoints.Add(new DeviceDataPoint
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-days)),
                Device = "/device1",
                ReadTb = read,
                WrittenTb = write,
                Errors = err
            });
        }
        Db.SaveChanges();
    }

    private SmartctlContext GetDb()
    {
        Environment.SetEnvironmentVariable("SQLITE_INMEMORY", "1");
        Db = new SmartctlContext();
        Db.Database.EnsureCreated();
        Db.DeviceDataPoints.RemoveRange(Db.DeviceDataPoints);
        Db.SaveChanges();
        return Db;
    }
}
