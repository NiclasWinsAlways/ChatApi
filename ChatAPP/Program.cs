using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Models;
using ChatApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR services
builder.Services.AddSignalR();

// Register the DbContext with the connection string from appsettings.json
builder.Services.AddDbContext<ChatAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the IPasswordHasher<User> service
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Register the repository service
builder.Services.AddScoped<IChatRepository, ChatRepository>();

// Add CORS policy to allow Angular app and SignalR connections
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder => builder.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()  // This allows cookies and authentication headers for SignalR
                          .SetIsOriginAllowedToAllowWildcardSubdomains()
                          .WithExposedHeaders("Authorization")); // Expose headers like Authorization
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS policy first in the middleware pipeline
app.UseCors("AllowAngularApp");

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Apply authentication middleware
app.UseAuthentication();  // This ensures user authentication takes place before any authorization checks

// Custom Middleware to log headers and help debug CORS issues
app.Use(async (context, next) =>
{
    // Log request headers for debugging
    Console.WriteLine("Incoming Request Headers:");
    foreach (var header in context.Request.Headers)
    {
        Console.WriteLine($"{header.Key}: {header.Value}");
    }

    // Handle OPTIONS requests manually (for preflight checks)
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.StatusCode = 204; // No content
        return;
    }

    await next.Invoke();
});

// Apply authorization middleware
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map SignalR hubs
app.MapHub<ChatHub>("/chathub");

app.Run();
