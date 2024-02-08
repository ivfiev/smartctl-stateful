using Smartctl.Core.Contracts;
using Smartctl.Data;
using Smartctl.Data.Models;

namespace Smartctl.Core;

public class SmartctlService(SmartctlContext db, IDeviceStatsProvider provider)
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    public PeriodDeviceStats GetPeriodDeviceStats(string deviceId)
    {
        UpsertTodaysDataPoint(deviceId);
        return CalcStats(deviceId);
    }

    private void UpsertTodaysDataPoint(string deviceId)
    {
        var stats = provider.GetDeviceStats(deviceId);

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
            today.Errors = stats.Errors;
        }

        db.SaveChanges();
    }

    private PeriodDeviceStats CalcStats(string deviceId)
    {
        var allStats = db.DeviceDataPoints
            .Where(data => data.Device == deviceId)
            .OrderByDescending(data => data.Date)
            .ToArray();
        var today = allStats.First(data => data.Date == _today);
        return new PeriodDeviceStats(today.ReadTb, today.WrittenTb, null, today.Errors);
    }

    private DeviceDataPoint GetModel(DateOnly date, string deviceId, DeviceStats stats)
    {
        return new DeviceDataPoint
        {
            Date = date,
            Device = deviceId,
            ReadTb = stats.ReadTb,
            WrittenTb = stats.WrittenTb,
            Errors = stats.Errors
        };
    }
}

public record PeriodDeviceStats(double ReadTb, double WrittenTb, IDictionary<int, double> WrittenTbPerPeriod, int Errors);
