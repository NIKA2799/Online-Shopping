using Dto;
using Interface.Model;
using Serilog;
using Webdemo.Exstnsion;
using Webdemo.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<AdminSettings>(
    builder.Configuration.GetSection("AdminSettings"));
// Configure logging with Serilog (optional)
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext();
});
Startup.ConfigureServices(builder.Services, builder.Configuration);
var app = builder.Build();

// bind the "AdminSettings" section

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
Startup.ConfigureAsync(app, builder.Environment);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    await SeedAdminHelper.SeedAdminAsync(sp);
}




app.Run();
