using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace TCPserver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void BtnStartServer_Click(object sender, EventArgs e)
        {
            Thread tcpServerRunThread = new Thread(new ThreadStart(TcpServerRun));
            tcpServerRunThread.Start();
        }

        private void TcpServerRun()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 5004);
            listener.Start();
            UpdateUI("Listening");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                UpdateUI("Connected");

                using (var process = new Process())
                {                 
                    process.StartInfo.FileName = "C:\\Program Files\\"; // add full path
                    process.Start();
                    UpdateUI("Open Application");
                    process.WaitForExit();
                }

                Thread tcpHandlerThread = new Thread(new ParameterizedThreadStart(TcpHandler));
                tcpHandlerThread.Start(client);

            }
        }

        private void TcpHandler(object client)
        {
            TcpClient mClient = (TcpClient)client;
            NetworkStream stream = mClient.GetStream();
            byte[] message = new byte[1024];
            stream.Read(message, 0, message.Length);
            UpdateUI("New Message = " + Encoding.ASCII.GetString(message));

            stream.Close();
            mClient.Close();

        }

        private void UpdateUI(string s)
        {
            Func<int> del = delegate ()
            {
                textBox1.AppendText(s + System.Environment.NewLine);
                return 0;
            };
            Invoke(del);
        }
    }
}
