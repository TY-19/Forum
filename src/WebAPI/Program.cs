using Forum.Application;
using Forum.Infrastructure;
using Forum.Infrastructure.Data;
using Forum.WebAPI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

await app.InitialiseDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/test", () => new { Response = "The server has returned a result" });

app.UseCors("FreeCorsPolicy");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(_ => { });

app.MapControllers();
app.Run();
