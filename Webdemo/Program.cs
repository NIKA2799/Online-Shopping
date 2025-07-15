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
Startup.ConfigureAsync(app, builder.Environment);
builder.Configuration
    .AddUserSecrets<Program>(); // or Startup

var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedAdminHelper.SeedAdminAsync(services);
}
app.Run();
