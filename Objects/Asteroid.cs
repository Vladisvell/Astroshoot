using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astroshooter
{
    class Asteroid : SpaceObject
    {
        public Image texture { get; private set; }
        public Vector Location { get; private set; }

        public Vector Velocity { get; private set; }

        public Asteroid(Vector spawn, Vector velocity)
        {
            InitializeImage();
            Location = spawn;
            Velocity = velocity;
        }
        void InitializeImage()
        {
            texture = Image.FromFile("textures/asteroid/asteroid.png");
        }

        public void SimulateTimeFrame(double dt)
        {
            UpdatePosition(dt);
        }

        private void UpdatePosition(double dt)
        {
            Location.X += Velocity.X * dt;
            Location.Y += Velocity.Y * dt;
        }

        public Vector GetCoordinates() => Location;

        public bool IsCollided(SpaceObject spaceObject)
        {
            var objCords = spaceObject.GetCoordinates();
            var objSize = spaceObject.GetSize();
            if (
                    this.Location.X - texture.Width / 2 < objCords.X - objSize.Width / 2 + objSize.Width
                    && this.Location.X - texture.Width / 2 + texture.Width > objCords.X - objSize.Width / 2
                    && this.Location.Y - texture.Height / 2 < objCords.Y - objSize.Height / 2 + objSize.Height
                    && this.Location.Y + texture.Height / 2 > objCords.Y - objSize.Height / 2
                )
                return true;
            return false;
        }

        public Size GetSize()
        {
            return texture.Size;
        }
    }
}
