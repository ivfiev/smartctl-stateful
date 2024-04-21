using Cocona;
using Microsoft.Extensions.DependencyInjection;
using Smartctl.App.Commands;
using Smartctl.Core;
using Smartctl.Core.Contracts;
using Smartctl.Core.Directories;
using Smartctl.Core.Formatters;
using Smartctl.Core.SmartMonTools;
using Smartctl.Core.Terminal;
using Smartctl.Data;

var builder = CoconaApp.CreateBuilder();

builder.Services.AddDbContext<SmartctlContext>();

builder.Services.AddSingleton<ICommandExecutor, CommandExecutor>();
builder.Services.AddSingleton<IDeviceStatsProvider, SmartMonToolsWrapper>();
builder.Services.AddSingleton<IDirectoryStatsProvider, DuWrapper>();
builder.Services.AddSingleton<SmartctlService>();
builder.Services.AddSingleton<DiffService>();
builder.Services.AddSingleton<IDeviceStatsFormatter, PlainTextDeviceStatsFormatter>();
builder.Services.AddSingleton<IDirectoryDiffFormatter, PlainTextDirectoryDiffFormatter>();

var app = builder.Build();

app.Services.GetRequiredService<SmartctlContext>().Database.EnsureCreated();

app.AddCommands<SmartctlCommand>();
app.AddCommands<DiffCommand>();

app.Run();
