using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    static List<Socket> clients = new List<Socket>();
    static Socket serverSocket;

    static void Main()
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // TODO: get the server port from the user
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, <port>));
        serverSocket.Listen(10);

        Console.WriteLine("Server is waiting for connections...");

        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            clients.Add(clientSocket);

            Console.WriteLine("A new client has joined.");

            Thread receiveThread = new Thread(() =>
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int received = clientSocket.Receive(buffer);
                    if (received == 0)
                    {
                        // İstemci ayrıldıysa
                        Console.WriteLine("A client has left.");
                        clients.Remove(clientSocket);
                        BroadcastMessage(clientSocket, "Left the room.");
                        break;
                    }

                    string data = Encoding.ASCII.GetString(buffer, 0, received);
                    Console.WriteLine(data);

                    BroadcastMessage(clientSocket, data);
                }
            });

            receiveThread.Start();
        }
    }

    static void BroadcastMessage(Socket sender, string message)
    {
        foreach (Socket client in clients)
        {
            if (client != sender)
            {
                client.Send(Encoding.ASCII.GetBytes(message));
            }
        }
    }
}
