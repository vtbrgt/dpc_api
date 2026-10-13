using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Middleware;
using TaskManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Serviços  ----------

// Controllers
builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Task Manager API", Version = "v1" });
});

// Banco de dados (EF Core + PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.EnableRetryOnFailure()));

// Tratamento global de erros
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// CORS para o React (local e deploy)
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;
                return uri.Host == "localhost" || uri.Host == "127.0.0.1" || allowedOrigins.Contains(origin);
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Injeção de dependência dos services:
builder.Services.AddScoped<ITarefaService, TarefaService>();

// Render define a variável PORT. Localmente ela não existe e vale o launchSettings (5070)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var app = builder.Build();

// ---------- Pipeline de middlewares ----------

app.UseCors("Frontend");
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

// Aplica as migrations automaticamente ao subir
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
