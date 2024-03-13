using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 27001);

            server.Bind(endPoint);

            byte[] buffer = new byte[ushort.MaxValue - 30];

            while (true)
            {
                EndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    server.ReceiveFrom(buffer, ref senderEndPoint);
                }
                catch(Exception ex) { continue; }
                Task.Run(() =>
                {
                    EndPoint sEP = senderEndPoint;
                    while (true)
                    {
                        try
                        {
                            Bitmap screenshot = CaptureScreen();
                            byte[] imageBytes = ImageToBytes(screenshot);

                            var bayts = imageBytes.Chunk(ushort.MaxValue - 30);


                            foreach (var by in bayts)
                            {
                                server.SendTo(by, sEP);
                            }

                            Thread.Sleep(10);
                        }
                        catch(Exception ex) { Console.WriteLine(ex.Message); break; }
                    }
                });
            }
        }

        static Bitmap CaptureScreen()
        {
            Rectangle bounds = new Rectangle(0, 0, 1920, 1080);
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(screenshot))
            {
                graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            }

            return screenshot;
        }

        static byte[] ImageToBytes(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}
