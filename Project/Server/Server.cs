using System.Net;
using System.Net.Sockets;
using static Helpers.Constants;

namespace Server;

public class Server
{
    private readonly int _port;
    private readonly IPAddress _ip;
    private readonly Socket _serverSocket;

    public Server(string ipAddress, int port)
    {
        _ip = IPAddress.Parse(ipAddress);
        _port = port;
        _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    public async Task Run()
    {
        while (true)
        {
            try
            {
                BindAndListen();
                
                var client = await _serverSocket.AcceptAsync();
                Console.WriteLine(client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    private void BindAndListen()
    {
        var ipEndpoint = new IPEndPoint(_ip, _port);
        _serverSocket.Bind(ipEndpoint);
        _serverSocket.Listen(Connections);
    }
}