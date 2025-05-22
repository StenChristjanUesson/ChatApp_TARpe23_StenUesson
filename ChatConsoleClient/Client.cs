using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatConsoleClient
{
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
            byte[] buffer = Encoding.UTF8.GetBytes(message + "\n");
            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task<string> ReceiveAsync()
        {
            byte[] buffer = new byte[1024];
            int byteCount = await _stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, byteCount);
        }

        public void Close()
        {
            _stream?.Close();
            _tcpClient?.Close();
        }
    }
}