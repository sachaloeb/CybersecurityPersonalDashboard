using System.Diagnostics;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using MongoDB.Driver;

public class PortScannerService : IPortScannerService
{
    // PortScannerService.cs
    private readonly IMongoCollection<DetailedPortScanResult> _scanHistoryCollection;

    public PortScannerService(MongoLogService mongoLogService)
    {
        // we can now call mongoLogService.GetCollection<T>()
        _scanHistoryCollection = mongoLogService.GetCollection<DetailedPortScanResult>("PortScanHistory");
    }

    public async Task<DetailedPortScanResult> ScanPortsDetailedAsync(PortScanRequest request)
    {
        // 1) Start the clock to measure scan duration
        var stopwatch = Stopwatch.StartNew();

        // 2) Check if host is up (basic “ping”)
        bool isHostUp = await IsHostReachable(request.Ip);

        // 3) Build your list of port statuses
        var portStatuses = new List<PortStatusInfo>();
        var tasks = new List<Task>();

        for (int port = request.StartPort; port <= request.EndPort; port++)
        {
            int currentPort = port;
            tasks.Add(Task.Run(async () =>
            {
                var portState = await CheckPortStatus(request.Ip, currentPort);
                lock (portStatuses)
                {
                    portStatuses.Add(new PortStatusInfo
                    {
                        Port = currentPort,
                        State = portState
                    });
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Stop the timer
        stopwatch.Stop();

        // 4) Build the final result
        var result = new DetailedPortScanResult
        {
            Ip = request.Ip,
            IsHostUp = isHostUp,
            Ports = portStatuses.OrderBy(p => p.Port).ToList(),
            ScanDuration = stopwatch.Elapsed,
            Timestamp = DateTime.UtcNow
        };

        // 5) Save to MongoDB (for future comparisons)
        await _scanHistoryCollection.InsertOneAsync(result);

        return result;
    }

    private async Task<PortState> CheckPortStatus(string ip, int port)
    {
        // We can interpret the outcome of a TCP Connect attempt
        // - If it connects quickly: Open
        // - If we get immediate “connection refused”: Closed
        // - If times out: Filtered
        // This is approximate; in real-world scanning,
        // full Nmap logic is more complex

        using var client = new TcpClient();
        try
        {
            var connectTask = client.ConnectAsync(ip, port);
            var completedTask = await Task.WhenAny(connectTask, Task.Delay(1000));
            
            if (completedTask == connectTask && client.Connected)
            {
                // We got a connection -> open
                return PortState.Open;
            }
            else
            {
                // Timed out -> likely filtered
                return PortState.Filtered;
            }
        }
        catch (SocketException ex)
        {
            // Some immediate errors are "connection refused" (closed)
            // Others might be timeouts or unreachable
            if (ex.SocketErrorCode == SocketError.ConnectionRefused)
            {
                return PortState.Closed;
            }
            else
            {
                // If we can't figure it out, call it filtered
                return PortState.Filtered;
            }
        }
        catch
        {
            return PortState.Filtered;
        }
    }

    private async Task<bool> IsHostReachable(string ip)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(ip, 1000);
            return (reply.Status == IPStatus.Success);
        }
        catch
        {
            // If it fails or throws, assume not reachable
            return false;
        }
    }
}