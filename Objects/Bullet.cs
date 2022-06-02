using System;
using System.Drawing;

namespace Astroshooter 
{
    class Bullet : SpaceObject
    {
        public Vec2 GetCoordinates()
        {
            return location;
        }

        public Image GetImage()
        {
            return texture;
        }

        public Size GetSize()
        {
            return texture.Size;
        }


        bool isDead;

        public bool IsCollided(SpaceObject spaceObject)
        {
            var objCords = spaceObject.GetCoordinates();
            var objSize = spaceObject.GetSize();
            var objRad = objSize.Width / 2;
            var thisRad = texture.Size.Width / 2;

            var dist = Vec2.GetDistanceBetween(GetCoordinates(), spaceObject.GetCoordinates());

            if (
                    this.location.X < objCords.X + objSize.Width
                    && this.location.X + texture.Width > objCords.X
                    && this.location.Y < objCords.Y + objSize.Height
                    && this.location.Y + texture.Height > objCords.Y
                )
            {
                if (spaceObject is Ship)
                    return false;
                if (spaceObject is Asteroid)
                {
                    isDead = true;
                    return true;
                }
            }
                return false;
        }

        public void SetCurrentCoordinates(double x, double y)
        {

        }

        public void SimulateTimeFrame(double dt)
        {
            UpdatePosition(dt);
            UpdateTimeToLive(dt);
            UpdateVelocity(dt);
        }

        public void UpdateTimeToLive(double dt)
        {
            timeToLive -= dt;
            if (timeToLive <= 0)
                isDead = true;
        }

        private void UpdatePosition(double dt)
        {
            location.X += velocity.X * dt;
            location.Y += velocity.Y * dt;
        }

        private void UpdateVelocity(double dt)
        {
            velocity += acceleration * dt;     
        }

        public bool IsDead() => isDead;

        Vec2 location = new Vec2();
        Vec2 velocity = new Vec2();
        Vec2 acceleration = new Vec2();

        Image texture = Image.FromFile("textures/bullet/bullet.png");
        public double timeToLive { get; private set; }

        Bullet()
        {

        }
        public Bullet(Vec2 spawnloc, Vec2 velocity, Image image = null)
        {
            location = spawnloc * 1;
            this.velocity = velocity * 1;
            if (image != null)
                texture = image;
            timeToLive = 2000;
        }
    }
}
