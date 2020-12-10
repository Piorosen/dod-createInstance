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
            Console.WriteLine("요청 위치 : 210.108");
            TcpClient client = new TcpClient("210.107.245.192", 3999);
            var stream = client.GetStream();
            var dns = Dns.GetHostAddresses(Dns.GetHostName());
            var req = new RequestCreateInstance
            {
                Cpu = 0.5f,
                Ip = string.Join(" - ", dns.Select((e) => e.ToString())),
                Memory = 2,
                Os = Environment.OSVersion.ToString()
            };

            var send = req.ToByteArray();
            stream.Write(send, 0, send.Length);
            while (!client.GetStream().CanRead);

            byte[] a = new byte[1024];
            int size = client.GetStream().Read(a, 0, 1024);

            var p = RespondCreateInstance.Parser.ParseFrom(a, 0, size);

            Console.WriteLine($"계정 : {p.Root}, 비밀번호 : {p.Password}, 접근 포트 : {p.Port}\n 접속 권한이 생겼습니다.");
            StreamWriter sw = new StreamWriter("result.txt");
            sw.Write($"계정 : {p.Root}, 비밀번호 : {p.Password}, 접근 포트 : {p.Port}\n 접속 권한이 생겼습니다.");
            sw.Close();
        }
    }
}
