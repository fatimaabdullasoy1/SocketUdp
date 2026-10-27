using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp
            );

            var serverEp = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 4567
            );

            Console.WriteLine("Client is available.");
            Console.WriteLine("Type SCREENSHOT to get a screenshot.");

            while (true)
            {
                string message = Console.ReadLine() ?? "";

                await client.SendToAsync(
                    Encoding.UTF8.GetBytes(message),
                    SocketFlags.None,
                    serverEp
                );

                if (message == "SCREENSHOT")
                {
                    using var memoryStream = new MemoryStream();
                    byte[] buffer = new byte[60000];

                    EndPoint remote = new IPEndPoint(
                        IPAddress.Any, 0
                    );

                    var result = await client.ReceiveFromAsync(
                        new ArraySegment<byte>(buffer),
                        SocketFlags.None,
                        remote
                    );

                    await memoryStream.WriteAsync(
                        buffer, 0, result.ReceivedBytes
                    );

                    await File.WriteAllBytesAsync(
                        "screenshot.jpg",
                        memoryStream.ToArray()
                    );

                    Console.WriteLine("Screenshot received.");
                }
            }
        }
    }
}

