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
using PalettaPolizeiPro.Services.Simulation;



var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
string? cs = builder.Configuration.GetConnectionString("Default");
Console.WriteLine(cs);  
if (cs is null) { throw new Exception("Please enter a valid connection string in appsettings.json"); }
DatabaseContext.SetConnectionString(cs);

new DatabaseContext().Database.Migrate();
var palettaControl = PalettaControlService.GetInstance();
var config = new ConfigReadService();
var stations = config.GetQueryStations();
stations.AddRange(config.GetCheckinStations());
List<IPLCLayer> plcs = new List<IPLCLayer>();   
foreach (var station in stations) 
{
    if (plcs!.FirstOrDefault(x => x.IP == station.IP && x.Rack == station.Rack && x.Slot == station.Slot) is null)
    {
        plcs.Add(new SimulationPlcLayer(station.IP,station.Rack,station.Slot));
    }
}

var jobs = new LongRunningJobHandler(new List<IUpdatable>
{ 
    new SimulationProcess(new List<Loop>{
        new Loop
        {
            CheckinStations = new List<Station>
            {
                new Station
                {
                    DB = 1,
                    IP = "192.168.1.120",
                    Rack = 1,
                    Slot = 1  
                   ,
                    Name = "First test loop querystation"
                }
            },
            QueryStations  = new List<Station>
            {
                 new Station
                {
                    DB = 2,
                    IP = "192.168.1.120",
                    Rack = 1,
                    Slot = 1
                   ,
                    Name = "First test loop checkstation"
                },
                  new Station
                {
                    DB = 3,
                    IP = "192.168.1.121",
                    Rack = 1,
                    Slot = 1
                   ,
                    Name = "First test loop checkstation"
                }
            },
            Palettas = new List<Paletta>
            {
                 
            },
            Name = "Firs Loop"
        }
    
    }),

},100);

PalettaControlService.Init(plcs);

builder.Services.AddScoped(typeof(IUserService),typeof(UserService));
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
