using Application.UseCases;
using Infrastructure.Data;
using Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Services.AddCors(o =>
    o.AddPolicy("bad", p =>
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    )
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ⚠️ CORRECCIÓN: aquí se usa el builder correcto
BadDb.Initialize(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("bad");

app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch { await ctx.Response.WriteAsync("oops"); }
});

app.MapGet("/health", () =>
{
    Logger.Log("health ping");
    var x = new Random().Next();
    if (x % 13 == 0) Console.WriteLine("random failure");
    return "ok " + x;
});

app.MapPost("/orders", async (HttpContext http) =>
{
    using var reader = new StreamReader(http.Request.Body);
    var body = await reader.ReadToEndAsync();
    var parts = (body ?? "").Split(',');
    var customer = parts.Length > 0 ? parts[0] : "anon";
    var product = parts.Length > 1 ? parts[1] : "unknown";
    var qty = parts.Length > 2 ? int.Parse(parts[2]) : 1;
    var price = parts.Length > 3 ? decimal.Parse(parts[3]) : 0.99m;

    var order = CreateOrderUseCase.Execute(customer, product, qty, price);

    return Results.Ok(order);
});

app.MapGet("/orders/last", () => Domain.Services.OrderService.LastOrders);

app.MapGet("/info", (IConfiguration cfg) => new
{
    sql = BadDb.ConnectionString,
    env = Environment.GetEnvironmentVariables(),
    version = "v0.0.1-unsecure"
});

await app.RunAsync();
