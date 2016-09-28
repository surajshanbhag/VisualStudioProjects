using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
namespace SocketPlot
{
    public partial class Form1 : Form
    {
        public struct sensorData
        {
            public double[] data;
            public sensorData(int a)
            {
                data = new double[6];
            }
        };
        List<sensorData> sensor_2;
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        System.Threading.Thread socketThread;
        int sensor_1_index = 0;
        int sensor_1_last = 0;
        Boolean Connected = false;
        string ip;
        int portNum;
        Stopwatch sw;
        const int temperature_index = 0;
        const int pulse_index = 1;
        const int emg_index = 2;
        const int acceleration_x = 3;
        const int acceleration_y = 4;
        const int acceleration_z = 5;

        int NumPoints;
        Boolean threadRunning = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sensor_2 = new List<sensorData>();
            sw = new Stopwatch();
            NumPoints = 500;
            //sw.Start();
        }
        public void DoThisAllTheTime()
        {
            MethodInvoker mi = delegate () { this.Text = DateTime.Now.ToString(); };
            this.Invoke(mi);
            while (threadRunning)
            {
                //you need to use Invoke because the new thread can't access the UI elements directly

                while (Connected)
                {
                    try
                    {
                        NetworkStream networkStream = clientSocket.GetStream();
                        byte[] bytesFrom = new byte[200];
                        networkStream.Read(bytesFrom, 0, (int)bytesFrom.Length);
                        string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                        var result = dataFromClient.Split(new[] { ';' });
                        AppendTextBox(dataFromClient);
                        addDataFromSocket(result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Connect")
            {
                ip = ipBox1.Text + "." + ipBox2.Text + "." + ipBox3.Text + "." + ipBox4.Text;
                portNum = Convert.ToInt32(portBox.Text);
                if (ip != null)
                {
                    try
                    {
                        clientSocket.Connect(ip, portNum);
                        Connected = true;
                        threadRunning = true;
                        socketThread = new System.Threading.Thread(DoThisAllTheTime);
                        socketThread.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }
                }
                button1.Text = "Dis-connect";
            }
            else
            {
                threadRunning = false;
                Connected = false;
                clientSocket.GetStream().Close();
                clientSocket.Close();
                button1.Text = "Connect";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Play")
            {
                button2.Text = "Pause";
                timer1.Start();
            }
            else
            {
                button2.Text = "Play";
                timer1.Stop();
            }
        }

        private void chartRefreshTimer(object sender, EventArgs e)
        {
            List<double> sensor_DisplayPoints = new List<double>();
            chart4.Series["Temperature"].Points.Clear();
            chart3.Series["EMG"].Points.Clear();
            chart2.Series["Acceleration_x"].Points.Clear();
            chart2.Series["Acceleration_y"].Points.Clear();
            chart2.Series["Acceleration_z"].Points.Clear();
            chart1.Series["Pulse"].Points.Clear();
            for (int count = 0; count < 6; count++)
            {
                sensor_DisplayPoints.Clear();
                for (int i = 0; i < NumPoints; i++)
                {
                    if (i < sensor_2.Count)
                    {
                        sensor_DisplayPoints.Insert(0,sensor_2[sensor_2.Count - 1 - i].data[count]);
                    }
                    else
                    {
                        sensor_DisplayPoints.Insert(0, 0);
                    }

                }
                for (int i = 0; i < NumPoints; i++)
                {
                    switch (count)
                    {
                        case temperature_index:
                            chart4.Series["Temperature"].Points.AddXY(i, sensor_DisplayPoints[i]);
                            break;
                        case emg_index:
                            chart3.Series["EMG"].Points.AddXY(i, sensor_DisplayPoints[i]);
                            break;
                        case pulse_index:
                            chart1.Series["Pulse"].Points.AddXY(i, sensor_DisplayPoints[i]);
                            break;
                        case acceleration_x:
                            chart2.Series["Acceleration_x"].Points.AddXY(i, sensor_DisplayPoints[i]);
                            break;
                        case acceleration_y:
                            chart2.Series["Acceleration_y"].Points.AddXY(i, sensor_DisplayPoints[i]);
                            break;
                        case acceleration_z:
                            chart2.Series["Acceleration_z"].Points.AddXY(i, sensor_DisplayPoints[i]);
                            break;
                    }
                    
                }
            }
            sensor_1_last = sensor_1_index;
        }
        private void addDataFromSocket(string[] data)
        {
            sensor_1_index += 1;
            sensorData newData = new sensorData(0);
            newData.data[temperature_index] = Convert.ToDouble(data[temperature_index]);
            newData.data[pulse_index] = Convert.ToDouble(data[pulse_index]);
            newData.data[emg_index] = Convert.ToDouble(data[emg_index]);
            newData.data[acceleration_x] = Convert.ToDouble(data[acceleration_x]);
            newData.data[acceleration_y] = Convert.ToDouble(data[acceleration_y]);
            newData.data[acceleration_z] = Convert.ToDouble(data[acceleration_z]);
            sensor_2.Add(newData);
            sw.Stop();
            //long microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
            try
            {
                AppendStatusStrip(sw.ElapsedMilliseconds.ToString());
            }
            catch (Exception ex)
            {
                AppendTextBox(ex.ToString());
            }
            sw.Start();

        }
        private void addData(object sender, EventArgs e)
        {
            Random rand = new Random();
            sensor_1_index += 1;
            sensorData newData = new sensorData(0);
            newData.data[0] = rand.Next(0, 100);
            newData.data[1] = rand.Next(0, 100);
            newData.data[2] = rand.Next(0, 100);
            newData.data[3] = rand.Next(0, 100);
            sensor_2.Add(newData);

        }
        public void AppendStatusStrip(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendStatusStrip), new object[] { value });
                return;
            }
            toolStripStatusLabel1.Text = value;
            toolStripStatusLabel2.Text = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString();
            statusStrip1.Refresh();
        }
        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            textBox6.AppendText(value);
            textBox6.AppendText(Environment.NewLine);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NumPoints = 500 + 500;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (NumPoints > 50)
                NumPoints -= 50;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            threadRunning = false;
            Connected = false;
            clientSocket.GetStream().Close();
            clientSocket.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                chart2.Series["Acceleration_x"].Enabled = true;
            }
            else
            {
                chart2.Series["Acceleration_x"].Enabled = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                chart2.Series["Acceleration_y"].Enabled = true;
            }
            else
            {
                chart2.Series["Acceleration_y"].Enabled = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                chart2.Series["Acceleration_z"].Enabled = true;
            }
            else
            {
                chart2.Series["Acceleration_z"].Enabled = false;
            }
        }
    }
}
