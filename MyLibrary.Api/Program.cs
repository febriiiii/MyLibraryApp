using Microsoft.OpenApi;
using MyLibrary.Core.Interfaces;
using MyLibrary.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.Register(); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sample Library API",
        Version = "v1",
        Description = "Clean Architecture with DDD & MediatR Style"
    });
});

var app = builder.Build();

// --- PIPELINE / MIDDLEWARE ---

// hanya untuk test websocket
app.UseDefaultFiles(); // Agar otomatis buka index.html saat akses root /
app.UseStaticFiles();  // Agar bisa baca file di wwwroot

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample Library V1");
    c.RoutePrefix = "swagger"; // Akses via http://localhost:5000/swagger
});

app.MapControllers();

app.UseWebSockets();
app.Map("/ws", async (HttpContext context, IWebSocketHandler wsHandler) => {
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await wsHandler.HandleAsync(webSocket);
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

app.Run();