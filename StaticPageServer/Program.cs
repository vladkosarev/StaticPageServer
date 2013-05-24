using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace StaticPageServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int port = 80;
                TcpListener server = new TcpListener(IPAddress.Any, port);
                server.Start();

                byte[] bytes = new byte[1024];

                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    NetworkStream stream = client.GetStream();

                    int i = stream.Read(bytes, 0, bytes.Length);

                    var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    var req = data.Split(' ');
                    if (req.Length > 3)
                    {
                        var path = req[1];
                        var filePath = "." + path.Replace('/', '\\');
                        Console.WriteLine(filePath);
                        if (File.Exists(filePath))
                        {
                            var content = File.ReadAllText(filePath);
                            stream.Write(System.Text.Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\n\r\n" + content), 0, content.Length + 19);
                        }
                        else if (Directory.Exists(filePath))
                        {
                            if (File.Exists("index.html"))
                            {
                                var content = File.ReadAllText(filePath + "\\index.html");
                                stream.Write(System.Text.Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\n\r\n" + content), 0, content.Length + 19);
                            }
                            else
                            {
                                stream.Write(System.Text.Encoding.ASCII.GetBytes("HTTP/1.1 404 Not Found"), 0, 22);
                            }
                        }
                        else
                        {
                            stream.Write(System.Text.Encoding.ASCII.GetBytes("HTTP/1.1 404 Not Found"), 0, 22);
                        }
                    }
                    stream.Close();
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
