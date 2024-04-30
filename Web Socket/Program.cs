using System.Net.WebSockets;
using System.Text;
using Web_Socket.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();


var host = new WebHostBuilder()
    .UseUrls("http://localhost:1903");

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


List<SocketModel> webSockets = new List<SocketModel>();
app.Use(async (ctx, nextMessage) =>
{

    //HttpContext
    if (ctx.Request.Path == "/wissen")
    {

        if (ctx.WebSockets.IsWebSocketRequest)
        {
            var socket = await ctx.WebSockets.AcceptWebSocketAsync();
            
            webSockets.Add(new SocketModel() { Socket=socket, SocketName=ctx.Request.QueryString.ToString()});
            await Speak(ctx, socket);
        }
        else
        {
            ctx.Response.StatusCode = 401;
        }
    }
    else
    {
        await nextMessage();
    }

});
app.UseFileServer();

app.Run();


async Task Speak(HttpContext context, WebSocket socket)
{
    while (true)
    {
        var bite = new byte[1024];

        WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(bite), CancellationToken.None);

        // Sonsuz döngü 

        // Sockete query string ile parametre gönderdik, sebebi, mesaj gönderen kiþinin kendi gönderdiði mesajý geri almasýydý.
        foreach (var item in webSockets.Where(s=>s.SocketName!=context.Request.QueryString.ToString()))
        {
            item.Socket.SendAsync(new ArraySegment<byte>(bite, 0, bite.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
        }


        
    }
}