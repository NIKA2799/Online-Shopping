using Serilog;
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
Startup.Configure(app, builder.Environment);
app.Run();
