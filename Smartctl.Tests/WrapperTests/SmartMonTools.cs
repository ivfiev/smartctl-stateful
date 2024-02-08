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

    [Fact]
    public void Wrapper_ParsesJsonDataCorrectly()
    {
        Cmd.Setup(cmd => cmd.ExecAsSudo(It.IsAny<string>())).Returns(GetJson(123, 234, 3, 5));

        var result = Sut.GetDeviceStats(Args());

        Assert.Equal(123ul, result.DataUnitsRead);
        Assert.Equal(234ul, result.DataUnitsWritten);
        Assert.Equal(8u, result.TotalErrors);
    }

    [Theory]
    [InlineData("/device1", 1001, 1002, 1001)]
    [InlineData("/device2", 1002, 8888, 8888)]
    public void Wrapper_GivenDeviceString_ReturnsCorrectDataUnitsRead(string device, int data1, int data2, int expected)
    {
        Cmd.Setup(cmd => cmd.ExecAsSudo(It.Is<string>(str => str.Contains("/device1")))).Returns(GetJson(data1));
        Cmd.Setup(cmd => cmd.ExecAsSudo(It.Is<string>(str => str.Contains("/device2")))).Returns(GetJson(data2));

        var result = Sut.GetDeviceStats(Args(device));

        Assert.Equal((ulong)expected, result.DataUnitsRead);
    }

    [Theory]
    [InlineData("/device1", 1001, 1002, 1001)]
    [InlineData("/device2", 1002, 8888, 8888)]
    public void Wrapper_GivenDeviceString_ReturnsCorrectDataUnitsWritten(string device, int data1, int data2, int expected)
    {
        Cmd.Setup(cmd => cmd.ExecAsSudo(It.Is<string>(str => str.Contains("/device1")))).Returns(GetJson(dataUnitsWritten: data1));
        Cmd.Setup(cmd => cmd.ExecAsSudo(It.Is<string>(str => str.Contains("/device2")))).Returns(GetJson(dataUnitsWritten: data2));

        var result = Sut.GetDeviceStats(Args(device));

        Assert.Equal((ulong)expected, result.DataUnitsWritten);
    }

    private DeviceStatsArgs Args(string device = "/device")
    {
        return new DeviceStatsArgs(device);
    }

    private string GetJson(int dataUnitsRead = 0, int dataUnitsWritten = 0, int mediaErrors = 0, int errorLogs = 0)
    {
        return $$"""
                 {
                    "nvme_smart_health_information_log":
                    {
                        "data_units_read": {{dataUnitsRead}},
                        "data_units_written": {{dataUnitsWritten}},
                        "media_errors": {{mediaErrors}},
                        "num_err_log_entries": {{errorLogs}}
                    }
                 }
                 """;
    }
}
