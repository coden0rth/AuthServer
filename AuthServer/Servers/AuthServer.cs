using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AuthServer
{
    public class AuthServer
    {
        private TcpListener server;
        private bool isRunning;

        public bool Start(string ipAddress, int port)
        {
            Logger.C($"Starting Authentiation TCP Server on {ipAddress}:{port}", Logger.MessageType.Warn);
            try
            {
                IPAddress address = IPAddress.Parse(ipAddress);
                server = new TcpListener(address, port);
                server.Start();
                isRunning = true;
                Logger.C($"Server started on { ipAddress}:{ port}", Logger.MessageType.Regular);
                return true;
            }
            catch (SocketException ex)
            {
                Logger.C($"Error starting server on {ipAddress}:{port} - {ex.Message}", Logger.MessageType.Warn);
                return false;
            }
            catch (FormatException)
            {
                Logger.C($"Invalid IP address format: {ipAddress}", Logger.MessageType.Warn);
                return false;
            }
        }
        public void InitListenThread()
        {
            Thread listenThread = new Thread(ListenForClients);
            listenThread.Start();
        }

        private void ListenForClients()
        {
            Logger.C($"Awaiting client connection...", Logger.MessageType.Regular);

            while (isRunning)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    Logger.C($"Incoming authentication request from {client.Client.AddressFamily}", Logger.MessageType.Regular);
                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(client);
                }
                catch (SocketException)
                {
                    if (!isRunning) break;
                }
            }
        }

        private void HandleClient(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received message: {message}");

                string response = "Message received";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client disconnected");
            }
        }

        public void Stop()
        {
            isRunning = false;
            server.Stop();
            Logger.C("AuthServer Halted! Exiting...", Logger.MessageType.Alert);
            Thread.Sleep(2000);
            Environment.Exit(0);
        }
    }
}
