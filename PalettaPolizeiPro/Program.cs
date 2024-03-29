using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using PalettaPolizeiPro;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Data.Palettas;
using PalettaPolizeiPro.Database;
using PalettaPolizeiPro.Services;
using PalettaPolizeiPro.Services.PalettaControl;
using PalettaPolizeiPro.Services.PLC;
using PalettaPolizeiPro.Services.Simulation;
using System.Diagnostics;


LogService.Init(Path.Combine(Environment.CurrentDirectory, "logs.txt"));
LogService.Log("Server started",LogLevel.Information);

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
string? cs = builder.Configuration.GetConnectionString("Database");
Console.WriteLine(cs);
if (cs is null) { throw new Exception("Please enter a valid connection string in appsettings.json"); }
DatabaseContext.Initialize(cs);

var palettaControl = PalettaControlService.GetInstance();
var config = new ConfigReadService();
var stations = config.GetQueryStations();
stations.AddRange(config.GetCheckinStations());
List<IPLCLayer> plcs = new List<IPLCLayer>();

PalettaControlService.Init(plcs);
builder.Services.AddScoped(typeof(IUserService), typeof(UserService));
builder.Services.AddScoped(typeof(ILoginService), typeof(LoginService));
builder.Services.AddSingleton(typeof(IPalettaControlService), palettaControl);
builder.Services.AddScoped(typeof(Client));
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);

#if RELEASE
    builder.WebHost.UseUrls("http://*:80");
#endif

var app = builder.Build();

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

app.Run();
