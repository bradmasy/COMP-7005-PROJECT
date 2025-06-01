using Helpers;
using static Helpers.Constants;

namespace Proxy;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Validator.ValidateProxy(args);

            var listenIp = args[ListenIp];
            var listenPort = args[ListenPort];
            var targetIp = args[ForwardingIp];
            var targetPort = args[ForwardingPort];
            var clientDropPercent = args[ClientDropPercentage];
            var serverDropPercent = args[ServerDropPercentage];
            var clientDelayChancePercent = args[ClientDelayChancePercentage];
            var serverDelayChancePercent = args[ServerDelayChancePercentage];
            var clientDelayTimeMin = args[ClientDelayTimeMin];
            var clientDelayTimeMax = args[ClientDelayTimeMax];
            var serverDelayTimeMax = args[ServerDelayTimeMax];
            var serverDelayTimeMin = args[ServerDelayTimeMin];

            var proxy = new Proxy(listenIp, int.Parse(listenPort), targetIp, int.Parse(targetPort),
                double.Parse(clientDropPercent),
                double.Parse(serverDropPercent),
                double.Parse(clientDelayChancePercent),
                double.Parse(serverDelayChancePercent),
                int.Parse(clientDelayTimeMin),
                int.Parse(clientDelayTimeMax),
                int.Parse(serverDelayTimeMax),
                int.Parse(serverDelayTimeMin));

            await proxy.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}