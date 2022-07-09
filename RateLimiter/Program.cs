RateLimiterTests.RunUnitTests();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var threshold = args.Length >= 1 ? int.Parse(args[0]) : 10;
var ttl = TimeSpan.FromMilliseconds(args.Length >= 2 ? int.Parse(args[1]) : 1000);

var config = new RateLimiter.Config { 
    Threshold = threshold, 
    Ttl = ttl 
};

builder.Services.AddSingleton<RateLimiter.Config>(config);
builder.Services.AddSingleton<IRateLimiter, RateLimiter>();
builder.Services.AddTransient<IStringHasher, StringHasher>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
