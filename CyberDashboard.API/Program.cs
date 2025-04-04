using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddScoped<IPortScannerService, PortScannerService>(); // ✅ Register service
builder.Services.AddHttpClient<ZapScannerService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowLocalhost3000");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

app.MapGet("/api/security-overview", () =>
    {
        var controller = new SecurityController();
        return Results.Ok(controller.Overview());
    })
    .WithMetadata(new HttpGetAttribute());

app.MapControllers();

app.Run();
