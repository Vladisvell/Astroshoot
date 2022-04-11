using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Astroshooter
{
    class Controller
    {
        private Ship ship;
        private SpaceField spacefield;
        private List<SpaceObject> spaceObjects;

        public Controller(Ship ship, SpaceField spacefield, List<SpaceObject> spaceObjects)
        {
            this.ship = ship;
            this.spacefield = spacefield;
            this.spaceObjects = spaceObjects;
        }

        private bool InBounds()
        {
            return ship.Location.X < spacefield.Width
                && ship.Location.X > 0
                && ship.Location.Y > 0
                && ship.Location.Y < spacefield.Height;
        }

        private void BoundaryCollisionHandle()
        {
            if (ship.Location.X < -32) ship.SetCurrentCoordinates(spacefield.Width, ship.Location.Y);
            if (ship.Location.X > spacefield.Width+32) ship.SetCurrentCoordinates(0, ship.Location.Y);
            if (ship.Location.Y < -32) ship.SetCurrentCoordinates(ship.Location.X, spacefield.Height);
            if (ship.Location.Y > spacefield.Height+32) ship.SetCurrentCoordinates(ship.Location.X, 0);
        }

        void Collisions()
        {
            foreach (var spaceObject in spaceObjects)
                CollisionCheck(ship, spaceObject);
        }

        public void CollisionCheck(SpaceObject left, SpaceObject right)
        {
            if (left.IsCollided(right))
                spacefield.BackColor = System.Drawing.Color.Green;
        }

        void SimulateSpaceObjectsTimeFrame(double dt)
        {
            foreach (var spaceObject in spaceObjects)
                spaceObject.SimulateTimeFrame(dt);
        }

        public void UpdateSimulation(double dt)
        {
            SimulateSpaceObjectsTimeFrame(dt);
            if (!InBounds()) BoundaryCollisionHandle();
            Collisions();
        }
    }
}
