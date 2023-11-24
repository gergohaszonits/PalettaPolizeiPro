using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MudBlazor.Services;
using PalettaPolizeiPro.Data;
using PalettaPolizeiPro.Database;
using PalettaPolizeiPro.Services;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
string? cs = builder.Configuration.GetConnectionString("Default");
Console.WriteLine(cs);  
if (cs is null) { throw new Exception("Please enter a valid connection string in appsettings.json"); }

builder.Services.AddScoped(typeof(IUserService),typeof(UserService));
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
