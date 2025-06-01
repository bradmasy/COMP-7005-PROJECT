using System.Net;
using static Helpers.Constants;

namespace Helpers;

public static class Validator
{
    public static void ValidateServer(string ipAddress, string port)
    {
        ValidateIp(ipAddress);
        ValidatePort(port);
    }

    public static void ValidateClient(string[] args)
    {
        if (args.Length != ValidClientArgs)
            throw new ArgumentException(
                "Invalid amount of arguments. Please provide an argument for IP, Port, Timeout and Max Retry.");

        ValidateIp(args[TargetIp]);
        ValidatePort(args[TargetPort]);

        var timeoutValid = int.TryParse(args[TimeoutArg], out var to);

        if (!timeoutValid) throw new ArgumentException("Invalid timeout number");

        var maxRetryValid = int.TryParse(args[MaxRetry], out var mr);

        if (!maxRetryValid) throw new ArgumentException("Invalid maxRetry number");
    }

    public static void ValidateProxy(string[] args)
    {
        if (args.Length != ValidProxyArgs) throw new ArgumentException("Invalid amount of arguments.");

        ValidateIp(args[ListenIp]);
        ValidatePort(args[ListenPort]);
        ValidateIp(args[ForwardingIp]);
        ValidatePort(args[ForwardingPort]);
        ValidatePercentage(args[ClientDropPercentage]);
        ValidatePercentage(args[ServerDropPercentage]);
        ValidatePercentage(args[ClientDelayChancePercentage]);
        ValidatePercentage(args[ServerDelayChancePercentage]);
        ValidateTime(args[ClientDelayTimeMin]);
        ValidateTime(args[ClientDelayTimeMax]);
        ValidateTime(args[ServerDelayTimeMin]);
        ValidateTime(args[ServerDelayTimeMax]);
    }

    private static void ValidateTime(string time)
    {
        var valid = TimeSpan.TryParse(time, out var parsedTime);
        if (!valid) throw new ArgumentException("Invalid time");
        if (parsedTime.TotalMinutes < 0) throw new ArgumentException("Invalid time, time must be a positive number");
    }

    private static void ValidatePercentage(string percentage)
    {
        var parsedPercentage = double.TryParse(percentage, out var parsedPercentageInt);
        if (!parsedPercentage) throw new ArgumentException("Invalid percentage number");
        if (parsedPercentageInt is < 0 or > 100)
            throw new ArgumentException("Invalid percentage, must be <= 0 or <= 100");
    }

    private static void ValidateIp(string ipAddress)
    {
        var ipValid = IPAddress.TryParse(ipAddress, out var ip);

        if (!ipValid) throw new ArgumentException("Invalid IP address");
    }

    private static void ValidatePort(string port)
    {
        var portValid = int.TryParse(port, out var p);

        if (!portValid) throw new ArgumentException("Invalid port number");
        if (p is < MinPort or > MaxPort) throw new ArgumentException("Invalid port numbers");
    }
}