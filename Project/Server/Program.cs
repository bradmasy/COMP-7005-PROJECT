using static Helpers.Constants;
using Helpers;

namespace Server;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var ip = args[ServerIp];
            var port = args[ServerPort];
            Console.WriteLine($"Server IP: {ip}");
            Console.WriteLine($"Server Port: {port}");
            Validator.ValidateServer(ip, port);

            var server = new Server(ip, int.Parse(port));

            // Run the server
            await server.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}