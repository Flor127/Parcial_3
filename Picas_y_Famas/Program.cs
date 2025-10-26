using Picas_y_Famas;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // Esto lo maneja Startup ahora

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();
