using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

public class SocketClient : BackgroundService
{
    private Socket? _socket;

    protected override async Task ExecuteAsync( CancellationToken stoppingToken )
    {
        _socket = await ConnectToSocket( stoppingToken );

        while ( !stoppingToken.IsCancellationRequested )
        {
            var message = Encoding.ASCII.GetBytes("Test Message");
            var bytesSent = _socket.Send(message);
            
            Console.WriteLine($"Sent {bytesSent} bytes to server");
            
            await Task.Delay(1000, stoppingToken);
        }

        _socket.Shutdown( SocketShutdown.Both );
        _socket.Close();
    }

    private static async Task<Socket> ConnectToSocket( CancellationToken stoppingToken = default )
    {
        var host = await Dns.GetHostEntryAsync( "server", stoppingToken );
        var ipAddress = host.AddressList[0];
        var localEndPoint = new IPEndPoint( ipAddress, 28117 );

        var sender = new Socket( ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
        await sender.ConnectAsync(localEndPoint, stoppingToken);

        return sender;
    }
}
