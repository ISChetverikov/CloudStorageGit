
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Server;

namespace ServerService
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SyncServer();

            server.Start();
            Console.ReadKey();
        }

        
    }
}
