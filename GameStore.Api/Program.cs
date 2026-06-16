using GameStore.Api.Endpoints;
using GameStore.Api.Data;

var builder = WebApplication.CreateBuilder(args);

var ConnString = builder.Configuration.GetConnectionString("GameStore");

builder.Services.AddSqlite<GameStoreContext>(ConnString);

builder.Services.AddValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

Console.WriteLine("Starting GameStore API...");

app.MapGameEndpoints();
app.MapGenresEndpoints();
await app.MigrateDbAsync();

app.Run();
