using System.Net;
using System.Net.Sockets;

namespace Client;

public class Client
{
    private readonly int _port;
    private readonly IPAddress _ip;
    private Socket _socket;

    public Client(string ipAddress, int port, int timeout, int maxRetry)
    {
        _ip = IPAddress.Parse(ipAddress);
        _port = port;
        
        
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    public async Task Connect()
    {
        var endPoint = new IPEndPoint(_ip, _port);
        await _socket.ConnectAsync(endPoint);
    }
}