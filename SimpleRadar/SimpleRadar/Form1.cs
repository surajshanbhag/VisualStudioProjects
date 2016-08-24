using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RadarClass;

namespace SimpleRadar
{
    public partial class Form1 : Form
    {
        Timer t1;
        RadarClass.Radar[] radar;
        RadarClass.Radar radar2;
        Random rnd;
        int count = 0;
        public Form1()
        {
            InitializeComponent();

            //radar1 = new Radar("Radar");
            rnd = new Random();
            t1 = new Timer();
            t1.Interval = 1;
            t1.Tick += new EventHandler(this.t1_Tick);
            t1.Start();
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            /*radar = new Radar[6];
            for (int i = 0; i < 3; i++)
            {
                radar[i] = new Radar(this, 300, (300*(i%3))+5, 0);
            }
            for (int i = 3; i < 6; i++)
            {
                radar[i] = new Radar(this, 300, (300 * (i % 3)) + 5, 305);
            }*/
            radar2 = new Radar(this, 300, 300, 305);

            //RadarClass.Radar radar2 = new RadarClass.Radar("Radar2");
        }
        public void t1_Tick(object sender, EventArgs e)
        {
            /*for (int i = 0; i < 6; i++)
            {
                radar[i].add_DataPoint(rnd.Next(1, 360), rnd.Next(rnd.Next(20,30), rnd.Next(31,100)));
            }*/
            count+=1;
            if(count == 360)
            {
                count = 0;
            }
            radar2.add_DataPoint(count, rnd.Next(1,100));
        }
    }
}

