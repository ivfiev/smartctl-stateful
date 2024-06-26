using System.Text;
using Moq;
using Smartctl.Core.Contracts;
using Smartctl.Core.Directories;

namespace Smartctl.Tests.WrapperTests;

public class DuTests
{
    public DuTests()
    {
        Cmd = new Mock<ICommandExecutor>();
        Sut = new DuWrapper(Cmd.Object);
    }

    private DuWrapper Sut { get; }
    private Mock<ICommandExecutor> Cmd { get; }

    [Fact]
    public void DuWrapper_ParsesEmptyListCorrectly()
    {
        Setup("/dir");
        var stats = Sut.GetStats("/dir");
        Assert.Empty(stats);
    }

    [Fact]
    public void DuWrapper_ParsesSingletonListCorrectly()
    {
        Setup("/dir", ("X", 1000));
        var stats = Sut.GetStats("/dir");
        Assert.Equal([new("/dir/X", 1000)], stats);
    }

    [Fact]
    public void DuWrapper_ParsesMultiElementListCorrectly()
    {
        Setup("/dir1/dir2", ("X", 1000), ("Y", 3200000), ("Z", 4));
        var stats = Sut.GetStats("/dir1/dir2");
        Assert.Equal([new("/dir1/dir2/X", 1000), new("/dir1/dir2/Y", 3200000), new("/dir1/dir2/Z", 4)], stats);
    }

    [Fact]
    public void DuWrapper_IgnoresTrailingSlash()
    {
        Setup("/dir", ("X", 1));
        var stats = Sut.GetStats("/dir/");
        Assert.Equal([new("/dir/X", 1)], stats);
    }

    private void Setup(string dir, params (string, int)[] data)
    {
        var raw = GetRawData(dir, data);
        var files = data.Select(d => d.Item1).ToArray();
        var paths = files.Select(f => $"{dir}/{f}");
        Cmd
            .Setup(cmd => cmd.Exec(It.Is<string>(c => c == $"ls -A {dir}")))
            .Returns(string.Join(" ", files));
        Cmd
            .Setup(cmd => cmd.Exec(It.Is<string>(c => c == $"du -s {string.Join(" ", paths)}")))
            .Returns(raw);
    }

    private string GetRawData(string dir, params (string, int)[] data)
    {
        var sb = new StringBuilder();

        foreach (var (file, sizeKb) in data)
        {
            sb.Append($"\r{sizeKb} \t {dir}/{file} \n");
        }

        return sb.ToString();
    }
}
