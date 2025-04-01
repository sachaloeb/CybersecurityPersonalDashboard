using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add CORS, etc.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add controllers
builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowLocalhost3000");

// If environment is Development, show OpenAPI docs
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/status", () =>
    {
        var controller = new NetworkController();
        return Results.Ok(controller.GetStatus());
    })
    .WithMetadata(new HttpGetAttribute());

app.MapPost("/api/evaluate", async (HttpRequest request) =>
    {
        using var reader = new StreamReader(request.Body);
        var requestBody = await reader.ReadToEndAsync();

        using var jsonDoc = JsonDocument.Parse(requestBody);
        var password = jsonDoc.RootElement.GetProperty("password").GetString();

        if (string.IsNullOrEmpty(password))
        {
            return Results.BadRequest("Password is required.");
        }

        var controller = new PasswordController();
        return Results.Ok(controller.Evaluate(password));
    })
    .WithMetadata(new HttpPostAttribute());

app.Run();