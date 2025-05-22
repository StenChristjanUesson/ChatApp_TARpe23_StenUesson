using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Sisesta oma nimi:");
            string username = Console.ReadLine();

            var client = new Client("127.0.0.1", 5000);
            await client.ConnectAsync();
            await client.SendAsync(username);

            Console.WriteLine("Ühendatud serveriga. Sisesta sõnumid:");

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    string serverMessage = await client.ReceiveAsync();
                    Console.WriteLine(serverMessage);
                }
            });

            while (true)
            {
                string message = Console.ReadLine();
                await client.SendAsync(message);
            }
        }
    }
}

public class Client
{
    private TcpClient _tcpClient;
    private NetworkStream _stream;
    private readonly string _host;
    private readonly int _port;

    public Client(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public async Task ConnectAsync()
    {
        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync(_host, _port);
        _stream = _tcpClient.GetStream();
    }

    public async Task SendAsync(string message)
    {
        if (_stream == null) return;
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message + "\n");
        await _stream.WriteAsync(buffer, 0, buffer.Length);
    }

    public async Task<string> ReceiveAsync()
    {
        byte[] buffer = new byte[1024];
        int byteCount = await _stream.ReadAsync(buffer, 0, buffer.Length);
        return System.Text.Encoding.UTF8.GetString(buffer, 0, byteCount);
    }
}