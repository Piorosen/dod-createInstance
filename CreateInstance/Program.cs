using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using Proto.Network;

namespace CreateInstance
{
    class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database("own.db");

            Task.Run(() =>
            {
                TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 3999));
                listener.Start();
                var port = new Random();
                int clientId = 0;
                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    Console.WriteLine("Connection 요청 받음 ");
                    while (!client.GetStream().CanRead) ;

                    byte[] a = new byte[1024];
                    int size = client.GetStream().Read(a, 0, 1024);
                    var p = RequestCreateInstance.Parser.ParseFrom(a, 0, size);

                    int portInt = port.Next(4000, 5000);
                    var req = new RespondCreateInstance
                    {
                        Port = portInt.ToString(),
                        Password = "root",
                        Root = "root"
                    };

                    Console.WriteLine($"요청 : [{p.Os}], [{p.Ip}] 처리 중");

                    var send = req.ToByteArray();

                    client.GetStream().Write(send, 0, send.Length);
                    client.Close();
                    Console.WriteLine("Close : ");
                    clientId += 1;
                    string memory = $"--memory={p.Memory}g";
                    //string cpu = "--cpu-period=100000 --cpu-quota=50000";
                    string cpu = $"--cpus={p.Cpu}";
                    string portString = $"-p {portInt}:22";

                    Process.Start(new ProcessStartInfo(@"C:\Program Files\Docker\Docker\resources\bin\docker.exe", $"run -d {memory} {cpu} {portString} rastasheep/ubuntu-sshd:latest"));
                    db.Add(p.Ip, p.Os, portInt.ToString());
                    Console.WriteLine($"요청 처리 완료");
                }
                //listener.Stop();
            });

            while (true)
            {
                Console.WriteLine("DB 목록 확인 : 'A'");
                ConsoleKeyInfo a = new ConsoleKeyInfo();
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.A:
                        db.GetData().Select((e) => $"Time [{e.time}], Port [{e.port}], OS : [{e.os}]\r\nIp [{e.ip}]\n").ToList().ForEach(e => Console.WriteLine(e));
                        break;
                }
            }

            
        }
    }
}
//https://hub.docker.com/r/rastasheep/ubuntu-sshd/dockerfile