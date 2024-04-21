using Microsoft.EntityFrameworkCore;
using Smartctl.Data;

namespace Smartctl.Tests.ServiceTests;

public class BaseTest
{
    protected SmartctlContext Db { get; set; }

    protected SmartctlContext GetDb()
    {
        Environment.SetEnvironmentVariable("SMARTCTL_SQLITE_INMEMORY", "1");
        Db = new SmartctlContext();
        Db.Database.OpenConnection();
        Db.Database.EnsureCreated();
        return Db;
    }
}
