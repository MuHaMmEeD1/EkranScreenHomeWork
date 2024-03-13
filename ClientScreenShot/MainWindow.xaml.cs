using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System;

namespace ClientScreenShot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        EndPoint remoutEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27001);
        bool Started = true;
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;


        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (Started)
            {
                IsleMetod();
                Started = false;
            }

        }

        async Task IsleMetod()
        {
            await Task.Run(() => {

                byte[] buffer = new byte[ushort.MaxValue - 30];

                client.SendTo(buffer, remoutEndPoint);

                while (true)
                {
                    buffer = new byte[ushort.MaxValue - 30];
                    List<byte> EsasBufferList = new List<byte>();

                    while (true)
                    {
                        int bytesReceived = client.ReceiveFrom(buffer, ref remoutEndPoint);
                        EsasBufferList.AddRange(buffer.Take(bytesReceived));

                        if (bytesReceived < buffer.Length)
                        {
                            break;
                        }
                    }

                    byte[] EsasBuffer = EsasBufferList.ToArray();



                    Dispatcher.Invoke(() => {
                        BitmapImage bitmap = new BitmapImage();

                        using (MemoryStream ms = new MemoryStream(EsasBuffer))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = ms;
                            bitmap.EndInit();
                        }

                        EsasImage.Source = bitmap;
                    });
                }

            });
        }


    }
}




























