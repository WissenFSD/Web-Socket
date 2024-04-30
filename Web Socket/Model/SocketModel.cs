using System.Net.Sockets;
using System.Net.WebSockets;

namespace Web_Socket.Model
{
    public class SocketModel
    {
        public string SocketName { get; set; }  
        public WebSocket Socket { get; set; }
    }
}
