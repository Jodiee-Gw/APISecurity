using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebsocketClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            using (var ws = new ClientWebSocket())
            {
                try
                {
                    // Kết nối tới WebSocket server
                    await ws.ConnectAsync(new Uri("wss://ws.ifelse.io"), CancellationToken.None) ;
                    Console.WriteLine("✅ Connected!");


                    // Gửi tin nhắn
                    //var msg = "Hello from ClientWebSocket!";
                    while (true)
                    {
                        Console.WriteLine("Enter something : ");
                        var msg = Console.ReadLine();
                        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
                        await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                        Console.WriteLine("📤 Sent: " + msg);

                        // Nhận phản hồi
                        var receiveBuffer = new byte[1024];
                        var result = await ws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                        string received = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                        Console.WriteLine("📩 Received: " + received);

                        Console.WriteLine("Do you wanna continue. Please type (Y)");
                        msg= Console.ReadLine();
                        if (msg == "N")
                        {
                            break;
                        }
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Error: " + ex.Message);
                }
            }


            Console.WriteLine("✅ Done. Press any key to exit.");

            Console.ReadLine();
        }
    }
}

