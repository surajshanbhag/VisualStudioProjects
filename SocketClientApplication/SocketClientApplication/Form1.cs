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
        Boolean Connected = false;

        delegate void SetTextCallback(int index ,string value);
        string ip;
        int portNum;
        int requestCount = 0;
        int[] array;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            t = new System.Threading.Thread(DoThisAllTheTime);
            array = new int[1000];
            t.Start();
        }
        public void DoThisAllTheTime()
        {
            MethodInvoker mi = delegate () { this.Text = DateTime.Now.ToString(); };
            this.Invoke(mi);

            while (true)
            {
                //you need to use Invoke because the new thread can't access the UI elements directly

                while (Connected)
                {
                    try
                    {
                        if (requestCount < 1000)
                        {
                            requestCount = requestCount + 1;
                        }
                        else
                        {
                            requestCount = 0;
                        }
                        NetworkStream networkStream = clientSocket.GetStream();
                        byte[] bytesFrom = new byte[200];
                        networkStream.Read(bytesFrom, 0,(int)bytesFrom.Length);
                        string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                        var result = dataFromClient.Split(new[] { ';' });
                        //array[requestCount] = Convert.ToInt32(dataFromClient);
                        //this.chart1.Series["Series1"].Points.Add(array);
                        //this.chart1.Series["Series1"].Points.AddXY(requestCount, Convert.ToInt32(dataFromClient));
                        AddDataPoint(result);
                        AppendChart(requestCount, result[0]);
                        AppendTextBox(dataFromClient);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }
        public void AddDataPoint(string[] dataFromClient)
        {
            
        }
        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            logBox.AppendText(value);
            logBox.AppendText(Environment.NewLine);
        }
        public void AppendChart(int index,string value)
        {

            if (InvokeRequired)
            {
                //this.Invoke(new Action<int,string>(AppendChart), new object[] { value });
                SetTextCallback d = new SetTextCallback(AppendChart);
                this.Invoke(d, new object[] { index, value });
                return;
            }
            else
            {
                try
                {
                    chart1.Series["series1"].Points.AddXY(index, Convert.ToInt32(value));
                    chart1.Update();
                }
                catch (Exception ez)
                {
                    AppendTextBox(ez.ToString());
                }

            }

            
        }
        private void connectButton_Click(object sender, EventArgs e)
        {
            ip = ipBox1.Text + "." + ipBox2.Text + "." + ipBox3.Text + "." + ipBox4.Text;
            portNum = Convert.ToInt32(portBox.Text);
            if (ip != null)
            {
                try
                {
                    clientSocket.Connect(ip, portNum);
                    Connected = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                }
            } 
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
