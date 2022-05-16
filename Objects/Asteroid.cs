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
        public Vec2 Location { get; private set; }

        public Vec2 Velocity { get; private set; }

        bool isDead;

        public double mass = 1;

        public double MovingAngle 
        { 
            get 
            {
                return Math.Atan2(Velocity.Y, Velocity.X);      
            } 
            private set { }
        }

        private double cooldown = 0;

        public Asteroid(Vec2 spawn, Vec2 velocity, Image texture = null)
        {
            if (texture == null)
                texture = Image.FromFile("textures/asteroid/asteroid_1.png");
            else
                this.texture = texture;
            Location = spawn;
            Velocity = velocity;
        }


        public void SimulateTimeFrame(double dt)
        {
            UpdatePosition(dt);
            UpdateCooldown(dt);
        }

        private void UpdateCooldown(double dt)
        {
            if(cooldown > 0)
                cooldown -= dt;
            if (cooldown < 0)
                cooldown = 0;
        }
        private void UpdatePosition(double dt)
        {
            Location.X += Velocity.X * dt;
            Location.Y += Velocity.Y * dt;
        }

        public Vec2 GetCoordinates() => Location;

        public bool IsCollided(SpaceObject spaceObject)
        {         
            var objCords = spaceObject.GetCoordinates();
            var objSize = spaceObject.GetSize();
            var objRad = objSize.Width / 2;
            var thisRad = texture.Size.Width / 2;
            if (
                    this.Location.X < objCords.X + objSize.Width
                    && this.Location.X + texture.Width > objCords.X
                    && this.Location.Y < objCords.Y + objSize.Height
                    && this.Location.Y + texture.Height > objCords.Y
                )
                return true;
            return false;
        }

        public void Collide(SpaceObject spaceObject)
        {
            if (spaceObject is Asteroid && cooldown <= 0)
            {
                var pointer = spaceObject as Asteroid;
                
                {

                    var newPointerVel = (pointer.Velocity * (pointer.mass - mass) + Velocity * 2 * mass) * (1 / (mass + pointer.mass));
                    var newVel = (Velocity * (mass - pointer.mass) + pointer.Velocity * 2 * pointer.mass) * (1 / (mass + pointer.mass));

                    pointer.Velocity = newPointerVel;
                    Velocity = newVel;
                    cooldown = 420;
                    pointer.cooldown = 420;
                }
            }
            if (spaceObject is Ship)
            {
                isDead = true;
            }
        }

        public Size GetSize() => texture.Size;

        public void SetCurrentCoordinates(double x, double y) => Location = new Vec2(x, y);

        public Image GetImage() => texture;

        public bool IsDead() => isDead;
    }
}
