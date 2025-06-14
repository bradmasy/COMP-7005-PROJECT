using System.Net;
using System.Net.Sockets;
using Helpers;
using static Helpers.Constants;

namespace Server;

public class Server(string ipAddress, int port)
{
    private readonly IPAddress _ip = IPAddress.Parse(ipAddress);
    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private readonly SortedDictionary<int, Packet> _receivedPackets = new();
    private int _expectedSequenceNumber = 0;

    public async Task Run()
    {
        Bind();

        Console.WriteLine($"Server listening on {_ip}:{port}");

        var buffer = new byte[PacketSize];

        while (true)
        {
            try
            {
                EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                var result = await _socket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, sender);

                var receivedData = buffer[..result.ReceivedBytes];
                var packet = Packet.ConvertBytesToPacket(receivedData);

                ResetExpectedSequenceNewConnection(packet);

                // If it is not the packet, skip 
                var duplicate = IsReceivedPacket(packet);
                var nextPacket = duplicate ?? packet;

                if (!IsNextPacket(nextPacket)) continue;

                Console.WriteLine("\n-- Client Message Received --\n");
                Console.WriteLine(nextPacket.ToString());

                var ackPacket = CreateAcknowledgement(nextPacket);

                await _socket.SendToAsync(
                    new ArraySegment<byte>(ackPacket.ConvertPacketToBytes()),
                    result.RemoteEndPoint
                );

                // Update expected sequence number
                _expectedSequenceNumber = nextPacket.SequenceNumber + 1;

                // add the packet to track
                _receivedPackets[nextPacket.SequenceNumber] = nextPacket;

                Console.WriteLine($"ACK sent for Seq: {ackPacket.SequenceNumber}, Ack: {ackPacket.AckNumber}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling message:");
                Console.WriteLine(ex.Message);
            }
        }
    }

    private Packet? IsReceivedPacket(Packet packet)
    {
        var exists = _receivedPackets.TryGetValue(packet.SequenceNumber, out var foundPacket);
        if (exists) return foundPacket;

        Console.WriteLine("Received packet not found");
        return null;
    }

    private bool IsNextPacket(Packet packet)
    {
        if (packet.SequenceNumber >= _expectedSequenceNumber) return true;

        Console.WriteLine(
            $"Ignored duplicate or out-of-order packet. Seq: {packet.SequenceNumber}, Expected: {_expectedSequenceNumber}");
        return false;
    }

    private void ResetExpectedSequenceNewConnection(Packet packet)
    {
        // Check if this is a new client connection (sequence number is 0)
        if (packet.SequenceNumber != 0) return;

        Console.WriteLine("New client connection detected, resetting sequence number.");
        _expectedSequenceNumber = 0;
        _receivedPackets.Clear();
    }

    private static Packet CreateAcknowledgement(Packet received)
    {
        return new Packet
        {
            EndPoint = received.EndPoint,
            Port = received.Port,
            SequenceNumber = received.SequenceNumber,
            AckNumber = received.AckNumber,
            Payload = $"Received Packet: {received.SequenceNumber}"
        };
    }

    private void Bind()
    {
        var endpoint = new IPEndPoint(_ip, port);
        _socket.Bind(endpoint);
    }
}