using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

public class SocketServer : BackgroundService
{
    private Socket? _socket;

    protected override async Task ExecuteAsync( CancellationToken stoppingToken )
    {
        _socket = await ConnectToSocket( stoppingToken );

        while ( !stoppingToken.IsCancellationRequested )
        {
            var bytes = new byte[1024];
            var bytesReceived = await _socket.ReceiveAsync(
                bytes,
                SocketFlags.None,
                stoppingToken
            );

            if ( bytesReceived == 0 )
            {
                throw new InvalidOperationException( "Connection to Web Server Disconnected" );
            }

            Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesReceived));
        }

        _socket.Shutdown( SocketShutdown.Both );
        _socket.Close();
    }

    private static async Task<Socket> ConnectToSocket( CancellationToken stoppingToken = default )
    {
        var host = await Dns.GetHostEntryAsync( "server", stoppingToken );
        var ipAddress = host.AddressList[0];
        var localEndPoint = new IPEndPoint( ipAddress, 28117 );

        var listener = new Socket( ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
        listener.Bind( localEndPoint );
        listener.Listen( 10 );

        Console.WriteLine( "Waiting for a connection..." );

        return await listener.AcceptAsync( stoppingToken );
    }
}
