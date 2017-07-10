using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Rpi
{
    public partial class Form1 : Form
    {
        private const int DefaultServerPort = 6400;
        private const int DefaultClientPort = 6401;
        private const int Cooldown = 500;
#if WINDOWS
        public const string TemporaryPrefix = @"temp";
#else
        public const string TemporaryPrefix = @"/home/pi/Debug/temp";
#endif


        private Client m_client;
        private bool m_listen = true;
        private int unique = 0;
        private List<Order> m_orders = new List<Order>();
        private HashSet<int> m_pulled = new HashSet<int>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_client = new Client(DefaultClientPort);
            m_client.Start();

            Thread thread = new Thread(() =>
            {
                while (m_listen)
                {
                    if (!m_client.Connected)
                    {
                        Thread.Sleep(Cooldown);
                        continue;
                    }

                    var ids = (
                        from v
                        in m_client.SendString(MessageType.RequestIds).Split(' ', '$')
                        where v.Length > 0
                        select v
                        ).ToArray();

                    foreach (var v in ids)
                    {
                        int id = int.Parse(v);

                        if (!m_pulled.Contains(id))
                        {
                            m_pulled.Add(id);
                            m_client.SendString(MessageType.RequestData, false, v);
                            ProcessData(m_client.PullBytes());
                        }
                    }
                }
            });

            thread.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_listen = false;
            m_client.Stop();
        }

        private void autoUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (m_client.Connected)
            {
                connectionLabel.Text = "Соединено";
                connectionLabel.ForeColor = Color.Green;
            }
            else
            {
                connectionLabel.Text = "Нет соединения";
                connectionLabel.ForeColor = Color.Red;
            }

            currentLabel.Text = 
                "Текущий заказ:\n\r" + m_orders.FirstOrDefault()?.ToString();
            nextLabel.Text =
                "Следующий заказ:\n\r" + m_orders.Skip(1).FirstOrDefault()?.ToString();

            // logTextBox.Text = Logger.GetLogString();
            // logTextBox.Select(logTextBox.TextLength, 0);
            // logTextBox.ScrollToCaret();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '#' && m_orders.Count > 0)
            {
                var reply = m_client.SendString(MessageType.Finish, true, m_orders[0]);

                if (Message.GetMessageType(reply) == MessageType.Ack)
                {
                    m_pulled.Remove(m_orders[0].Id);
                    m_orders.RemoveAt(0);
                }
            }
        }

        private void ProcessData(byte[] tarGz)
        {
            string temporaryDir = TemporaryPrefix + unique++.ToString();
            string temporaryInfo = temporaryDir + @"/nfo.txt";
            string temporaryProg = temporaryDir + @"/PROG.LST";

            int id = -1;

            using (var stream = new MemoryStream(tarGz))
            {
                var archive = TarArchive.CreateInputTarArchive(stream);
                try
                {
                    archive.ExtractContents(temporaryDir);
                }
                catch (TarException e)
                {
                    Console.WriteLine(e.Message);
                    // inProcessIds.Remove(id);
                }

                using (StreamReader sr = new StreamReader(temporaryInfo))
                {
                    id = int.Parse(sr.ReadLine());

                    string notes = sr.ReadLine();
                    string customer = sr.ReadLine();
                    string material = sr.ReadLine();
                    GasType gas = (GasType)int.Parse(sr.ReadLine());
                    string path = sr.ReadLine();

                    int n = int.Parse(sr.ReadLine());

                    for (int i = 0; i < n; ++i)
                    {
                        string[] info = sr.ReadLine().Split('$');
                    }

                    byte[] prog = File.ReadAllBytes(temporaryProg);

                    m_orders.Add(new Order(
                        id,
                        material,
                        gas,
                        notes,
                        prog
                    ));

                    m_pulled.Add(id);
                    // InternalSetUpdated();
                }
                archive.Close();
                Console.WriteLine("Получен .TAR от сервера.");
            }
        }
    }
}
