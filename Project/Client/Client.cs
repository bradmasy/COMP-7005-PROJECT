using System.Net;
using System.Net.Sockets;
using Helpers;

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
        var packet = new Packet
        {
            SequenceNumber = 0,
            AckNumber = 0,
            Payload = data
        };

        var packetBytes = packet.ConvertPacketToBytes();
        await _socket.SendToAsync(new ArraySegment<byte>(packetBytes), _endPoint);
        Console.WriteLine("SENT");
        
        
    }
}