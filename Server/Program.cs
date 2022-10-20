using Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddHostedService<SocketServer>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapHealthChecks("/health");

app.Run();