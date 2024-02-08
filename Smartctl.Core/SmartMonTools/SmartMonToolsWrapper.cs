using System.Text.Json;
using System.Text.Json.Serialization;
using Smartctl.Core.Contracts;

namespace Smartctl.Core.SmartMonTools;

public class SmartMonToolsWrapper(ICommandExecutor exec) : IDeviceStatsProvider
{
    public DeviceStats GetDeviceStats(DeviceStatsArgs args)
    {
        var json = exec.ExecAsSudo(GetCommandString(args));
        var obj = JsonSerializer.Deserialize<SmartMonToolsRawResult>(json);
        return new DeviceStats(
            obj.Log.DataUnitsRead,
            obj.Log.DataUnitsWritten,
            obj.Log.ErrorLogs + obj.Log.MediaErrors);
    }

    private static string GetCommandString(DeviceStatsArgs args)
    {
        return $"smartctl -A {args.DeviceId} -j";
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
        public required uint MediaErrors { get; set; }

        [JsonPropertyName("num_err_log_entries")]
        public required uint ErrorLogs { get; set; }
    }
}
