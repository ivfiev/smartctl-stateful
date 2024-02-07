using Moq;
using Smartctl.Core.Contracts;
using Smartctl.Core.SmartMonTools;

namespace Smartctl.Tests.WrapperTests;

public class SmartMonTools
{
    public SmartMonTools()
    {
        Cmd = new Mock<ICommandExecutor>();
        Sut = new SmartMonToolsWrapper(Cmd.Object);
    }

    private SmartMonToolsWrapper Sut { get; }
    private Mock<ICommandExecutor> Cmd { get; }

    [Theory]
    [InlineData("/device1", 1001, 1002, 1001)]
    [InlineData("/device2", 1002, 8888, 8888)]
    public void Wrapper_GivenDeviceString_ReturnsCorrectValue(string device, int data1, int data2, int expected)
    {
        Cmd.Setup(cmd => cmd.ExecAsSudo(It.Is<string>(str => str.Contains("/device1")))).Returns(GetJson(data1));
        Cmd.Setup(cmd => cmd.ExecAsSudo(It.Is<string>(str => str.Contains("/device2")))).Returns(GetJson(data2));

        var result = Sut.GetDiskStats(Args(device));

        Assert.Equal((ulong)expected, result.DataUnitsWritten);
        Assert.Equal(GetJson(expected), result.Raw);
    }

    private DiskStatsArgs Args(string device = "/device")
    {
        return new DiskStatsArgs(device);
    }

    private string GetJson(int dataUnitsWritten)
    {
        return $$"""
                 {
                    "nvme_smart_health_information_log":
                    {
                        "data_units_written": {{dataUnitsWritten}}
                    }
                 }
                 """;
    }
}