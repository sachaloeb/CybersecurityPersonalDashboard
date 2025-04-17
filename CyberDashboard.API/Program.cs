using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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

builder.Services.AddControllers()
           .AddJsonOptions(opts =>
           {
              // Register the System.Text.Json converter that emits enum names instead of numbers
                   opts.JsonSerializerOptions.Converters.Add(
                           new System.Text.Json.Serialization.JsonStringEnumConverter()
                       ); 
           });
builder.Services.AddScoped<IPortScannerService, PortScannerService>();
builder.Services.AddHttpClient<ZapScannerService>();
builder.Services.AddSingleton<ThreatLogService>();
builder.Services.AddScoped<ThreatSimulationService>();
builder.Services.AddScoped<SshBruteForceService>();
// 1) Add PostgreSQL for analytics / reports
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// 2) Add MongoLogService for storing logs in MongoDB
builder.Services.AddSingleton<MongoLogService>();
//builder.Services.AddDbContext<AppDbContext>(options =>
   // options.UseNpgsql("Host=localhost;Port=5432;Database=cyber_dashboard;Username=sloeb;Password=eDceBn3h"));
//const string connectionUri = "mongodb+srv://sachaloeb:xfBD6K8Vpm6GTmNx@cluster0.3fjol.mongodb.net/CyberDashboardNoSQL?retryWrites=true&w=majority&appName=Cluster0";
//var settings = MongoClientSettings.FromConnectionString(connectionUri);
// Set the ServerApi field of the settings object to set the version of the Stable API on the client
//settings.ServerApi = new ServerApi(ServerApiVersion.V1);
// Create a new client and connect to the server
//var client = new MongoClient(settings);
// Send a ping to confirm a successful connection
//try {
  //var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
  //Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
//} catch (Exception ex) {
  //Console.WriteLine(ex);
//}
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue; // risky if you have no other checks
});
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
