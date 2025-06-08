using System.Net;
using System.Net.Sockets;
using System.Text;
using Helpers;
using static Helpers.Constants;

namespace Server;

public class Server
{
    private readonly int _port;
    private readonly IPAddress _ip;
    private readonly Socket _serverSocket;
    private readonly EndPoint _remoteEndPoint;

    public Server(string ipAddress, int port)
    {
        _ip = IPAddress.Parse(ipAddress);
        _port = port;
        _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _remoteEndPoint = new IPEndPoint(IPAddress.Any, 80); // Accept from any IP and Port
    }

    public async Task Run()
    {
        Bind();

        while (true)
        {
            var buffer = new byte[1024];

            try
            {
                EndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);

                var datagram =
                    await _serverSocket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None,
                        senderEndPoint);
                Console.WriteLine("Client connected\n");

                var dataGramToPacket = Packet.ConvertBytesToPacket(buffer.ToArray());
            
                Console.WriteLine(dataGramToPacket.ToString());
                
                // send the ack packet back, no payload needed
                var updatedAckNumber = dataGramToPacket.AckNumber + dataGramToPacket.SequenceNumber;
                var ackPacket = new Packet
                {
                    AckNumber = updatedAckNumber,
                    SequenceNumber = dataGramToPacket.SequenceNumber + dataGramToPacket.Payload.Length,
                    Payload = "Received",
                    EndPoint = dataGramToPacket.EndPoint,
                    Port = dataGramToPacket.Port
                };
                Console.WriteLine("\n");
                Console.WriteLine(ackPacket.ToString());

                await _serverSocket.SendToAsync(new ArraySegment<byte>(ackPacket.ConvertPacketToBytes()),
                    datagram.RemoteEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
            }
        }
    }

    private void Bind()
    {
        var ipEndpoint = new IPEndPoint(_ip, _port);
        _serverSocket.Bind(ipEndpoint);
    }
}