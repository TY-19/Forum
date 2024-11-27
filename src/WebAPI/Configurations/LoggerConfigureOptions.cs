using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Forum.WebAPI.Configurations;

public class LoggerConfigureOptions(IConfiguration configuration)
{
    public void Configure(LoggerConfiguration options)
    {
        options
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .ReadFrom.Configuration(configuration)
            .WriteTo.MSSqlServer(configuration.GetConnectionString("DefaultConnection"),
                restrictedToMinimumLevel: LogEventLevel.Information,
                sinkOptions: new MSSqlServerSinkOptions()
                {
                    TableName = "LogEvents",
                    AutoCreateSqlTable = true,
                })
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug);
    }
}
