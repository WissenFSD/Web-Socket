using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();


app.Use(async (ctx, nextMessage) =>
{

    Console.WriteLine("Web Socket dinliyor");

    if (ctx.Request.Path == "wissen")
    {

        if (ctx.WebSockets.IsWebSocketRequest)
        {
            var socket = await ctx.WebSockets.AcceptWebSocketAsync();
            await Speak(ctx, socket);
         }
    }

});

app.Run();


async Task Speak(HttpContext context, WebSocket socket)
{
    var bite = new byte[1024];

    WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(bite), CancellationToken.None);
    while (!result.CloseStatus.HasValue)
    {
        var inComingMessage = Encoding.UTF8.GetString(bite, 0, result.Count);
        Console.WriteLine("Clientten gelen : " + inComingMessage);
        var rnd = new Random();
        var random = rnd.Next(1, 100);
        string message = string.Format("Numaran {0}", random.ToString());
        byte[] outGoingMessage = Encoding.UTF8.GetBytes(message);


        await socket.SendAsync(new ArraySegment<byte>(outGoingMessage, 0, outGoingMessage.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

        result = await socket.ReceiveAsync(new ArraySegment<byte>, bite, CancellationToken.None);

    }
    await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

}