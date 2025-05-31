using Helpers;
using static Helpers.Constants;

namespace Client;

public class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Validator.ValidateClient(args);

            var targetIp = args[TargetIp];
            var targetPort = args[TargetPort];
            var timeout = args[TimeoutArg];
            var maxRetry = args[MaxRetry];


            var client = new Client(targetIp, int.Parse(targetPort), int.Parse(timeout), int.Parse(maxRetry));
            await client.Send("hello");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}