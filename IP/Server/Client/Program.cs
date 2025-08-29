using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

class Client
{
    static void Main()
    {
        UdpClient client = new UdpClient();
        string serverIP = "127.0.0.1";
        int port = 13000;

        Console.WriteLine("nhap ten mien:");

        while (true)
        {
            Console.Write("> ");
            string domain = Console.ReadLine();
            if (domain.ToLower() == "exit") break;

            byte[] data = Encoding.UTF8.GetBytes(domain);
            client.Send(data, data.Length, serverIP, port);

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] recvData = client.Receive(ref remoteEP);
            string result = Encoding.UTF8.GetString(recvData);

            Console.WriteLine("Ketqua IP:");
            Console.WriteLine(result);
        }

        client.Close();
    }
}