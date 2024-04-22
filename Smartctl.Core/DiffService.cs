using Smartctl.Core.Contracts;
using Smartctl.Data;
using Smartctl.Data.Models;

namespace Smartctl.Core;

public class DiffService(SmartctlContext db, IDirectoryStatsProvider provider)
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    public DirectoryStats[] GetDiff(string directory)
    {
        var stats = provider.GetStats(directory);
        var todaysData = db.DirectoryDataPoints.Where(data => data.Date == _today).ToDictionary(data => data.Path);

        foreach (var s in stats)
        {
            UpsertTodaysDataPoint(todaysData, s);
        }

        db.SaveChanges();

        return CalcDiff(directory, stats);
    }

    private void UpsertTodaysDataPoint(IDictionary<string, DirectoryDataPoint> todaysData, DirectoryStats stats)
    {
        var todaysPoint = todaysData.TryGetValue(stats.Path, out var value) ? value : null;

        if (todaysPoint is null)
        {
            var model = GetModel(stats);
            db.DirectoryDataPoints.Add(model);
        }
        else
        {
            todaysPoint.SizeKb = stats.SizeKb;
        }
    }

    private DirectoryDataPoint GetModel(DirectoryStats stats)
    {
        return new DirectoryDataPoint
        {
            Date = _today,
            Path = stats.Path,
            SizeKb = stats.SizeKb
        };
    }

    private DirectoryStats[] CalcDiff(string dir, DirectoryStats[] current)
    {
        var allDates = db.DirectoryDataPoints.Select(data => data.Date).ToHashSet();
        allDates.Remove(_today);

        if (allDates.Count == 0)
        {
            return current;
        }

        var lastDate = allDates.Max();
        var previous = db.DirectoryDataPoints
            .Where(data => data.Path.StartsWith(dir) && data.Date == lastDate)
            .ToDictionary(data => data.Path, data => data.SizeKb);

        List<DirectoryStats> result = [];

        foreach (var (path, sizeKb) in current)
        {
            if (previous.TryGetValue(path, out var prevSizeKb))
            {
                result.Add(new(path, sizeKb - prevSizeKb));
            }
            else
            {
                result.Add(new(path, sizeKb));
            }
        }

        foreach (var (prevPath, prevSizeKb) in previous)
        {
            if (current.All(curr => curr.Path != prevPath))
            {
                result.Add(new(prevPath, -prevSizeKb));
            }
        }

        return result.ToArray();
    }
}
