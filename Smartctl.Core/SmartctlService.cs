using Smartctl.Core.Contracts;
using Smartctl.Data;
using Smartctl.Data.Models;

namespace Smartctl.Core;

public class SmartctlService(SmartctlContext db, IDeviceStatsProvider provider)
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    public PeriodDeviceStats GetPeriodDeviceStats(string deviceId)
    {
        var stats = provider.GetDeviceStats(deviceId);
        UpsertTodaysDataPoint(deviceId, stats);
        return CalcStats(deviceId, stats);
    }

    private void UpsertTodaysDataPoint(string deviceId, DeviceStats stats)
    {
        var today = db.DeviceDataPoints.FirstOrDefault(data => data.Date == _today);

        if (today is null)
        {
            var model = GetModel(DateOnly.FromDateTime(DateTime.Now), deviceId, stats);
            db.DeviceDataPoints.Add(model);
        }
        else
        {
            today.ReadTb = stats.ReadTb;
            today.WrittenTb = stats.WrittenTb;
        }

        db.SaveChanges();
    }

    private PeriodDeviceStats CalcStats(string deviceId, DeviceStats stats)
    {
        var allStats = db.DeviceDataPoints
            .Where(data => data.Device == deviceId)
            .OrderByDescending(data => data.Date)
            .ToArray();

        var today = allStats.First(data => data.Date == _today);
        var yesterday = allStats.SkipWhile(data => data.Date >= _today).FirstOrDefault();
        var week = allStats.SkipWhile(data => data.Date > _today.AddDays(-7)).FirstOrDefault();
        var month = allStats.SkipWhile(data => data.Date > _today.AddDays(-30)).FirstOrDefault();

        var periods = new Dictionary<int, double>();
        AddPeriodPoint(periods, today, yesterday);
        AddPeriodPoint(periods, today, week);
        AddPeriodPoint(periods, today, month);

        return new PeriodDeviceStats(today.ReadTb, today.WrittenTb, periods, stats.Errors);
    }

    private void AddPeriodPoint(IDictionary<int, double> periods, DeviceDataPoint today, DeviceDataPoint? then)
    {
        if (then is not null)
        {
            var days = today.Date.DayNumber - then.Date.DayNumber;
            periods[days] = today.WrittenTb - then.WrittenTb;
        }
    }

    private DeviceDataPoint GetModel(DateOnly date, string deviceId, DeviceStats stats)
    {
        return new DeviceDataPoint
        {
            Date = date,
            Device = deviceId,
            ReadTb = stats.ReadTb,
            WrittenTb = stats.WrittenTb
        };
    }
}
