using System.Net;
using System.Net.Sockets;
using Helpers;

namespace Proxy;

public class Proxy
{
    private readonly Socket _socket;

    private readonly EndPoint _clientEndPoint;
    private readonly EndPoint _serverEndPoint;

    private readonly IPAddress _listenIp;
    private readonly IPAddress _targetIp;

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
        Console.WriteLine("Proxy started");
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        _clientDropPercent = clientDropPercent;
        _serverDropPercent = serverDropPercent;
        _clientDelayPercent = clientDelayPercent;
        _serverDelayPercent = serverDelayPercent;
        _clientDelayTimeMin = clientDelayTimeMin;
        _serverDelayTimeMax = serverDelayTimeMax;
        _clientDelayTimeMax = clientDelayTimeMax;
        _serverDelayTimeMin = serverDelayTimeMin;

        _targetIp = IPAddress.Parse(targetIp);
        _listenIp = IPAddress.Parse(listenIp);

        _clientEndPoint = new IPEndPoint(_listenIp, listenPort);
        _serverEndPoint = new IPEndPoint(_targetIp, targetPort);

        _socket.Bind(_clientEndPoint);
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
                Console.WriteLine($"incoming packet from: {result.RemoteEndPoint}");
                Console.WriteLine($"is it the client: {result.RemoteEndPoint.Equals(_clientEndPoint)}");
                var packet = Packet.ConvertBytesToPacket(buffer);
                Console.WriteLine(packet.ToString());
                var forwardTo = result.RemoteEndPoint.Equals(_clientEndPoint) ? _serverEndPoint : _clientEndPoint;
                Console.WriteLine($"forwarding to: {forwardTo}");
                var isToClient = forwardTo.Equals(_clientEndPoint);
                
                var isDropped = IsDropped(isToClient
                    ? _clientDropPercent
                    : _serverDropPercent);

                if (isDropped) continue;

                var isDelayed =
                    IsDelayed(isToClient ? _clientDelayPercent : _serverDelayPercent);

                if (isDelayed)
                {
                    var min = isToClient ? _clientDelayTimeMin : _serverDelayTimeMin;
                    var max = isToClient ? _clientDelayTimeMax : _serverDelayTimeMax;
                    var delayTime = CalculateDelayTime(min, max);
                    var timeSpan = TimeSpan.FromMilliseconds(delayTime);

                    Task.Delay(timeSpan).Wait();
                }

                await _socket.SendToAsync(new ArraySegment<byte>(buffer), SocketFlags.None, forwardTo);
                buffer.AsSpan().Clear();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Console.WriteLine(ex.Message);
        }
    }

    private static bool IsDropped(double dropPercentage)
    {
        var random = new Random();
        var percent = random.NextDouble() * 100;

        return percent < dropPercentage;
    }

    private static bool IsDelayed(double delayPercentage)
    {
        var random = new Random();
        var percent = random.NextDouble() * 100;

        return percent < delayPercentage;
    }

    private static double CalculateDelayTime(double min, double max)
    {
        var random = new Random();
        return random.NextDouble() * (max - min) + min;
    }
}