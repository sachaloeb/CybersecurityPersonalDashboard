using System.Net.Sockets;

public class PortScannerService : IPortScannerService
{
    public async Task<PortScanResult> ScanPortsAsync(PortScanRequest request)
    {
        var openPorts = new List<int>();
        var tasks = new List<Task>();

        for (int port = request.StartPort; port <= request.EndPort; port++)
        {
            int currentPort = port;
            tasks.Add(Task.Run(async () =>
            {
                if (await IsPortOpenAsync(request.Ip, currentPort))
                {
                    lock (openPorts)
                    {
                        openPorts.Add(currentPort);
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);
        openPorts.Sort();

        return new PortScanResult
        {
            Ip = request.Ip,
            OpenPorts = openPorts
        };
    }

    private static async Task<bool> IsPortOpenAsync(string ip, int port)
    {
        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(ip, port);
            var result = await Task.WhenAny(connectTask, Task.Delay(500));
            return result == connectTask && client.Connected;
        }
        catch
        {
            return false;
        }
    }
}