using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

public class Client
{
    private Socket _socket;
    private EndPoint _endPoint;


    public Client(string ipAddress, int port, int timeout, int maxRetry)
    {
        var ip = IPAddress.Parse(ipAddress);
        _endPoint = new IPEndPoint(ip, port);
        
        
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    public async Task Send(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        await _socket.SendToAsync(new ArraySegment<byte>(bytes), _endPoint);
        Console.WriteLine("SENT");
        
        
    }
}