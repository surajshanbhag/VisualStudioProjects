using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
namespace AdvancedRadar
{
    public partial class Form1 : Form
    {
        private  AdvancedRadar.Radar[] radar;
        private  SerialPort COMPort;
        private string[] list_Ports;
        private static int[] list_BaudRate= {300,600,1200,2400,9600,14400,19200,38400,57600,115200};
        private  Button button_LoadPorts;
        private  Button button_Run;
        private ComboBox combo_ListPort;
        private ComboBox combo_ListBaudRates;
        private TextBox textBox_Data;
        private GroupBox groupBox_Misc;
        private int divisions;
        Timer t;
        Random rnd;
        int count = 0;
        public Form1()
        {
            InitializeComponent();
            form_LoadControls();
            t = new Timer();
            t.Interval = 1;
            t.Tick += new EventHandler(this.t_Tick);
            rnd = new Random();

        }
        
        public void t_Tick(object sender,EventArgs e)
        {
            
            if(count == divisions)
            {
                count = 0;
            }
            for(int i = 0; i < 6; i++)
            {
                if (!radar[i].isBusy())
                {
                    radar[i].update_Global(this, count, rnd.Next(1, 100));
                }
            }
                        
            count++;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            t.Start();

        }
        private void form_LoadControls()
        {
            radar = new Radar[6];
            int xStart = 436;
            int yStart = 0;
            int size = 200;
            divisions = 500;
            radar[0] = new Radar(this, size, xStart, yStart, divisions, "Lidar");
            radar[1] = new Radar(this, size, xStart, yStart + size + 5, divisions, "Lidar");
            radar[2] = new Radar(this, size, xStart, yStart + size * 2 + 5, divisions, "Lidar");
            radar[3] = new Radar(this, size, xStart + size + 5, yStart, divisions, "Lidar");
            radar[4] = new Radar(this, size, xStart + size + 5, yStart + size + 5, divisions, "Lidar");
            radar[5] = new Radar(this, size, xStart + size + 5, yStart + size * 2 + 5, divisions, "Lidar");

            this.button_LoadPorts = new System.Windows.Forms.Button();
            this.button_Run = new System.Windows.Forms.Button();
            this.combo_ListPort = new System.Windows.Forms.ComboBox();
            this.combo_ListBaudRates = new System.Windows.Forms.ComboBox();
            this.textBox_Data = new System.Windows.Forms.TextBox();
            this.groupBox_Misc = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            this.groupBox_Misc.SuspendLayout();
            // 
            // button_LoadPorts
            // 
            this.button_LoadPorts.Location = new System.Drawing.Point(13, 13);
            this.button_LoadPorts.Name = "LoadPorts";
            this.button_LoadPorts.Size = new System.Drawing.Size(75, 23);
            this.button_LoadPorts.TabIndex = 0;
            this.button_LoadPorts.Text = "Load_Ports";
            this.button_LoadPorts.UseVisualStyleBackColor = true;
            this.button_LoadPorts.Click += new System.EventHandler(this.button_LoadPorts_Click);
            // 
            // button_Run
            // 
            this.button_Run.Location = new System.Drawing.Point(348, 15);
            this.button_Run.Name = "Run";
            this.button_Run.Size = new System.Drawing.Size(75, 23);
            this.button_Run.TabIndex = 1;
            this.button_Run.Text = "Run";
            this.button_Run.UseVisualStyleBackColor = true;
            // 
            // combo_ListPort
            // 
            this.combo_ListPort.FormattingEnabled = true;
            this.combo_ListPort.Location = new System.Drawing.Point(94, 15);
            this.combo_ListPort.Name = "combo_ListPort";
            this.combo_ListPort.Size = new System.Drawing.Size(121, 21);
            this.combo_ListPort.TabIndex = 2;
            // 
            // combo_ListBaudRates
            // 
            this.combo_ListBaudRates.FormattingEnabled = true;
            this.combo_ListBaudRates.Location = new System.Drawing.Point(221, 15);
            this.combo_ListBaudRates.Name = "combo_ListBaudRates";
            this.combo_ListBaudRates.Size = new System.Drawing.Size(121, 21);
            this.combo_ListBaudRates.TabIndex = 3;
            // 
            // textBox_Data
            // 
            this.textBox_Data.Location = new System.Drawing.Point(6, 46);
            this.textBox_Data.Multiline = true;
            this.textBox_Data.Name = "textBox_Data";
            this.textBox_Data.Size = new System.Drawing.Size(415, (size * 3) - 40);
            this.textBox_Data.TabIndex = 4;
            // 
            // groupBox_Misc
            // 
            this.groupBox_Misc.Controls.Add(this.button_LoadPorts);
            this.groupBox_Misc.Controls.Add(this.textBox_Data);
            this.groupBox_Misc.Controls.Add(this.combo_ListBaudRates);
            this.groupBox_Misc.Controls.Add(this.button_Run);
            this.groupBox_Misc.Controls.Add(this.combo_ListPort);
            this.groupBox_Misc.Location = new System.Drawing.Point(5, 0);
            this.groupBox_Misc.Name = "groupBox_Misc";
            this.groupBox_Misc.Size = new System.Drawing.Size(428, (size * 3) + 10);
            this.groupBox_Misc.TabIndex = 5;
            this.groupBox_Misc.TabStop = false;
            this.groupBox_Misc.Text = "Settings And Data";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 611);
            this.Controls.Add(this.groupBox_Misc);

            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox_Misc.ResumeLayout(false);
            this.groupBox_Misc.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button_LoadPorts_Click(object sender, EventArgs e)
        {
            list_Ports = SerialPort.GetPortNames();
            foreach (String portName in list_Ports)
            {
                combo_ListPort.Items.Add(portName);
            }
            foreach (int bauds in list_BaudRate)
            {
                combo_ListBaudRates.Items.Add(bauds);
            }
        }
        private void button_Run_Click(object sender, EventArgs e)
        {
            COMPort.BaudRate = Convert.ToInt32(combo_ListBaudRates.SelectedItem.ToString());
            COMPort.PortName = combo_ListPort.SelectedItem.ToString();
            COMPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }
        private void DataReceivedHandler(object sender,SerialDataReceivedEventArgs e)
        {
            string indata = COMPort.ReadExisting();
            textBox_Data.AppendText(indata+"\n");
        }
    }
}
