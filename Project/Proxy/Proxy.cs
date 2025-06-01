using System.Net;
using System.Net.Sockets;

namespace Proxy;

public class Proxy
{
    private readonly Socket _socket;
    private readonly EndPoint _remoteEndPoint;

    private readonly IPAddress _ip;
    private readonly int _port;

    public Proxy(
        string ip,
        int port,
        string targetIp,
        int targetPort,
        double clientDropPercent,
        double serverDropPercent,
        double clientDelayPercent,
        double serverDelayPercent,
        int clientDelayTimeMin,
        int clientDelayTimeMax,
        int serverDelayTimeMin,
        int serverDelayTimeMax)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0); // Accept from any IP and Port
        _ip = IPAddress.Parse(ip);
        _port = port;
    }

    public async Task Run()
    {
        try
        {
            while (true)
            {
                Bind();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void Bind()
    {
        var ipEndpoint = new IPEndPoint(_ip, _port);
        _socket.Bind(ipEndpoint);
    }
}