using System;
using System.IO;
using System.Net.Sockets;
using Proto.Network;
using Google.Protobuf;
using Google.Protobuf.Collections;
namespace RequestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CPU 성능 제한 : (50%)");
            Console.WriteLine("RAM 성능 : 2GB");
            Console.WriteLine("아이디 : root");

            TcpClient client = new TcpClient("127.0.0.1", 4000);
            var stream = client.GetStream();

            var req = new RequestCreateInstance
            {
                Id = "root",
                Ip = "127.0.0.1",
                Tag = "null"
            };

            var send = req.ToByteArray();
            stream.Write(send, 0, send.Length);
            while (!client.GetStream().CanRead);

            byte[] a = new byte[1024];
            int size = client.GetStream().Read(a, 0, 1024);

            var p = RespondCreateInstance.Parser.ParseFrom(a, 0, size);

            Console.WriteLine($"{p.Id} : {p.Ip} : {p.Password} : {p.Port} : {p.Root} : {p.Tag}");
            
        }
    }
}
