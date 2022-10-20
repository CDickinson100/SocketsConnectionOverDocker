using Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddHostedService<SocketClient>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapHealthChecks( "/health" );

app.Run();