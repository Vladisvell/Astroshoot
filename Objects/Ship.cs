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
        public Vec2 Location { get; private set; }

        private Vec2 velocity = new Vec2();
        private double thrustForce = 0;
        private Vec2 ResistanceForce = new Vec2();
        private Vec2 acceleration = new Vec2();
        private double mass = 2;
        private double inertia = 0.001;
        bool isDead;
        public double cooldown {get; private set;}

        public void SetShootCooldown(double setted) => cooldown = setted;

        public float Direction { get; private set; }

        private void UpdateCooldown(double dt) => cooldown -= dt;

        public Image GetTexture() => ShipTexture;

        public Vec2 GetCurrentCoordinates() => Location;

        public Vec2 GetAcceleration() => acceleration;

        public double GetThrustForce() => thrustForce;

        public void SetThrustForce(double newForce) => thrustForce = newForce; 

        public Vec2 GetVelocity() => velocity;

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
            UpdateCooldown(dt);
            
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
                thrustForce -= thrustForce / 5;
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

        public Vec2 GetCoordinates() => Location;

        public bool IsCollided(SpaceObject spaceObject)
        {
            var objCords = spaceObject.GetCoordinates();
            var objSize = spaceObject.GetSize();
            var objRad = objSize.Width * Math.Sqrt(2);
            var thisRad = ShipTexture.Size.Width * Math.Sqrt(2);
            if (
                    this.Location.X < objCords.X + objSize.Width
                    && this.Location.X + ShipTexture.Width > objCords.X
                    && this.Location.Y < objCords.Y + objSize.Height
                    && this.Location.Y + ShipTexture.Height > objCords.Y
                )
            {
                if (spaceObject is Asteroid)
                    isDead = true;
                return true;
            }
                return false;
        }

        public Size GetSize() => ShipTexture.Size;

        public Image GetImage()
        {
            return ShipTexture;
        }

        public bool IsDead() => isDead;

        public Ship(Vec2 spawnPoint)
        {
            ShipTexture = Image.FromFile("textures/ship/ship.bmp");
            Location = new Vec2(spawnPoint.X, spawnPoint.Y);
        }

    }
}
