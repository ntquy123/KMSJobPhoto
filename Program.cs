
using System.IO;
using Microsoft.Extensions.Configuration;
using erpsolution.api;

// Determine the hosting environment as early as possible so the correct appsettings.<ENV>.json is loaded.
var bootstrapConfiguration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .Build();

var requestedEnvironment = bootstrapConfiguration["Environment"];

if (!string.IsNullOrWhiteSpace(requestedEnvironment))
{
    void EnsureEnvironmentVariable(string key)
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
        {
            Environment.SetEnvironmentVariable(key, requestedEnvironment);
        }
    }

    EnsureEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    EnsureEnvironmentVariable("DOTNET_ENVIRONMENT");
}

var builder = WebApplication.CreateBuilder(args);

// Clear default logging providers so NLog is the single source of truth for log output.
builder.Logging.ClearProviders();

// Instantiate Startup so we can continue using the familiar Configure/ConfigureServices pattern.
var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Resolve the logging dependencies that Startup.Configure expects.
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

// Configure the HTTP pipeline using the existing Startup logic.
startup.Configure(app, app.Environment, loggerFactory, app.Services);

app.Run();
