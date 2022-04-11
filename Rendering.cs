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
        static Image ShipTexture;
        static Vector shipCords;
        float angle = 0;
        private Controller controller;
        Point mousecords;

        private List<Asteroid> asteroids;
        private List<SpaceObject> spaceObjects;

        private Label velocity;
        private Label acceleration;
        private Label thrustforce;
        private Label direction;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public SpaceField()
        {
            InitializeComponent();
            ship = new Ship(new Vector(ClientSize.Width / 2 ,ClientSize.Height / 2));
            shipCords = ship.Location;

            asteroids = new List<Asteroid>();
            asteroids.Add(new Asteroid(new Vector(100, 100), new Vector(0.001, 0.001)));

            spaceObjects = new List<SpaceObject>();
            spaceObjects.Add(new Asteroid(new Vector(100, 100), new Vector(0.001, 0.001)));

            controller = new Controller(ship, this, spaceObjects);


            ShipTexture = ship.ShipTexture;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = "Astroshooter";
            DoubleBuffered = true;
            
            BackColor = Color.Black;
            InitializeTimer();
            InitializeLabels();
        }

        public void InitializeLabels()
        {
            velocity = new Label
            {
                Location = new Point(0, 10),
                Size = new Size(100, 15),
                Text = "Velocity = ",
                ForeColor = Color.White,     
            };
            acceleration = new Label
            {
                Location = new Point(0, 70),
                Size = new Size(100, 15),
                Text = "Acceleration = ",
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            thrustforce = new Label
            {
                Location = new Point(0, 130),
                Size = new Size(100, 15),
                Text = "Acceleration = ",
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            direction = new Label
            {
                Location = new Point(0, 190),
                Size = new Size(130, 15),
                Text = "Direction = ",
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            Controls.Add(velocity);
            Controls.Add(acceleration);
            Controls.Add(thrustforce);
            Controls.Add(direction);

        }

        public void InitializeTimer()
        {
            timer.Interval = 10;
            timer.Tick += (sender, args) => Invalidate(sender, args);
            timer.Tick += (sender, args) => ship.SimulateTimeFrame(timer.Interval);
            timer.Tick += (sender, args) => controller.UpdateSimulation(timer.Interval);
            timer.Tick += (sender, args) => MovementDataTick();
            timer.Start();
        }

        public void MovementDataTick()
        {
            var vel = ship.GetVelocity();
            var thrust = ship.GetThrustForce();
            var accel = ship.GetAcceleration();
            velocity.Text = "Velocity = " + Math.Round(Math.Sqrt(vel.X * vel.X + vel.Y * vel.Y), 5).ToString();
            thrustforce.Text = "Thrust = " + thrust.ToString();
            acceleration.Text = "Acceleration = " + Math.Sqrt(accel.X * accel.X + accel.Y * accel.Y);
            direction.Text = "Direction = " + ship.Direction.ToString();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.W)
            {
                ship.ApplyForce();
            }
            if (e.KeyCode == Keys.R)
                BackColor = Color.Black;
        }
            

        void Invalidate(object sender, EventArgs e) {this.Invalidate(); this.Update(); }


        protected override void OnPaint(PaintEventArgs e)
        {
            shipCords = ship.GetCurrentCoordinates();
            Graphics g = this.CreateGraphics();
            g.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            g.TranslateTransform((float)shipCords.X,
                (float)shipCords.Y);             
            g.RotateTransform(angle);
            ship.ChangeDirection(angle);
            g.DrawImage(ShipTexture, new Point(-ShipTexture.Width / 2, -ShipTexture.Height / 2));
            e.Graphics.DrawLine(Pens.Red, new PointF((float)shipCords.X, (float)shipCords.Y), mousecords);
            RenderAsteroids(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            mousecords = e.Location;
            RotateImageAngle(e);
        }

        public float RotateImageAngle(MouseEventArgs e)
        {
            var y2 = e.Y;
            var y1 = shipCords.Y + ShipTexture.Height / 2;
            var x2 = e.X;
            var x1 = shipCords.X + ShipTexture.Width / 2;         
            angle = (float)(Math.Atan2((y1 - y2), (x1 - x2)) * 180 / Math.PI);
            return 0;
        }

        void RenderAsteroids(PaintEventArgs e)
        {
            foreach (var asteroid in asteroids)
                e.Graphics.DrawImage(asteroid.texture, new PointF((float)asteroid.Location.X, (float)asteroid.Location.Y));
        }


        
    }
}
