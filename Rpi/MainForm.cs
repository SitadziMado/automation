using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rpi
{
    public partial class MainForm : Form
    {
        private const int DefaultServerPort = 6400;
        private const int DefaultClientPort = 6401;

        private Server server;

        /// <summary>
        /// Конструктор главной формы.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            Logger.CreateLogFile();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            server = new Server(DefaultServerPort, ClientProc);
            server.Start();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            server.Stop();
        }

        private void replyClientButton_Click(object sender, EventArgs e)
        {
            // server.SendString(0, MessageType.SendIds, new int[] { 1, 4, 9, 16, 25 });
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void autoUpdateTimer_Tick(object sender, EventArgs e)
        {
            logTextBox.Text = Logger.GetLogString();
            logTextBox.Select(logTextBox.TextLength, 0);
            logTextBox.ScrollToCaret();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            server.AddDevice(ipTextBox.Text, DefaultClientPort);
        }

        public byte[] ClientProc(int id, MessageType msg, object[] parameters)
        {
            switch (msg)
            {
                case MessageType.RequestIds:
                    return Encoding.UTF8.GetBytes("ids 1 4 9 16 25 36 49 64 81 100\n");

                case MessageType.RequestData:
                    return Encoding.UTF8.GetBytes("data\nGOLF WANG");

                default:
                    break;
            }

            return Encoding.ASCII.GetBytes("ack\n");
        }
    }
}
