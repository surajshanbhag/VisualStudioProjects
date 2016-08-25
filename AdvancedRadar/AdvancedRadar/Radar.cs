using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;

namespace AdvancedRadar
{
    class Radar
    {
        // Colors
        private Color color_Background = Color.Black;
        private Color color_Line = Color.GreenYellow;
        private Color color_Point = Color.Red;

        // pen sizes
        private const int penSz_Circle = 1;
        private const int penSz_SweepLine = 1;
        private const int penSz_Point = 1;

        // dimensions
        private int size_Radar;
        private int size_PictureBox;
        private int length_SweepLine;

        // locations
        private int xloc_Radar;
        private int yloc_Radar;
        private int xloc_PictureBox;
        private int yloc_PictureBox;

        //Misc

        private static int number_Radar = new int();
        private string name_Radar;

        private int angle_SweepLine;
        private int index_Data;

        private int divisions_Angle;    // total number of divisions of 360 degrees

        private int[] data_Global;
        private int[] data_Covered;

        private PictureBox pictureBox_Radar;
        private Bitmap bmp_Radar;
        private Bitmap bmp_BaseRadar;
        private Graphics graphics_Radar;
        /*-------------------------------------------------------------*/
        public Radar(Form form, int size, int x, int y, int Divisions, string name = "Radar")
        {
            size_PictureBox = size;
            xloc_PictureBox = x;
            yloc_PictureBox = y;

            size_Radar = size_PictureBox - 3;     // size of the radar
            length_SweepLine = size_Radar / 2;
            xloc_Radar = (size_Radar / 2);
            yloc_Radar = (size_Radar / 2);

            number_Radar++;
            name_Radar = name + number_Radar.ToString();

            divisions_Angle = Divisions;
            data_Global = new int[divisions_Angle];
            data_Covered = new int[divisions_Angle];

            form.SuspendLayout();
            // create the picture element
            this.pictureBox_Radar = new System.Windows.Forms.PictureBox();
            pictureBox_Radar.Location = new System.Drawing.Point(xloc_PictureBox, xloc_PictureBox);
            pictureBox_Radar.BorderStyle = BorderStyle.Fixed3D;
            pictureBox_Radar.Name = name_Radar;
            pictureBox_Radar.ClientSize = new System.Drawing.Size(size_PictureBox, size_PictureBox);
            pictureBox_Radar.TabIndex = 0;
            pictureBox_Radar.TabStop = false;
            pictureBox_Radar.BackColor = color_Background;

            form.Controls.Add(pictureBox_Radar);


            bmp_Radar = new Bitmap(size_PictureBox, size_PictureBox);
            bmp_BaseRadar = new Bitmap(size_PictureBox, size_PictureBox);
            pictureBox_Radar.Image = bmp_Radar;

            draw_Radar();
            form.ResumeLayout();

        }
        public void update_Global(int angle, int value)
        {
            data_Global[angle] = value * length_SweepLine / 100;

            Bitmap bmp = new Bitmap(bmp_BaseRadar);

            angle_SweepLine = (int)(360 * angle / this.divisions_Angle);

            Draw_Line(angle_SweepLine, length_SweepLine, Color.Aqua, bmp);

            for (int index = 0; index < divisions_Angle; index++)
            {
                if (data_Covered[index] != 0 && index != angle_SweepLine)
                {
                    Draw_point(index, data_Covered[index], color_Point, bmp, false);
                }
            }
            Draw_point(angle_SweepLine, data_Global[angle_SweepLine], color_Point, bmp, false);
            data_Covered[angle_SweepLine] = data_Global[angle_SweepLine];
            //load bitmap in picturebox1
            pictureBox_Radar.Image = bmp;


        }
        public void draw_Radar()
        {
            Draw_Circle(size_Radar / 2, true, bmp_BaseRadar);
            Draw_Circle((int)(0.75 * size_Radar / 2), true, bmp_BaseRadar);
            Draw_Circle((int)(0.50 * size_Radar / 2), true, bmp_BaseRadar);
            Draw_Circle((int)(0.25 * size_Radar / 2), true, bmp_BaseRadar);
            Draw_Line(0, length_SweepLine, color_Line, bmp_BaseRadar);
            Draw_Line(90, length_SweepLine, color_Line, bmp_BaseRadar);
            Draw_Line(180, length_SweepLine, color_Line, bmp_BaseRadar);
            Draw_Line(270, length_SweepLine, color_Line, bmp_BaseRadar);
        }
        public void Draw_Circle(int radius, bool fill, Bitmap bmp)
        {
            Pen myPen = new Pen(color_Line, penSz_Circle);
            Graphics myGraphics = Graphics.FromImage(bmp);
            if (fill)
            {
                SolidBrush myBrush = new System.Drawing.SolidBrush(color_Background);
                myGraphics.FillEllipse(myBrush, xloc_Radar - radius, yloc_Radar - radius, radius * 2, radius * 2);
                myBrush.Dispose();
            }

            myGraphics.DrawEllipse(myPen, xloc_Radar - radius, yloc_Radar - radius, radius * 2, radius * 2);
            myGraphics.Dispose();
            myPen.Dispose();
        }
        public void Draw_point(int angle, int distance, Color colorPoint, Bitmap bmp, bool line = true)
        {
            int radius = 1;
            int center_x = 0;
            int center_y = 0;

            if (angle >= 0 && angle <= 180)
            {
                //right half
                //u in degree is converted into radian.

                center_x = xloc_Radar + (int)(distance * Math.Sin(Math.PI * angle / 180));
                center_y = yloc_Radar - (int)(distance * Math.Cos(Math.PI * angle / 180));
            }
            else
            {
                center_x = xloc_Radar - (int)(distance * -Math.Sin(Math.PI * angle / 180));
                center_y = yloc_Radar - (int)(distance * Math.Cos(Math.PI * angle / 180));
            }
            Pen myPen = new Pen(colorPoint, penSz_Point);
            SolidBrush myBrush = new System.Drawing.SolidBrush(colorPoint);
            Graphics myGraphics = Graphics.FromImage(bmp);
            myGraphics.DrawEllipse(myPen, center_x - radius, center_y - radius, radius * 2, radius * 2);
            myGraphics.FillEllipse(myBrush, center_x - radius, center_y - radius, radius * 2, radius * 2);
            if (line)
            {
                Draw_Line(angle, distance, color_Point, bmp);
            }
            myGraphics.Dispose();
            myPen.Dispose();
            myBrush.Dispose();
        }
        public void Draw_Line(int angle, int length, Color linecolor, Bitmap bmp)
        {
            int x = 0;
            int y = 0;
            if (angle == -1)
            {
                angle = 359;
            }
            if (angle >= 0 && angle <= 180)
            {
                //right half
                //u in degree is converted into radian.

                x = xloc_Radar + (int)(length * Math.Sin(Math.PI * angle / 180));
                y = yloc_Radar - (int)(length * Math.Cos(Math.PI * angle / 180));
            }
            else
            {
                x = xloc_Radar - (int)(length * -Math.Sin(Math.PI * angle / 180));
                y = yloc_Radar - (int)(length * Math.Cos(Math.PI * angle / 180));
            }
            Pen myPen = new Pen(linecolor, penSz_SweepLine);
            Graphics myGraphics = Graphics.FromImage(bmp);
            myGraphics.DrawLine(myPen, new Point(xloc_Radar, yloc_Radar), new Point(x, y));
            myGraphics.Dispose();
            myPen.Dispose();
        }
    }
}
