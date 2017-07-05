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

        private Server server = new Server(6400);
        private Client client = new Client(6401);

        /// <summary>
        /// Конструктор главной формы.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            server.Start();
            client.AsyncWaitForConnection();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            server.Stop();
        }

        private void replyClientButton_Click(object sender, EventArgs e)
        {
            server.AddDevice("127.0.0.1", DefaultClientPort);
        }

        private void connectToServerButton_Click(object sender, EventArgs e)
        {
            client.SendString(Message.RequestIds);
        }

    }
}
