using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace StaticPageServer
{
    public class Server
    {
        private static class HttpStrings
        {
            public static string OK
            {
                get { return "HTTP/1.1 200 OK\r\n\r\n"; }
            }

            public static string NotFound
            {
                get { return "HTTP/1.1 404 Not Found\r\n\r\n"; }
            }
        }

        public static void Start(IPAddress address, int port)
        {
            try
            {
                var server = new TcpListener(address, port);
                Console.WriteLine("Post Apocalyptic Web Server started on {0}:{1}", address, port);
                server.Start();

                var bytes = new byte[1024];

                while (true)
                {
                    // Client Connected
                    var client = server.AcceptTcpClient();
                    var stream = client.GetStream();
                    try
                    {
                        var i = stream.Read(bytes, 0, bytes.Length);
                        var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        // Pretend to know what http is like
                        var req = data.Split(' '); // parsing http <- ha!
                        string served = null;
                        if (req.Length > 3)
                        {
                            var path = req[1];
                            var filePath = "." + path.Replace('/', '\\');
                            if (File.Exists(filePath))
                            {
                                served = filePath;
                                var content = File.ReadAllText(served);
                                stream.Write(System.Text.Encoding.ASCII.GetBytes(HttpStrings.OK + content), 0,
                                             content.Length + HttpStrings.OK.Length);
                            }
                            else if (Directory.Exists(filePath))
                            {
                                if (File.Exists("index.html"))
                                {
                                    served = filePath + "index.html";
                                    var content = File.ReadAllText(served);
                                    stream.Write(System.Text.Encoding.ASCII.GetBytes(HttpStrings.OK + content), 0,
                                                 content.Length + HttpStrings.OK.Length);
                                }
                                else
                                {
                                    stream.Write(System.Text.Encoding.ASCII.GetBytes(HttpStrings.NotFound), 0,
                                                 HttpStrings.NotFound.Length);
                                }
                            }
                            else
                            {
                                stream.Write(System.Text.Encoding.ASCII.GetBytes(HttpStrings.NotFound), 0,
                                             HttpStrings.NotFound.Length);
                            }

                            if (served != null)
                            {
                                Console.WriteLine("[{0}] Served {1} to {2}", client.Client.RemoteEndPoint, served, path);
                            }
                            else
                            {
                                Console.WriteLine("[{0}] Couldn't find file to serve {1}", client.Client.RemoteEndPoint, path);
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[{0}] Bad things happened: {1}", client.Client.RemoteEndPoint, e.Message);
                    }
                    finally
                    {
                        stream.Close();
                        client.Close();
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Server failed to start - SocketException: {0}", e);
            }
        }
    }
}