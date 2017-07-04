using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class Server
    {
        public Server()
        {

        }

        public void Start()
        {
            m_listen = true;

            Thread thread = new Thread(() =>
            {
                IPAddress ip;
                TcpListener server = null;

                try
                {
                    ip = new IPAddress(127 | 0 | 0 | 1 << 24);
                    server = new TcpListener(ip, DefaultServerPort);
                    server.Start();

                    while (m_listen)
                    {
                        if (!server.Pending())
                        {
                            Thread.Sleep(PendingCooldown);
                            continue;
                        }

                        var client = server.AcceptTcpClient();
                        var address = ((IPEndPoint)client.Client.RemoteEndPoint).Address;

                        if (!m_addresses.Contains(address))
                        {
                            using (var sw = new StreamWriter(client.GetStream()))
                                sw.Write(Message.Ack);

                            m_addresses.Add(address);
                            m_clients.Add(client);
                        }
                    }
                }
                catch (SocketException e)
                {
                    throw e;
                }
                catch (IOException e)
                {
                    throw e;
                }
                finally
                {
                    // Останавливаем сервер и уничтожаем клиентов.
                    server?.Stop();
                    m_clients.Clear();
                    m_addresses.Clear();
                }
            });
        }

        public void Stop()
        {
            m_listen = false;
        }

        private const int DefaultServerPort = 6400;
        private const int PendingCooldown = 500;

        private List<TcpClient> m_clients = new List<TcpClient>();
        private HashSet<IPAddress> m_addresses = new HashSet<IPAddress>();
        private bool m_listen = false;
    }
}
