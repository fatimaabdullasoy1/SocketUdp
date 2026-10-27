
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SERVR;

namespace SERVR
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var server = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp
            );

            var serverEp = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 4567
            );

            server.Bind(serverEp);

            var helper = new ImageHelper();
            byte[] buffer = new byte[65535];

            Console.WriteLine("Server is working.");
            Console.WriteLine("Waiting for SCREENSHOT...");

            while (true)
            {
                EndPoint remote = new IPEndPoint(
                    IPAddress.Any, 0
                );

                var result = await server.ReceiveFromAsync(
                    new ArraySegment<byte>(buffer),
                    SocketFlags.None,
                    remote
                );

                string message = Encoding.UTF8.GetString(
                    buffer, 0, result.ReceivedBytes
                );

                Console.WriteLine($"Received: {message}");

                if (message == "SCREENSHOT")
                {
                    string path = helper.TakeScreenShot();

                    byte[] imageBytes =
                        helper.GetBytesOfImage(path);

                    await server.SendToAsync(
                        imageBytes,
                        SocketFlags.None,
                        result.RemoteEndPoint
                    );

                    Console.WriteLine("Screenshot sent.");
                }
            }
        }
    }
}
