using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Middleware;
using TaskManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Serviços (equivalente aos "providers/modules" do Nest) ----------

// Controllers + enum serializado como texto ("EmAndamento" em vez de 1)
builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Task Manager API", Version = "v1" });
});

// Banco de dados (EF Core + SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Tratamento global de erros (retorna ProblemDetails padronizado)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// CORS para o React (Vite roda em 5173 por padrão)
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Injeção de dependência dos seus services (descomente quando criar):
builder.Services.AddScoped<ITarefaService, TarefaService>();

var app = builder.Build();

// ---------- Pipeline de middlewares (a ordem importa) ----------

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(); // disponível em /swagger

app.UseCors("Frontend");

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

// Aplica as migrations automaticamente ao subir (facilita para quem avalia)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
