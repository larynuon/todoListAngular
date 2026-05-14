using todoListAngular.Server.Todos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register application services
builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();  // In-memory storage (singleton for app lifetime)
builder.Services.AddScoped<TodoService>();  // Business logic service (scoped per request)

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
