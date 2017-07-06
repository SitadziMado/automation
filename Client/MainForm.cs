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
    public partial class Form1 : Form
    {
        private const int DefaultServerPort = 6400;
        private const int DefaultClientPort = 6401;

        private Client client;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new Client(DefaultClientPort);
            client.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.Stop();
        }

        private void autoUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                connectionLabel.Text = "Соединено";
                connectionLabel.ForeColor = Color.Green;
            }
            else
            {
                connectionLabel.Text = "Нет соединения";
                connectionLabel.ForeColor = Color.Red;
            }

            logTextBox.Text = Logger.GetLogString();
            logTextBox.Select(logTextBox.TextLength, 0);
            logTextBox.ScrollToCaret();
        }

        private void reqButton_Click(object sender, EventArgs e)
        {
            client.SendString(MessageType.RequestIds, true, 0);
        }

        private void dataButton_Click(object sender, EventArgs e)
        {
            client.SendString(MessageType.RequestData, false, 0);
            var buffer = client.PullBytes();
            buffer = null;
        }
    }
}
