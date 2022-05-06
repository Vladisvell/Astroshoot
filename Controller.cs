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
        private List<int> toDelete = new List<int>(512);

        public Controller(Ship ship, SpaceField spacefield, List<SpaceObject> spaceObjects)
        {
            this.ship = ship;
            this.spacefield = spacefield;
            this.spaceObjects = spaceObjects;
        }

        private void BoundaryCollisionHandle(SpaceObject obj)
        {
            var Location = obj.GetCoordinates();

            if (Location.X < -32) obj.SetCurrentCoordinates(spacefield.Width, Location.Y);
            if (Location.X > spacefield.Width + 32) obj.SetCurrentCoordinates(0, Location.Y);
            if (Location.Y < -32) obj.SetCurrentCoordinates(ship.Location.X, spacefield.Height);
            if (Location.Y > spacefield.Height + 32) obj.SetCurrentCoordinates(Location.X, -16);
        }

        void Collision(SpaceObject spaceobject)
        {
            for(int i = 0; i < spaceObjects.Count; i++)
            {
                if(spaceobject != null && spaceObjects[i] != null)
                {
                    if (spaceobject != spaceObjects[i])
                        CollisionCheck(spaceObjects[i], spaceobject);
                    CollisionCheck(spaceObjects[i], ship);
                }
            }
        }

        public void CollisionCheck(SpaceObject left, SpaceObject right)
        {
            if (left.IsCollided(right) && (left is Ship || right is Ship))
                spacefield.BackColor = System.Drawing.Color.Green;
            if (left.IsCollided(right))
            {
                if(left is Asteroid)
                {
                    var leftaster = left as Asteroid;
                    leftaster.Collide(right);
                }
            }           
        }

        void SimulateSpaceObjectsTimeFrame(double dt)
        {
            BoundaryCollisionHandle(ship);

            for(int i = 0; i < spaceObjects.Count; i++)
            {
                if(spaceObjects[i] != null)
                {
                    spaceObjects[i].SimulateTimeFrame(dt);
                    BoundaryCollisionHandle(spaceObjects[i]);
                    Collision(spaceObjects[i]);
                    if (spaceObjects[i].IsDead())
                        toDelete.Add(i);
                }
            }
            for(int i = 0; i < toDelete.Count; i++)
            {
                if(toDelete[i] != -1)
                {
                    spaceObjects[toDelete[i]] = spaceObjects[spaceObjects.Count - 1];
                    spaceObjects.RemoveAt(spaceObjects.Count - 1);
                }
                toDelete[i] = -1;
            }       
            toDelete.Clear();
        }

        public void UpdateSimulation(double dt)
        {
            SimulateSpaceObjectsTimeFrame(dt);              
        }
    }
}
