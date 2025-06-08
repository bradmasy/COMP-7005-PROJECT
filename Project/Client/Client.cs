using System.Net;
using System.Net.Sockets;
using System.Text;
using Helpers;

namespace Client;

public class Client
{
    private readonly Socket _socket;
    private readonly EndPoint _endPoint;
    private int _timeout;
    private readonly IPAddress _ipAddress;
    private int _maxRetry;
    private readonly int _port;


    public Client(string ipAddress, int port, int timeout, int maxRetry)
    {
        _port = port;
        _ipAddress = IPAddress.Parse(ipAddress);
        _endPoint = new IPEndPoint(_ipAddress, port);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        _timeout = timeout;
        _maxRetry = maxRetry;
        _socket.ReceiveTimeout = timeout;
    }

    public async Task<byte[]> Run()
    {
        // have sequence number incrementing
        var sequenceNumber = new Random().Next(1000, 9999);
        var ackNumber = 0;

        try
        {
            while (true)
            {
                Console.WriteLine("Please Type The Payload For The Packet:\n");
                var input = Console.ReadLine();
                Console.WriteLine("\n");

                if (string.IsNullOrEmpty(input)) throw new Exception("Error: Please enter a valid argument.");

                byte[]? received = null;

                for (var i = 0; i < _maxRetry; i++)
                {
                    Console.WriteLine($"Sending message {i + 1}/{_maxRetry}");
                    await Send(input, sequenceNumber, ackNumber);

                    var receiveTask = Receive();
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(_timeout));

                    var success = await Task.WhenAny(receiveTask, timeoutTask);

                    if (success != receiveTask) continue;
                    
                    received = receiveTask.Result;
                    break;
                }

                if (received == null) throw new Exception("Error: No packet received.");


                var convertedPacket = Packet.ConvertBytesToPacket(received);
                Console.WriteLine(convertedPacket.ToString());

                ackNumber++;
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
        var bytesToPacket = Packet.ConvertBytesToPacket(packetBytes);
        Console.WriteLine(bytesToPacket.ToString());
        await _socket.SendToAsync(new ArraySegment<byte>(packetBytes), _endPoint);
    }

    private async Task<byte[]> Receive()
    {
        var buffer = new byte[1024];
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        var result = await _socket.ReceiveFromAsync(
            new ArraySegment<byte>(buffer), SocketFlags.None, remoteEndPoint);

        var receivedData = buffer[..result.ReceivedBytes];

        Console.WriteLine("RECEIVED");
        return receivedData;
    }

    // private async Task<byte[]> RecurseReceive(int retryCount)
    // {
    //     if (retryCount > _maxRetry)
    //     {
    //         throw new Exception($"Retry {retryCount} of {_maxRetry}");
    //     }
    //
    //     return await Receive();
    // }
}