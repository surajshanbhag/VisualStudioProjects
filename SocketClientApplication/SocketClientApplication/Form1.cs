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
            this.SuspendLayout();
            System.Windows.Forms.DataVisualization.Charting.ChartArea[] chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea[4];
            System.Windows.Forms.DataVisualization.Charting.Legend[] legend = new System.Windows.Forms.DataVisualization.Charting.Legend[4];
            System.Windows.Forms.DataVisualization.Charting.Series[] series = new System.Windows.Forms.DataVisualization.Charting.Series[4];
            System.Windows.Forms.DataVisualization.Charting.Title[] title = new System.Windows.Forms.DataVisualization.Charting.Title[4];
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart[4];
            string[] chartNames = { "Acceleration", "Pulse", "Temperature", "EMG" };
            for (int i = 0; i < 4; i++)
            {
                chartArea[i] = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
                legend[i] = new System.Windows.Forms.DataVisualization.Charting.Legend();
                series[i] = new System.Windows.Forms.DataVisualization.Charting.Series();
                title[i] = new System.Windows.Forms.DataVisualization.Charting.Title();
                chart[i] = new System.Windows.Forms.DataVisualization.Charting.Chart();
                ((System.ComponentModel.ISupportInitialize)(this.chart[i])).BeginInit();
            }
            // 
            // chart_temp
            // 
            for (int i = 0; i < 4; i++)
            {
                chartArea[i].Name = "chartArea_" + i.ToString();
                this.chart[i].ChartAreas.Add(chartArea[i]);
                legend[i].Enabled = false;
                legend[i].Name = "legend_" + i.ToString();
                this.chart[i].Legends.Add(legend[i]);
                this.chart[i].Location = new System.Drawing.Point(397, 15 + i * 198);
                this.chart[i].Margin = new System.Windows.Forms.Padding(4);
                this.chart[i].Name = "chart_" + i.ToString();
                series[i].ChartArea = "chartArea_" + i.ToString();
                series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                series[i].Legend = "legend_" + i.ToString();
                series[i].Name = "series_" + i.ToString();
                this.chart[i].Series.Add(series[i]);
                this.chart[i].Size = new System.Drawing.Size(1137, 189);
                this.chart[i].TabIndex = 4;
                this.chart[i].Text = "chart_" + i.ToString();
                title[i].Name = chartNames[i];
                this.chart[i].Titles.Add(title[i]);
                this.chart[i].ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
            }
            for (int i = 0; i < 4; i++)
            {
                this.Controls.Add(this.chart[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                ((System.ComponentModel.ISupportInitialize)(this.chart[i])).EndInit();
            }
            this.ResumeLayout(false);
            this.PerformLayout();
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
                        AppendChart(requestCount, result[1]);
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
                    chart[0].Series["series_0"].Points.AddXY(index, Convert.ToDouble(value));
                    chart[0].Update();
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
