using System;
using System.Net;

namespace StaticPageServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2) Server.Start(IPAddress.Parse(args[0]), Convert.ToInt16(args[1]));
            Server.Start(IPAddress.Any, 8777);
        }
    }
}
