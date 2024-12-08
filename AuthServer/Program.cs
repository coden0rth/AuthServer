using System;

namespace AuthServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Init();
            AuthServer authServer = new AuthServer();

            string ipAddress = "127.0.0.1";
            int port = 8080;

            if (authServer.Start(ipAddress, port))
            {
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    authServer.Stop();
                };

                while (true) { }
            }
            else
            {
                Logger.C("Failed to start the server. Press any key to exit...", Logger.MessageType.Alert);
                Console.ReadLine();
            }
        }
    }
}
