using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace SocketClientApplication
{
    public partial class Form1 : Form
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        System.Threading.Thread t;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            msg("Client Started");
            t = new System.Threading.Thread(DoThisAllTheTime);
            t.Start();
        }
        public void DoThisAllTheTime()
        {
            while (true)
            {
                //you need to use Invoke because the new thread can't access the UI elements directly
                MethodInvoker mi = delegate () { this.Text = DateTime.Now.ToString(); };
                this.Invoke(mi);
                int requestCount = 0;
                clientSocket.Connect("192.168.7.2", 5000);
                //label1.Text = "Client Socket Program - Server Connected ...";
                while ((true))
                {
                    try
                    {
                        requestCount = requestCount + 1;
                        NetworkStream networkStream = clientSocket.GetStream();
                        byte[] bytesFrom = new byte[200];
                        networkStream.Read(bytesFrom, 0,(int)bytesFrom.Length);
                        string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                        AppendTextBox(dataFromClient);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }
        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            textBox1.Text += value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NetworkStream serverStream = clientSocket.GetStream();
            //byte[] outStream = System.Text.Encoding.ASCII.GetBytes(textBox2.Text + "$");
            //serverStream.Write(outStream, 0, outStream.Length);
            //serverStream.Flush();

            byte[] inStream = new byte[200];
            serverStream.Read(inStream, 0, (int)inStream.Length);
            string returndata = System.Text.Encoding.ASCII.GetString(inStream);
            msg(returndata);
        }

        public void msg(string mesg)
        {
            textBox1.AppendText(mesg);
            textBox1.AppendText(Environment.NewLine);
        }

    }
}
