using ApexCharts;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using PalettaPolizeiPro;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Data.Users;
using PalettaPolizeiPro.Database;
using PalettaPolizeiPro.MIddleware;
using PalettaPolizeiPro.Services;
using PalettaPolizeiPro.Services.Events;
using PalettaPolizeiPro.Services.Orders;
using PalettaPolizeiPro.Services.PalettaControl;
using PalettaPolizeiPro.Services.PLC;
using PalettaPolizeiPro.Services.Simulation;
using PalettaPolizeiPro.Services.Stations;
using PalettaPolizeiPro.Services.Users;
using System.Diagnostics;

#if DEBUG
DEBUG = true;
#endif

LogService.Init(Path.Combine(Environment.CurrentDirectory, "Logs"));
LogService.Log("Server started", LogLevel.Information);

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
string? cs = builder.Configuration.GetConnectionString("Database");
Console.WriteLine(cs);
if (cs is null) { throw new Exception("Please enter a valid connection string in appsettings.json"); }
DatabaseContext.Initialize(cs);
if (SIMULATION)
{
    SimulatedTcpPlc.InitSimulation("localhost", 6969);
}
//
var palettaControl = PalettaControlService.GetInstance();
var stationService = StationService.GetInstance();
var eventService = LineEventService.GetInstance();
palettaControl.Init(stationService.GetAll());
var lineProcess = new LineControlProcess((PalettaControlService)palettaControl);


builder.Services.AddSingleton(typeof(IStationService), stationService);
builder.Services.AddScoped(typeof(IUserService), typeof(UserService));
builder.Services.AddSingleton(typeof(LineEventService), eventService);
builder.Services.AddSingleton(typeof(LineControlProcess), lineProcess);


builder.Services.AddScoped(typeof(ILoginService), typeof(LoginService));
builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));

builder.Services.AddSingleton(typeof(IPalettaControlService), palettaControl);
builder.Services.AddScoped(typeof(Client));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<ClientIpService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 100;
    config.SnackbarConfiguration.ShowTransitionDuration = 100;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

#if RELEASE
    builder.WebHost.UseUrls("http://*:80");
#endif

var app = builder.Build();
app.UseMiddleware<ClientIpAddressMiddleware>(); 

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
var jobs = new LongRunningJobHandler(new List<IUpdatable> {lineProcess  }, 10);

jobs.Start();
app.Run();
