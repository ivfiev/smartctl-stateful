using System.Text.Json;
using System.Text.Json.Serialization;
using Smartctl.Core.Contracts;

namespace Smartctl.Core.SmartMonTools;

public class SmartMonToolsWrapper(ICommandExecutor exec) : IDeviceStatsProvider
{
    public DeviceStats GetDeviceStats(string deviceId)
    {
        var json = exec.SudoExec(GetCommandString(deviceId));
        var obj = JsonSerializer.Deserialize<SmartMonToolsRawResult>(json);
        return new DeviceStats(
            GetTb(obj.Log.DataUnitsRead),
            GetTb(obj.Log.DataUnitsWritten),
            obj.Log.ErrorLogs + obj.Log.MediaErrors);
    }

    private static string GetCommandString(string deviceId)
    {
        return $"smartctl -A {deviceId} -j";
    }

    private double GetTb(ulong units)
    {
        return (double)units / 1_000_000_000 * 512;
    }

    private class SmartMonToolsRawResult
    {
        [JsonPropertyName("nvme_smart_health_information_log")]
        public required NvmeSmartInfoLog Log { get; set; }
    }

    private class NvmeSmartInfoLog
    {
        [JsonPropertyName("data_units_read")]
        public required ulong DataUnitsRead { get; set; }

        [JsonPropertyName("data_units_written")]
        public required ulong DataUnitsWritten { get; set; }

        [JsonPropertyName("media_errors")]
        public required int MediaErrors { get; set; }

        [JsonPropertyName("num_err_log_entries")]
        public required int ErrorLogs { get; set; }
    }
}
