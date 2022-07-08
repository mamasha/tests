using MsShapeEditor;

void AddDependencies(IServiceCollection services)
{
    services
        .AddTransient<IBoard, Board>()
        .AddSingleton<IUndo, Undo>()
        .AddSingleton<IRepository, Repository>();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
AddDependencies(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
