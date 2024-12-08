using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer
{
    public class AuthServer
    {
        static void Main(string[] args)
        {

            TCPServer tcpServer = new TCPServer();

            tcpServer.Start(8080);

        }
    }
}
