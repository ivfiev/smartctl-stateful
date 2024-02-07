using System.Text.Json;
using System.Text.Json.Serialization;
using Smartctl.Core.Contracts;

namespace Smartctl.Core.SmartMonTools;

public class SmartMonToolsWrapper(ICommandExecutor exec) : IDiskStatsProvider
{
    public DiskStats GetDiskStats(DiskStatsArgs args)
    {
        var json = exec.ExecAsSudo(GetCommandString(args));
        var obj = JsonSerializer.Deserialize<SmartResult>(json);
        return new DiskStats(obj.Log.DataUnitsWritten, json);
    }

    private static string GetCommandString(DiskStatsArgs args)
    {
        return $"smartctl -A {args.Device} -j";
    }

    private class SmartResult
    {
        [JsonPropertyName("nvme_smart_health_information_log")]
        public required NvmeSmartInfoLog Log { get; set; }
    }

    private class NvmeSmartInfoLog
    {
        [JsonPropertyName("data_units_written")]
        public required uint DataUnitsWritten { get; set; }
    }
}