using Interface.Model;
using Serilog;
using Webdemo.Exstnsion;
using Webdemo.Startup;

var builder = WebApplication.CreateBuilder(args);


// Configure logging with Serilog (optional)
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext();
});
Startup.ConfigureServices(builder.Services, builder.Configuration);
var app = builder.Build();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
Startup.ConfigureAsync(app, builder.Environment);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
builder.Configuration
    .AddUserSecrets<Program>(); // or Startup



app.Run();
