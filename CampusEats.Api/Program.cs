using CampusEats.Api.Data;
using CampusEats.Api.Services;
using Microsoft.EntityFrameworkCore;

var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFile))
{
    DotNetEnv.Env.Load(envFile);
}

var builder = WebApplication.CreateBuilder(args);

var frontendOrigin = builder.Configuration["FrontendOrigin"];
if (string.IsNullOrWhiteSpace(frontendOrigin))
{
    if (builder.Environment.IsProduction())
    {
        throw new InvalidOperationException("Set FrontendOrigin to the deployed frontend's exact HTTPS origin.");
    }

    frontendOrigin = "http://localhost:5173";
}

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Set ConnectionStrings__Default in CampusEats.Api/.env or in the hosting platform's environment variables.");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MenuDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
    policy.WithOrigins(frontendOrigin)
          .AllowAnyHeader()
          .AllowAnyMethod()));

builder.Services.AddScoped<IMenuService, MenuService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();