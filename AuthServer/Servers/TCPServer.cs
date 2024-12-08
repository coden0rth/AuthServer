using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuthServer
{
    public class TCPServer
    {
        private TcpListener server;
        private bool isRunning;
        private Dictionary<string, TcpClient> activeConnections = new Dictionary<string, TcpClient>();

        public void Start(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;

            Console.WriteLine($"TCP server started on port {port}");

            Task.Run(() => ListenForClients());
        }

        private void ListenForClients()
        {
            while (isRunning)
            {
                TcpClient client = server.AcceptTcpClient();
                string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                Console.WriteLine($"New TCP connection from {clientIP}");

                Thread clientThread = new Thread(() => HandleClient(client, clientIP));
                clientThread.Start();
            }
        }

        private void HandleClient(TcpClient client, string clientIP)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine($"{clientIP} disconnected");
            }
        }

        public void Stop()
        {
            isRunning = false;
            server.Stop();
            Console.WriteLine("TCP server stopped");
        }
    }
}
