using Moq;
using Smartctl.Core.Contracts;
using Smartctl.Core.SmartMonTools;

namespace Smartctl.Tests.WrapperTests;

public class SmartMonToolsTests
{
    public SmartMonToolsTests()
    {
        Cmd = new Mock<ICommandExecutor>();
        Sut = new SmartMonToolsWrapper(Cmd.Object);
    }

    private SmartMonToolsWrapper Sut { get; }
    private Mock<ICommandExecutor> Cmd { get; }

    [Fact]
    public void Wrapper_ParsesJsonDataCorrectly()
    {
        Cmd.Setup(cmd => cmd.ExecAsSudo(It.IsAny<string>())).Returns(GetJson(2000000, 1000000, 3, 5));

        var result = Sut.GetDeviceStats("/device");

        Assert.Equal(1.024, result.ReadTb);
        Assert.Equal(0.512, result.WrittenTb);
        Assert.Equal(8, result.Errors);
    }

    [Fact]
    public void Wrapper_RequestsCorrectDeviceId()
    {
        Cmd.Setup(cmd => cmd.ExecAsSudo(It.IsAny<string>())).Returns(GetJson());

        Sut.GetDeviceStats("/dev/nvme0");

        Cmd.Verify(cmd => cmd.ExecAsSudo(It.Is<string>(str => str.Contains("/dev/nvme0"))), Times.Once());
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
