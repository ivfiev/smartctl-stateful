namespace Smartctl.Core.Contracts;

public interface IDeviceStatsProvider
{
    DeviceStats GetDeviceStats(string deviceId);
}
