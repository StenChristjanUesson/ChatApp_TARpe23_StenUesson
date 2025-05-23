using System;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Sisesta oma kasutajanimi:");
            string username = Console.ReadLine();
            Console.WriteLine("Sisesta oma parool:");
            string password = Console.ReadLine();

            string token = await GetJwtTokenAsync(username, password);
            if (token == null)
            {
                Console.WriteLine("Programmi lõpetamine.");
                return;
            }

            var client = new Client("127.0.0.1", 5000, token);
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
        private static async Task<string> GetJwtTokenAsync(string username, string password)
        {
            using var httpClient = new HttpClient();
            var loginData = new { username, password };
            var response = await httpClient.PostAsJsonAsync("http://localhost:5000/api/auth/login", loginData);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return result["token"];
            }
            else
            {
                Console.WriteLine("Autentimine ebaõnnestus.");
                return null;
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
    private readonly string _token;

    public Client(string host, int port, string token)
    {
        _host = host;
        _port = port;
        _token = token;
    }

    public async Task ConnectAsync()
    {
        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync(_host, _port);
        _stream = _tcpClient.GetStream();

        await SendAsync($"TOKEN:{_token}");
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
