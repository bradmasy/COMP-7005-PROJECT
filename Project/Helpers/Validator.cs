using System.Net;
using static Helpers.Constants;

namespace Helpers;

public static class Validator
{
    public static void ValidateServer(string ipAddress, string port)
    {
        var ipValid = IPAddress.TryParse(ipAddress, out var ip);

        if (!ipValid) throw new ArgumentException("Invalid IP address");

        var portValid = int.TryParse(port, out var p);

        if (!portValid)
            throw new ArgumentException($"Invalid port. Port must be an integer between {MinPort} and {MaxPort}");

        if (p is < MinPort or > MaxPort) throw new ArgumentException("Invalid port numbers");
    }

    public static void ValidateClient(string[] args)
    {
        if (args.Length != 4) throw new ArgumentException("Invalid amount of arguments. Please provide an argument for IP, Port, Timeout and Max Retry.");
        var ipValid = IPAddress.TryParse(args[TargetIp], out var ip);

        if (!ipValid) throw new ArgumentException("Invalid IP address");

        var portValid = int.TryParse(args[TargetPort], out var p);

        if (!portValid)
            throw new ArgumentException($"Invalid port. Port must be an integer between {MinPort} and {MaxPort}");

        if (p is < MinPort or > MaxPort) throw new ArgumentException("Invalid port numbers");

        var timeoutValid = int.TryParse(args[TimeoutArg], out var to);

        if (!timeoutValid) throw new ArgumentException("Invalid timeout number");

        var maxRetryValid = int.TryParse(args[MaxRetry], out var mr);

        if (!maxRetryValid) throw new ArgumentException("Invalid maxRetry number");
    }
}