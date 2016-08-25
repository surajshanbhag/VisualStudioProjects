using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvancedRadar
{
    public partial class Form1 : Form
    {
        AdvancedRadar.Radar radar;
        Timer t;
        Random rnd;
        int count = 0;
        public Form1()
        {
            InitializeComponent();
            t = new Timer();
            t.Interval = 1;
            t.Tick += new EventHandler(this.t_Tick);
            rnd = new Random();

        }
        public void t_Tick(object sender,EventArgs e)
        {
            
            if(count == 360)
            {
                count = 0;
            }
            radar.update_Global(this,count, rnd.Next(1, 100));
            count++;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            radar = new Radar(this, 300, 0, 0, 360, "Radar");
            t.Start();
        }
    }
}
