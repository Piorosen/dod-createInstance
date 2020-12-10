using System;
using System.IO;
using System.Net.Sockets;
using Proto.Network;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.Net;
using System.Linq;

namespace RequestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CPU 성능 제한 : (50%)");
            Console.WriteLine("RAM 성능 : 2GB");
            Console.WriteLine("")
            TcpClient client = new TcpClient("127.0.0.1", 4000);
            var stream = client.GetStream();
            var dns = Dns.GetHostAddresses(Dns.GetHostName());
            var req = new RequestCreateInstance
            {
                Cpu = 0.5f,
                Ip = String.Join(" - ", dns.Select((e) => e.ToString())),
                Memory = 2,
                Os = Environment.OSVersion.ToString()
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
