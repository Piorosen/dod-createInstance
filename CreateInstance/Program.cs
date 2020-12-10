using System;
using System.Diagnostics;
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

                int portInt = port.Next(4000, 5000);
                var req = new RespondCreateInstance
                {
                    Port = portInt.ToString(),
                    Password = "root",
                    Root = "root"
                };

                Console.WriteLine($"요청 : [{p.Os}], [{p.Ip}]");

                var send = req.ToByteArray();

                client.GetStream().Write(send, 0, send.Length);
                client.Close();
                Console.WriteLine("Close : ");
                clientId += 1;
                string memory = $"--memory={p.Memory}g";
                //string cpu = "--cpu-period=100000 --cpu-quota=50000";
                string cpu = $"--cpus={p.Cpu}";
                string portString = $"-p {portInt}:22";
                

                Process.Start(new ProcessStartInfo("/usr/local/bin/docker", $"run -d {memory} {cpu} {portString} rastasheep/ubuntu-sshd:latest"));
            }
            //listener.Stop();
        }
    }
}
//https://hub.docker.com/r/rastasheep/ubuntu-sshd/dockerfile