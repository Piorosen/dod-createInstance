using System;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Proto.Network;

namespace CreateInstance
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 4000));
            listener.Start();
            var port = new Random();
            int clientId = 0;
            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("Connection : ");
                while (!client.GetStream().CanRead) ;

                byte[] a = new byte[1024];
                int size = client.GetStream().Read(a, 0, 1024);
                var p = RequestCreateInstance.Parser.ParseFrom(a, 0, size);

                
                var req = new RespondCreateInstance
                {
                    Id = clientId,
                    Ip = p.Ip,
                    Port = port.Next(4000, 5000).ToString(),
                    Password = "hello",
                    Root = p.Id,
                    Tag = p.Tag,
                };

                var send = req.ToByteArray();

                client.GetStream().Write(send, 0, send.Length);
                client.Close();
                Console.WriteLine("Close : ");
                clientId += 1;
            }
            //listener.Stop();
        }
    }
}
