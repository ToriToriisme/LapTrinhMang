using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        UdpClient server = new UdpClient(13000);
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        Console.WriteLine("Server đang chay...");

        while (true)
        {
            byte[] data = server.Receive(ref remoteEP);
            string domain = Encoding.UTF8.GetString(data);
            Console.WriteLine($"nhan ten mien: {domain}");

            string response = "";

            try
            {
                IPHostEntry entry = Dns.GetHostEntry(domain);
                foreach (IPAddress ip in entry.AddressList)
                {
                    response += ip.ToString() + "\n";
                }
            }
            catch (Exception ex)
            {
                response = "loi phan giai: " + ex.Message;
            }

            byte[] reply = Encoding.UTF8.GetBytes(response);
            server.Send(reply, reply.Length, remoteEP);
        }
    }
}