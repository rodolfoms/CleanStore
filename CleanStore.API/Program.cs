using CleanStore.Application;
using CleanStore.Infrastructure;
using CleanStore.Infrastructure.SharedContext.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=localhost;Port=5432;Database=cleanstore;Username=postgres;Password=rodolfo@2026";
//"Server=localhost,1433;Database=cleanstore;User ID=sa;Password=1q2w3e4r@#$;Trusted_Connection=False; TrustServerCertificate=True;";
builder.Services.AddDbContext<AppDbContext>(x =>
    x.UseNpgsql(connectionString, m => m.MigrationsAssembly("CleanStore.API")));
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.MapPost("/accounts", async (
    ISender sender,
    CleanStore.Application.AccountContext.UseCases.Create.Command command) =>
{
    var result = await sender.Send(command);
    return TypedResults.Created($"accounts/{result.Value.Id}", result);
});

app.MapGet("/accounts/{id}", async (
    ISender sender,
    Guid id) =>
{
    var query = new CleanStore.Application.AccountContext.UseCases.Get.Query(id);
    var result = await sender.Send(query);
    return TypedResults.Ok(result);
});

app.Run();