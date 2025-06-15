using System.Net;
using System.Net.Sockets;
using Helpers;
using static Helpers.Constants;

namespace Client;

public class Client
{
    private readonly Socket _socket;
    private readonly EndPoint _endPoint;
    private readonly int _timeout;
    private int _maxRetry;
    private int _sequenceNumber;
    private int _ackNumber;
    private readonly Dictionary<int, Packet> _receivedPackets = new();
    private readonly Dictionary<int, Packet> _packets = new();

    public Client(string ipAddress, int port, int timeout, int maxRetry)
    {
        var ipAddress1 = IPAddress.Parse(ipAddress);
        _endPoint = new IPEndPoint(ipAddress1, port);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        _timeout = timeout;
        _maxRetry = maxRetry;
        _socket.ReceiveTimeout = timeout;

        Console.WriteLine($"Client running on port: {((IPEndPoint)_socket.LocalEndPoint).Port}");
    }

    public async Task Run()
    {
        try
        {
            while (true)
            {
                var input = GetUserInput();

                if (string.IsNullOrEmpty(input)) throw new Exception("Error: Please enter a valid argument.");

                // run loop at least one time
                if (_maxRetry == 0) _maxRetry = 1;

                BuildPackets(input);

                _receivedPackets.Clear(); // Clear previous received packets

                await SendAndRetry();

                if (ReceivedExpectedAmountOfPackets()) continue;

                DisplayPackets();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception("Client error", ex);
        }
    }

    public void Shutdown()
    {
        _socket.Close();
    }

    private void DisplayPackets()
    {
        Console.WriteLine($"Received {_receivedPackets.Count} Packets\n");
        foreach (var packet in _receivedPackets.Values)
        {
            Console.WriteLine(packet);
        }
    }

    private bool ReceivedExpectedAmountOfPackets()
    {
        return _receivedPackets.Count != _packets.Count;
    }

    private async Task SendAndRetry()
    {
        for (var i = 0; i < _maxRetry; i++)
        {
            Console.WriteLine($"Sending message {i + 1}/{_maxRetry}\n");

            await Send(isRetry: i > 0);

            // If we've received all packets, break out of the retry loop
            if (_receivedPackets.Count == _packets.Count)
            {
                Console.WriteLine("All packets acknowledged, stopping retries.\n");
                break;
            }

            var receiveTask = ReceiveNPackets();

            var timeout = await IsTimeOut(receiveTask);

            if (timeout) continue;

            if (_receivedPackets.Count != _packets.Count) continue;

            Console.WriteLine("All packets acknowledged, stopping retries.\n");
            break;
        }
    }

    private Task ReceiveNPackets()
    {
        var task = Task.Run(async () =>
        {
            // await each packet sent before confirming data has been received 
            while (_receivedPackets.Count < _packets.Count)
            {
                try
                {
                    await IsCorrectPacket();
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        });

        return task;
    }

    private async Task IsCorrectPacket()
    {
        var data = await Receive();
        var packet = Packet.ConvertBytesToPacket(data);

        // if the matching ack number is different then there is a mismatch, and we skip this packet/drop
        if (packet.AckNumber != _packets[packet.SequenceNumber].SequenceNumber)
        {
            Console.WriteLine(
                $"Warning: Received packet with mismatched acknowledgment number. Expected: {_packets[packet.SequenceNumber].SequenceNumber}, Got: {packet.AckNumber}");
            return;
            //   continue;
        }

        // the key in the dictionary is the sequence number
        _receivedPackets[packet.SequenceNumber] = packet;
    }

    private async Task<bool> IsTimeOut(Task receiveTask)
    {
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(_timeout));
        var completedTask = await Task.WhenAny(receiveTask, timeoutTask);

        if (completedTask != timeoutTask) return false;
        Console.WriteLine($"Timeout occurred after {_timeout} seconds, retrying...\n");
        return true;
    }

    private async Task Send(bool isRetry = false)
    {
        // Only send packets that haven't been acknowledged yet
        var packetsToSend = isRetry
            ? _packets.Where(p => !_receivedPackets.ContainsKey(p.Key))
            : _packets;

        if (!packetsToSend.Any())
        {
            Console.WriteLine("All packets have been acknowledged, no need to send more.\n");
            return;
        }

        Console.WriteLine($"Sending {packetsToSend.Count()} packet(s)...\n");

        var packets = packetsToSend
            .OrderBy(p => p.Value.SequenceNumber)
            .Select(p => p.Value.ConvertPacketToBytes())
            .Select(b => _socket.SendToAsync(b, SocketFlags.None, _endPoint))
            .ToList();

        await Task.WhenAll(packets);
    }

    private static string? GetUserInput()
    {
        Console.WriteLine("Please Type The Payload For The Packet:\n");
        var input = Console.ReadLine();
        Console.WriteLine("\n");
        return input;
    }

    private async Task<byte[]> Receive()
    {
        var buffer = new byte[PacketSize];

        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        var result = await _socket.ReceiveFromAsync(
            new ArraySegment<byte>(buffer), SocketFlags.None, remoteEndPoint);

        var receivedData = buffer[..result.ReceivedBytes];
        return receivedData;
    }

    private void BuildPackets(string data)
    {
        _packets.Clear();

        var packet = new Packet
        {
            SequenceNumber = _sequenceNumber++,
            AckNumber = _ackNumber++,
            Payload = data
        };

        _packets[packet.SequenceNumber] = packet;

        // var index = 0;
        //
        // while (index < data.Length)
        // {
        //     var chunkLength = Math.Min(Chunk, data.Length - index);
        //     var chunk = data.Substring(index, chunkLength);
        //
        //     var packet = new Packet
        //     {
        //         SequenceNumber = _sequenceNumber++,
        //         AckNumber = _ackNumber++,
        //         Payload = chunk
        //     };
        //
        //     _packets[packet.SequenceNumber] = packet;
        //     index += Chunk;
        // }
    }
}