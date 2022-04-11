using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

namespace Astroshooter
{
    class Ship : SpaceObject
    {
        public readonly Image ShipTexture;
        public Vector Location { get; private set; }
        
        private Vector velocity = new Vector();
        private double thrustForce = 0;
        private Vector ResistanceForce = new Vector();
        private Vector acceleration = new Vector();
        private double mass = 2;
        private double inertia = 0.001;

        public float Direction { get; private set; }

        public Image GetTexture() => ShipTexture;

        public Vector GetCurrentCoordinates() => Location;

        public Vector GetAcceleration() => acceleration;

        public double GetThrustForce() => thrustForce;

        public void SetThrustForce(double newForce) => thrustForce = newForce; 

        public Vector GetVelocity() => velocity;

        public void SetCurrentCoordinates(double x, double y)
        {
            Location.X = x;
            Location.Y = y;
        }

        public void ChangeDirection(float newDirection) => Direction = newDirection;

        private void UpdateAcceleration(double dt)
        {
            acceleration.X = -Math.Cos(Direction / 180 * Math.PI) * thrustForce / mass;
            acceleration.Y = -Math.Sin(Direction / 180 * Math.PI) * thrustForce / mass;
        }

        private void UpdateVelocity(double dt)
        {
            velocity.X += acceleration.X * dt + ResistanceForce.X * dt;
            velocity.Y += acceleration.Y * dt + ResistanceForce.Y * dt;
        }

        private void UpdatePosition(double dt)
        {
            Location.X += velocity.X * dt;
            Location.Y += velocity.Y * dt;
        }

        private void UpdateResistance()
        {
            ResistanceForce.X = -velocity.X / 100;
            ResistanceForce.Y = -velocity.Y / 100;
        }

        public void SimulateTimeFrame(double dt)
        {
            UpdateAcceleration(dt);         
            UpdateVelocity(dt);
            UpdateResistance();
            UpdatePosition(dt);
            DeapplyForce();
            
        }

        public void ApplyForce()
        {
            if (thrustForce == 0)
                thrustForce += 0.01;
            if (thrustForce < 0.1)
                thrustForce += 0.003;
        }

        public void DeapplyForce()
        {
            if (thrustForce > 0)
            {
                thrustForce -= thrustForce / 10;
                thrustForce = Math.Round(thrustForce, 7);
            }
                
            if (Math.Abs(acceleration.X) > 0 || Math.Abs(acceleration.Y) > 0)
            {
                acceleration.X -= acceleration.X / 100;
                acceleration.Y -= acceleration.Y / 100;
                acceleration.X = Math.Round(acceleration.X, 2);
                acceleration.Y = Math.Round(acceleration.Y, 2);
            }         
        }

        public Vector GetCoordinates() => Location;

        public bool IsCollided(SpaceObject spaceObject)
        {
            var objCords = spaceObject.GetCoordinates();
            var objSize = spaceObject.GetSize();
            if (
                    this.Location.X < objCords.X + objSize.Width
                    && this.Location.X + ShipTexture.Width > objCords.X
                    && this.Location.Y < objCords.Y + objSize.Height
                    && this.Location.Y + ShipTexture.Height > objCords.Y
                )
                return true;
            return false;
        }

        public Size GetSize() => ShipTexture.Size;

        public Ship(Vector spawnPoint)
        {
            ShipTexture = Image.FromFile("textures/ship/ship.bmp");
            Location = new Vector(spawnPoint.X, spawnPoint.Y);
        }

    }
}
