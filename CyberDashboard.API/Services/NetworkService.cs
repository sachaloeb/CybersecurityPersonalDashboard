using System.Net.NetworkInformation;

public static class NetworkService
{
    public static List<NetworkConnection> GetStatus()
    {
        var connections = new List<NetworkConnection>();
        var properties = IPGlobalProperties.GetIPGlobalProperties();
        var tcpConnections = properties.GetActiveTcpConnections();

        foreach (var connection in tcpConnections)
        {
            connections.Add(new NetworkConnection
            {
                Local = connection.LocalEndPoint.ToString(),
                LocalPort = connection.LocalEndPoint.Port,
                Remote = connection.RemoteEndPoint.ToString(),
                RemotePort = connection.RemoteEndPoint.Port,
                Protocol = "TCP",
                State = connection.State.ToString()
            });
        }

        return connections;
    }
}