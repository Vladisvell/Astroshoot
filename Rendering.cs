﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Timers;
using System.Drawing.Drawing2D;
using System.Windows.Input;
using System.IO;


namespace Astroshooter
{
    public partial class SpaceField : Form
    {
        Ship ship;
        static Image ShipTexture;
        static Vec2 shipCords;
        float angle = 0;
        private Controller controller;
        Point mousecords;

        private List<SpaceObject> spaceObjects;

        private bool isSpacePressed = false;
        private bool isWPressed = false;
        private bool isRPressed = false;
        private bool isLPressed = false;
        private bool debugCollisions = false;
        private bool debugModeEnabled = false;

        private Label velocity;
        private Label acceleration;
        private Label thrustforce;
        private Label direction;

        List<Image> asteroidTextures;

        Random random;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer spawnTimer = new System.Windows.Forms.Timer();

        public SpaceField(string[] args)
        {
            
            InitializeComponent();

            //if (args.Contains("-debug"))
                debugModeEnabled = true;
            

            asteroidTextures = InitializeAsteroidImages();
            ship = new Ship(new Vec2(ClientSize.Width / 2 ,ClientSize.Height / 2));
            shipCords = ship.Location;

            random = new Random((int)DateTime.Now.Ticks);

            spaceObjects = new List<SpaceObject>(512);

            spaceObjects.Add(ship);

            controller = new Controller(ship, this, spaceObjects, asteroidTextures);

            


            ShipTexture = ship.ShipTexture;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = "Astroshooter";
            DoubleBuffered = true;
            
            BackColor = Color.Black;
            InitializeTimer();
            if(debugModeEnabled)
                InitializeLabels();
            //Cursor.Hide();
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
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

        public List<Image> InitializeAsteroidImages()
        {
            string asteroidDirectoryPath = @".\textures\asteroid";
            string directory = Directory.GetCurrentDirectory();
            var files = Directory.EnumerateFiles(asteroidDirectoryPath, "*.png", SearchOption.AllDirectories);
            var ech = files.ToList();
            var imageList = files.Select(x => Image.FromFile(x)).ToList();
            return imageList;
        }

        public void InitializeTimer()
        {
            timer.Interval = 10;
            timer.Tick += (sender, args) => ship.SimulateTimeFrame(timer.Interval);
            timer.Tick += (sender, args) => controller.UpdateSimulation(timer.Interval);
            timer.Tick += (sender, args) => MovementDataTick();
            timer.Tick += (sender, args) => Invalidate(sender, args);
            timer.Tick += (sender, args) => InputTick();
            timer.Start();

            spawnTimer.Interval = 1000;
            timer.Tick += (sender, args) => controller.CreateRandomAsteroid();
        }

        public void MovementDataTick()
        {
            var vel = ship.GetVelocity();
            var thrust = ship.GetThrustForce();
            var accel = ship.GetAcceleration();
            if (debugModeEnabled)
            {
                velocity.Text = "Velocity = " + Math.Round(Math.Sqrt(vel.X * vel.X + vel.Y * vel.Y), 5).ToString();
                thrustforce.Text = "Thrust = " + thrust.ToString();
                acceleration.Text = "Acceleration = " + Math.Sqrt(accel.X * accel.X + accel.Y * accel.Y);
                direction.Text = "Direction = " + ship.Direction.ToString();
            }

        }

        public void InputTick()
        {
            if (isSpacePressed && !ship.IsDead())
            {
                if (ship.cooldown <= 0)
                {
                    var pelleteVelocity = new Vec2();
                    pelleteVelocity.X = -Math.Cos(angle / 180 * Math.PI);
                    pelleteVelocity.Y = -Math.Sin(angle / 180 * Math.PI);
                    spaceObjects.Add(new Bullet(ship.GetCurrentCoordinates(), pelleteVelocity)); //пока это торпеда
                    ship.SetShootCooldown(300);
                }
            }
            if (isWPressed && !ship.IsDead())
                ship.ApplyForce();
            if (isRPressed)
                BackColor = Color.Black;
            if (isLPressed)
                controller.CreateRandomAsteroid();

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space)
                isSpacePressed = true;
            if (e.KeyCode == Keys.W)
                isWPressed = true;
            if (e.KeyCode == Keys.R)
                isRPressed = true;
            if (e.KeyCode == Keys.T)
                debugCollisions = !debugCollisions;
            if (e.KeyCode == Keys.L)
                controller.CreateRandomAsteroid();
            if (e.KeyCode == Keys.E)
                controller.Respawn();
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
            if (e.KeyCode == Keys.Pause)
                timer.Enabled = !timer.Enabled;

        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Space)
                isSpacePressed = false;
            if (e.KeyCode == Keys.W)
                isWPressed = false;
            if (e.KeyCode == Keys.R)
                isRPressed = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
                isSpacePressed = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
                isSpacePressed = false;
        }

        void Invalidate(object sender, EventArgs e) {this.Invalidate();  }


        protected override void OnPaint(PaintEventArgs e)
        {
            shipCords = ship.GetCurrentCoordinates();
            Graphics g = null;

            g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //рендер корабля
            if (!ship.IsDead())
            {
                g.TranslateTransform((float)shipCords.X,
                 (float)shipCords.Y);
                g.RotateTransform(angle);
                ship.ChangeDirection(angle);
                g.DrawImage(ShipTexture, new Point(-ShipTexture.Width / 2, -ShipTexture.Height / 2));
                e.Graphics.ResetTransform();
                //e.Graphics.DrawImage(ShipTexture, (float)shipCords.X, (float)shipCords.Y);
                e.Graphics.DrawLine(Pens.Red, new PointF((float)shipCords.X, (float)shipCords.Y), mousecords);
            }
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
            foreach (var spaceobject in spaceObjects)
            {
                if(spaceobject != null && !(spaceobject is Ship))
                {
                    var objectLocation = spaceobject.GetCoordinates();

                    e.Graphics.DrawImage(spaceobject.GetImage(), Vec2.TransformToPointF(objectLocation));
                    if (debugCollisions)
                    {
                        var kek = spaceobject.GetImage();
                        var objectSize = spaceobject.GetSize();
                        var rect = new RectangleF(Vec2.TransformToPointF(objectLocation), new SizeF((float)kek.Size.Width, (float)kek.Size.Height));
                        //e.Graphics.DrawRectangle(new Pen(Color.Red), rect);
                        e.Graphics.DrawRectangles(new Pen(Color.Red), new RectangleF[] { rect });
                        e.Graphics.DrawString("KEKW", SystemFonts.DefaultFont, Brushes.LightGreen, Vec2.TransformToPointF(objectLocation));
                    }
                }
            }
        }   
    }
}
