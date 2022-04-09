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
using System.Drawing.Drawing2D;

namespace Astroshooter
{
    public partial class SpaceField : Form
    {
        Ship ship;
        Image ShipTexture;
        Image background;
        Vector shipCords;
        double rotation = 10;

        public SpaceField()
        {
            InitializeComponent();
            //this.BackColor = Color.Black;
            this.Name = "Asteroids";
            this.background = new Bitmap(ClientSize.Width, ClientSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            ship = new Ship(new Vector(200,200));
            
            ShipTexture = ship.ShipTexture;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            

            Bitmap bmp = new Bitmap(ShipTexture.Width, ShipTexture.Height);

            base.OnPaint(e);

            var shipImage = RotateImage(ShipTexture, 90);
            //ShipTexture = shipImage;

            //e.Graphics.DrawImage(shipImage, 100, 100);
            shipCords = ship.GetCurrentCoordinates();
            var timer = new System.Windows.Forms.Timer();
            e.Graphics.DrawImage(shipImage, (float)shipCords.X, (float)shipCords.Y);
            timer.Tick += (sender, args) => Invalidate();
            timer.Interval = 1000;
            timer.Start();
        }

        private void Transform(Graphics g)
        {

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillRectangle(Brushes.Black, ClientRectangle);

            g.TranslateTransform((float)shipCords.X, (float)shipCords.Y);
            g.RotateTransform(90 + (float)(ship.Direction * 180 / Math.PI));
        }

        public static Image RotateImage(Image img, float rotationAngle)
        {
            //create an empty Bitmap image
            Bitmap bmp = new Bitmap(img.Width, img.Height);

            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            //turn the Bitmap into a Graphics object
            Graphics gfx = Graphics.FromImage(bmp);

            //now we set the rotation point to the center of our image
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

            //now rotate the image
            gfx.RotateTransform(rotationAngle);

            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            //gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //now draw our new image onto the graphics object
            gfx.DrawImage(img, new Point(0, 0));

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            return bmp;
        }


    }
}
