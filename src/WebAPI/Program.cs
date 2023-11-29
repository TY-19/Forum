using Forum.Infrastructure;
using Forum.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt => opt.AddPolicy(name: "FreeCorsPolicy",cfg => 
{ 
    cfg.AllowAnyHeader();
    cfg.AllowAnyMethod();
    cfg.WithOrigins("*");
}));

var app = builder.Build();

await app.InitialiseDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/test", () => new { Response = "The server has returned a result" });

app.UseCors("FreeCorsPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
