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
        private Color color_Circle = Color.GreenYellow;
        private Color color_Line = Color.Green;
        private Color color_SweepLine = Color.Aqua;
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

        private float angle_SweepLine;
        private int index_Data;

        private int divisions_Angle;    // total number of divisions of 360 degrees

        private int[] data_Global;
        private int[] data_Covered;

        private PictureBox pictureBox_Radar;
        private Bitmap bmp_Radar;
        private Bitmap bmp_BaseRadar;
        private Graphics graphics_Radar;
        private Pen pen_Radar;
        private Brush brush_RadarBackground;
        private Brush brush_Point;
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
            pictureBox_Radar.Location = new System.Drawing.Point(xloc_PictureBox, yloc_PictureBox);
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

            graphics_Radar = Graphics.FromImage(bmp_Radar);
            brush_RadarBackground = new SolidBrush(color_Background);
            brush_Point = new SolidBrush(color_Point);
            pen_Radar = new Pen(Color.GreenYellow, penSz_Circle);
            draw_Radar(bmp_Radar);
            form.ResumeLayout();

        }
        public void update_Global(Form form, int angle, int value)
        {
            form.SuspendLayout();
            
            data_Global[angle] = value * length_SweepLine / 100;

            draw_Radar(bmp_Radar);
            angle_SweepLine = (float)(float)(360 * (float)angle / (float)this.divisions_Angle);
            index_Data = angle;
            Draw_Line(angle_SweepLine, length_SweepLine, color_SweepLine, bmp_Radar);

            for (int index = 0; index < divisions_Angle; index++)
            {
                if (data_Covered[index] != 0 && index != angle_SweepLine)
                {
                    float tempAngle = (float)(float)(360 * (float)index / (float)this.divisions_Angle);
                    Draw_point(tempAngle, data_Covered[index], color_Point, bmp_Radar, false);
                }
            }
            Draw_point(angle_SweepLine, data_Global[index_Data], color_Point, bmp_Radar, false);
            data_Covered[index_Data] = data_Global[index_Data];
            //load bitmap in picturebox1
            pictureBox_Radar.Image = bmp_Radar;
            form.ResumeLayout();

        }
        public void draw_Radar(Bitmap bmp)
        {
            Draw_Circle(size_Radar / 2, true, bmp);
            Draw_Circle((int)(0.75 * size_Radar / 2), true, bmp);
            Draw_Circle((int)(0.50 * size_Radar / 2), true, bmp);
            Draw_Circle((int)(0.25 * size_Radar / 2), true, bmp);
            Draw_Line(0, length_SweepLine, color_Line, bmp);
            Draw_Line(90, length_SweepLine, color_Line, bmp);
            Draw_Line(180, length_SweepLine, color_Line, bmp);
            Draw_Line(270, length_SweepLine, color_Line, bmp);
        }
        public void Draw_Circle(int radius, bool fill, Bitmap bmp)
        {
            if (fill)
            {
                graphics_Radar.FillEllipse(brush_RadarBackground, xloc_Radar - radius, yloc_Radar - radius, radius * 2, radius * 2);
            }
            pen_Radar.Color = color_Circle;
            pen_Radar.Width = penSz_Circle;
            graphics_Radar.DrawEllipse(pen_Radar, xloc_Radar - radius, yloc_Radar - radius, radius * 2, radius * 2);
            //myGraphics.Dispose();
        }
        public void Draw_point(float angle, int distance, Color colorPoint, Bitmap bmp, bool line = true)
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
            pen_Radar.Color = color_Point;
            pen_Radar.Width = penSz_Point;
            graphics_Radar.DrawEllipse(pen_Radar, center_x - radius, center_y - radius, radius * 2, radius * 2);
            graphics_Radar.FillEllipse(brush_Point, center_x - radius, center_y - radius, radius * 2, radius * 2);
            if (line)
            {
                Draw_Line(angle, distance, color_Point, bmp);
            }
        }
        public void Draw_Line(float angle, int length, Color linecolor, Bitmap bmp)
        {
            int x = 0;
            int y = 0;
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
            pen_Radar.Color = linecolor;
            pen_Radar.Width = penSz_SweepLine;
            graphics_Radar.DrawLine(pen_Radar, new Point(xloc_Radar, yloc_Radar), new Point(x, y));

        }
    }
}
