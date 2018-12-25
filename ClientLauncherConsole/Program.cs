using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var client = new Client.Client();
            client.Start();
            Console.ReadKey();
        }
    }
}
