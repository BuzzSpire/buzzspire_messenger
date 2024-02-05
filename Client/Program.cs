using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Terminal.Gui;

class Client
{
    static void Main()
    {
        
        // TODO: get the server IP address and port from the user
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(new IPEndPoint(IPAddress.Parse(ip-adres>), <port>));

        string clientName;
        string clientMessage;

        Console.WriteLine("Enter your name: ");
        clientName = Console.ReadLine();

        // Sunucudan gelen mesajları dinleme iş parçacığı
        Thread receiveThread = new Thread(() =>
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                int received = client.Receive(buffer);
                if (received == 0)
                {
                    // Sunucu kapandığında veya bağlantı kesildiğinde
                    Console.WriteLine("Disconnected from the server.");
                    break;
                }

                string data = Encoding.ASCII.GetString(buffer, 0, received);
                Console.WriteLine(data);
            }
        });

        receiveThread.Start();

        Console.WriteLine("Enter your message (or 'exit' to quit):");
        while (true)
        {
            clientMessage = Console.ReadLine();

            if (clientMessage.ToLower() == "exit")
            {
                break;
            }

            client.Send(Encoding.ASCII.GetBytes(clientName + ": " + clientMessage + DateTime.Now.ToString(" (HH:mm:ss)") + "\n"));
        }

        client.Close();
    }
}