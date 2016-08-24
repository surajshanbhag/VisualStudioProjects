using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;


namespace RadarClass
{
    class Radar
    {
        //constants
        private const int Circle_pen_Size = 04;
        private const int Hand_pen_Size = 02;
        private const int Point_pen_Size = 02;


        private int form_Size;      // size of form , picture box and image
        private int radar_Size;     // size of the radar
        private int picture_Size;
        private int hand_Length;

        private int picLocation_x, picLocation_y;
        private int radarLocation_x, radarLocation_y;   // Location of the Radar

        private Color color_background = Color.White;
        private Color color_line = Color.Black;
        private Color color_Point = Color.Red;

        private static int radar_Count = new int(); // number of objects
        private string radar_name;        // radar_name for the Radar

        private int hand_deg;       // degree of the hand

        private int[] data_global;
        private int[] data_covered;
        private Timer t;
        private PictureBox radar_PictureBox;
        private Bitmap radar_bmp;
        private Pen radar_pen;
        private Graphics radar_graphics;
        private Form Radar_Form;

        public Radar(string name = "Radar")
        {
            form_Size = 300;      // size of form , picture box and image
            picture_Size = form_Size - 10;
            picLocation_x = 0;
            picLocation_y = 0;
            radar_Size = picture_Size-5;     // size of the radar
            hand_Length = (radar_Size / 2)+1;
            radarLocation_x = radar_Size / 2;
            radarLocation_y = radar_Size / 2;

            t = new Timer();
            radar_Count++;

            radar_name = name + radar_Count.ToString();

            // Set the Forms
            Radar_Form = new Form();
            Radar_Form.SuspendLayout();
            //Radar_Form.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //Radar_Form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Radar_Form.ClientSize = new System.Drawing.Size(form_Size, form_Size);
            Radar_Form.AutoSize = false;
            Radar_Form.Name = radar_name;
            Radar_Form.Text = radar_name;
            Radar_Form.ResumeLayout(false);
            Radar_Form.BackColor = Color.Black;
            // 
            draw_Radar(Radar_Form, picLocation_x, picLocation_y);
            Radar_Form.ResumeLayout();
            radarDisplay_start();
            Radar_Form.Show();
            data_global = new int[360];
            data_covered = new int[360];
        }
        public Radar(Form form, int size, int x, int y, string name = "Radar")
        {
            picture_Size = size;
            picLocation_x = x;
            picLocation_y = y;
            radar_Size = picture_Size-3;     // size of the radar
            hand_Length = radar_Size / 2;
            radarLocation_x = (radar_Size / 2);
            radarLocation_y = (radar_Size / 2);

            t = new Timer();
            radar_Count++;

            radar_name = name + radar_Count.ToString();

            form.SuspendLayout();
            draw_Radar(form, picLocation_x, picLocation_y);
            form.ResumeLayout();
            radarDisplay_start();
            data_global = new int[360];
            data_covered = new int[360];
        }
        public void draw_Radar(Form form, int loc_x, int loc_y)
        {
            // create the picture element
            this.radar_PictureBox = new System.Windows.Forms.PictureBox();
            radar_PictureBox.Location = new System.Drawing.Point(loc_x, loc_y);
            radar_PictureBox.BorderStyle = BorderStyle.Fixed3D;
            radar_PictureBox.Name = radar_name;
            radar_PictureBox.ClientSize = new System.Drawing.Size(picture_Size, picture_Size);
            radar_PictureBox.TabIndex = 0;
            radar_PictureBox.TabStop = false;
            //radar_PictureBox.Dock = DockStyle.Fill;
            radar_PictureBox.BackColor = color_background;
            form.Controls.Add(radar_PictureBox);
            radar_bmp = new Bitmap(picture_Size, picture_Size);
            radar_PictureBox.Image = radar_bmp;
        }

        public void add_DataPoint(int deg, int val)
        {
            
            data_global[deg] = val * hand_Length / 100;
        }
        public int get_DataPoint(int deg)
        {
            return data_global[deg];
        }
        public void radarDisplay_start()
        {
            t.Interval = 5; //in millisecond
            t.Tick += new EventHandler(this.t_Tick);
            t.Start();
        }
        public void radarDisplay_stop()
        {
            t.Stop();
        }
        private void t_Tick(object sender, EventArgs e)
        {
            //draw circle

            Draw_Circle(radarLocation_x, radarLocation_y, radar_Size / 2);
            Draw_Circle(radarLocation_x, radarLocation_y, (int)(0.66 * radar_Size / 2));
            Draw_Circle(radarLocation_x, radarLocation_y, (int)(0.33 * radar_Size / 2));

            Draw_Line(hand_deg, hand_Length);
            Draw_Line(0, hand_Length);
            Draw_Line(90, hand_Length);
            Draw_Line(180, hand_Length);
            Draw_Line(270, hand_Length);

            for (int index = 0; index < 360; index++)
            {
                if (data_covered[index] != 0)
                {
                    Draw_point(index, data_covered[index]);
                }
            }
            data_covered[hand_deg] = data_global[hand_deg];
            //load bitmap in picturebox1
            radar_PictureBox.Image = radar_bmp;

            //update
            hand_deg++;
            if (hand_deg == 360)
            {
                hand_deg = 0;
            }
        }
        public void Draw_Circle(int center_x, int center_y, int radius)
        {
            radar_pen = new Pen(color_line, Circle_pen_Size);
            SolidBrush myBrush = new System.Drawing.SolidBrush(color_background);
            radar_graphics = Graphics.FromImage(radar_bmp);
            radar_graphics.DrawEllipse(radar_pen, center_x - radius, center_y - radius, radius * 2, radius * 2);
            radar_graphics.FillEllipse(myBrush, center_x - radius, center_y - radius, radius * 2, radius * 2);
            radar_graphics.Dispose();
            radar_pen.Dispose();
            myBrush.Dispose();
        }
        public void Draw_point(int angle, int distance)
        {
            int radius = 1;
            int center_x = 0;
            int center_y = 0;
            if (angle >= 0 && angle <= 180)
            {
                //right half
                //u in degree is converted into radian.

                center_x = radarLocation_x + (int)(distance * Math.Sin(Math.PI * angle / 180));
                center_y = radarLocation_y - (int)(distance * Math.Cos(Math.PI * angle / 180));
            }
            else
            {
                center_x = radarLocation_x - (int)(distance * -Math.Sin(Math.PI * angle / 180));
                center_y = radarLocation_y - (int)(distance * Math.Cos(Math.PI * angle / 180));
            }
            radar_pen = new Pen(color_Point, Point_pen_Size);
            SolidBrush myBrush = new System.Drawing.SolidBrush(Color.Green);
            radar_graphics = Graphics.FromImage(radar_bmp);
            radar_graphics.DrawEllipse(radar_pen, center_x - radius, center_y - radius, radius * 2, radius * 2);
            radar_graphics.FillEllipse(myBrush, center_x - radius, center_y - radius, radius * 2, radius * 2);
            //Draw_Line(angle, distance);
            radar_graphics.Dispose();
            radar_pen.Dispose();
            myBrush.Dispose();
        }
        public void Draw_Line(int angle, int length)
        {
            int x = 0;
            int y = 0;
            if (angle >= 0 && angle <= 180)
            {
                //right half
                //u in degree is converted into radian.

                x = radarLocation_x + (int)(length * Math.Sin(Math.PI * angle / 180));
                y = radarLocation_y - (int)(length * Math.Cos(Math.PI * angle / 180));
            }
            else
            {
                x = radarLocation_x - (int)(length * -Math.Sin(Math.PI * angle / 180));
                y = radarLocation_y - (int)(length * Math.Cos(Math.PI * angle / 180));
            }
            radar_pen = new Pen(color_line, Hand_pen_Size);
            radar_graphics = Graphics.FromImage(radar_bmp);
            radar_graphics.DrawLine(radar_pen, new Point(radarLocation_x, radarLocation_y), new Point(x, y));
            radar_graphics.Dispose();
            radar_pen.Dispose();
        }
    }
}
