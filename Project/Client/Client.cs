using System.Net;
using System.Net.Sockets;
using System.Text;
using Helpers;

namespace Client;

public class Client
{
    private readonly Socket _socket;
    private readonly EndPoint? _endPoint;
    private int _timeout;
    private int _maxRetry;


    public Client(string ipAddress, int port, int timeout, int maxRetry)
    {
        var ip = IPAddress.Parse(ipAddress);
        _endPoint = new IPEndPoint(ip, port);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _timeout = timeout;
        _maxRetry = maxRetry;
        _socket.ReceiveTimeout = timeout;
    }

    public async Task Run()
    {
        // have sequence number incrementing
        var sequenceNumber = new Random().Next(1000, 9999);
        var ackNumber = 0;

        try
        {
            while (true)
            {
                Console.WriteLine("Send to server:\n");
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) throw new Exception("Error: Please enter a valid argument.");
                await Send(input, sequenceNumber, ackNumber);


                var acknowledgement = await RecurseReceive(0);
                Console.WriteLine("Acknowledgement Received:\n");
                var convertedPacket = ConvertBytesToPacket(acknowledgement);
                Console.WriteLine(acknowledgement);
                Console.WriteLine(convertedPacket.AckNumber);
                Console.WriteLine("Acknowledgement Received:\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception("Client error", ex);
        }
    }

    private async Task Send(string data, int sequenceNumber, int ackNumber)
    {
        var packet = new Packet
        {
            SequenceNumber = sequenceNumber,
            AckNumber = ackNumber,
            Payload = data
        };

        var packetBytes = packet.ConvertPacketToBytes();
        await _socket.SendToAsync(new ArraySegment<byte>(packetBytes), _endPoint);
        Console.WriteLine("SENT");
    }

    private async Task<byte[]> Receive()
    {
        var buffer = new byte[1024];
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        var result = await _socket.ReceiveFromAsync(
            new ArraySegment<byte>(buffer), SocketFlags.None, remoteEndPoint);

        // Trim to actual received length
        var receivedData = buffer[..result.ReceivedBytes];

        Console.WriteLine("RECEIVED");
        return receivedData;
    }

    private async Task<byte[]> RecurseReceive(int retryCount)
    {
        if (retryCount > _maxRetry)
        {
            throw new Exception($"Retry {retryCount} of {_maxRetry}");
        }

        return await Receive();
    }

    private static Packet ConvertBytesToPacket(byte[] packet)
    {
        var sequenceNumber = BitConverter.ToInt32(packet[..4], 0);
        var ackNumber = BitConverter.ToInt32(packet[4..], 0);
        var payload = packet[8..];

        var convertedPayload = Encoding.UTF8.GetString(payload);

        var convertedPacket = new Packet
        {
            SequenceNumber = sequenceNumber,
            AckNumber = ackNumber,
            Payload = convertedPayload
        };

        return convertedPacket;
    }
}