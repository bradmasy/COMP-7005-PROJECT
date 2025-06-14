using System.Net;
using System.Net.Sockets;
using Helpers;

namespace Proxy;

public class Proxy
{
    private readonly Socket _socket;

    private readonly EndPoint _serverEndPoint;

    private readonly double _clientDropPercent;
    private readonly double _serverDropPercent;
    private readonly double _clientDelayPercent;
    private readonly double _serverDelayPercent;
    private readonly double _clientDelayTimeMin;
    private readonly double _serverDelayTimeMax;
    private readonly double _clientDelayTimeMax;
    private readonly double _serverDelayTimeMin;

    public Proxy(
        string listenIp,
        int listenPort,
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

        _clientDropPercent = clientDropPercent;
        _serverDropPercent = serverDropPercent;
        _clientDelayPercent = clientDelayPercent;
        _serverDelayPercent = serverDelayPercent;
        _clientDelayTimeMin = clientDelayTimeMin;
        _serverDelayTimeMax = serverDelayTimeMax;
        _clientDelayTimeMax = clientDelayTimeMax;
        _serverDelayTimeMin = serverDelayTimeMin;

        var targetIp1 = IPAddress.Parse(targetIp);
        var listenIp1 = IPAddress.Parse(listenIp);

        EndPoint proxyEndPoint = new IPEndPoint(listenIp1, listenPort);

        _serverEndPoint = new IPEndPoint(targetIp1, targetPort);
        _socket.Bind(proxyEndPoint);

        Console.WriteLine($"Proxy started on {proxyEndPoint}\n");
    }

    public async Task Run()
    {
        try
        {
            var buffer = new byte[1024];
            EndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                var result = await _socket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, sender);
                var packet = Packet.ConvertBytesToPacket(buffer);

                DisplayIncomingTraffic(result, packet);

                Task.Run(() => ProcessPacket(result, packet));
           
                buffer.AsSpan().Clear();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void Shutdown()
    {
        _socket.Close();
    }
    
    
    private async Task ProcessPacket(SocketReceiveFromResult result, Packet packet)
    {
        // append the incoming IP of the packet as the endpoint for the returning packet to help redirect to client
        var isToClient = !result.RemoteEndPoint.Equals(_serverEndPoint);

        AssignClientDestination(isToClient, result, packet);

        // If the incoming packet is from the static server, we will forward to the client endpoint
        var forwardTo = result.RemoteEndPoint.Equals(_serverEndPoint)
            ? new IPEndPoint(packet.EndPoint, packet.Port)
            : _serverEndPoint;

        var isDropped = IsDropped(isToClient
            ? _clientDropPercent
            : _serverDropPercent);

        if (isDropped)
        {
            Console.WriteLine($"\n--Dropping Packet From {(isToClient ? "Client" : "Server")}--\n");
            return;
        }

        var isDelayed =
            IsDelayed(isToClient ? _clientDelayPercent : _serverDelayPercent);

        if (isDelayed)
        {
            var delayTime = CalculateDelay(isToClient);
            await Task.Delay(TimeSpan.FromSeconds(delayTime)); // Delay packet if needed
        }

        Console.WriteLine($"\n--Forwarding Packet From {result.RemoteEndPoint} to {forwardTo}--\n");

        await Send(packet, forwardTo); // Send packet to its destination
    }

    private async Task Send(Packet packet, EndPoint endPoint)
    {
        await _socket.SendToAsync(new ArraySegment<byte>(packet.ConvertPacketToBytes()), SocketFlags.None,
            endPoint);
    }

    private static void AssignClientDestination(bool isClient, SocketReceiveFromResult result, Packet packet)
    {
        if (!isClient) return;

        packet.EndPoint = ((IPEndPoint)result.RemoteEndPoint).Address;
        packet.Port = ((IPEndPoint)result.RemoteEndPoint).Port;
    }

    private static void DisplayIncomingTraffic(SocketReceiveFromResult result, Packet packet)
    {
        Console.WriteLine($"Incoming Packet From {result.RemoteEndPoint}\n");
        Console.WriteLine(packet);
    }

    private double CalculateDelay(bool isToClient)
    {
        Console.WriteLine($"\n--Delaying Packet From {(isToClient ? "Client" : "Server")}--\n");

        var min = isToClient ? _clientDelayTimeMin : _serverDelayTimeMin;
        var max = isToClient ? _clientDelayTimeMax : _serverDelayTimeMax;
        var delayTime = CalculateDelayTime(min, max);

        Console.WriteLine($"--Delaying Packet By {delayTime}s--\n");

        return delayTime;
    }

    private static bool IsDropped(double dropPercentage)
    {
        var random = new Random();
        var percent = random.NextDouble() * 100;

        Console.WriteLine(
            $"Drop Percentage {dropPercentage}% | Calculated Drop Percentage {percent}% {percent < dropPercentage}");
        return percent < dropPercentage;
    }

    private static bool IsDelayed(double delayPercentage)
    {
        var random = new Random();
        var percent = random.NextDouble() * 100;

        Console.WriteLine(
            $"Delay Percentage {delayPercentage}% | Calculated Delay Percentage {percent}% {percent < delayPercentage}");
        return percent < delayPercentage;
    }

    private static double CalculateDelayTime(double min, double max)
    {
        var random = new Random();
        return random.NextDouble() * (max - min) + min;
    }
}