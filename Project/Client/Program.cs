using Helpers;
using static Helpers.Constants;

namespace Client;

public class Program
{
    static async Task Main(string[] args)
    {
        var targetIp = args[TargetIp];
        var targetPort = args[TargetPort];
        var timeout = args[TimeoutArg];
        var maxRetry = args[MaxRetry];

        Validator.ValidateClient(targetIp, targetPort, timeout, maxRetry);

        var client = new Client(targetIp, int.Parse(targetPort), int.Parse(timeout), int.Parse(maxRetry));
    }
}