using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Astroshooter
{
    class Controller
    {
        private Ship ship;
        private SpaceField spacefield;
        private List<SpaceObject> spaceObjects;
        private Random random = new Random((int)DateTime.Now.Ticks);
        private List<Image> asteroidTextureList;
        private Queue<SpaceObject> toSpawn = new Queue<SpaceObject>(100);
        private bool isPaused;
        private double circularParameter = 0;


        public Controller(Ship ship, SpaceField spacefield, List<SpaceObject> spaceObjects, List<Image> asteroidImages)
        {
            this.ship = ship;
            this.spacefield = spacefield;
            this.spaceObjects = spaceObjects;
            asteroidTextureList = asteroidImages;
        }

        private void BoundaryCollisionHandle(SpaceObject obj)
        {
            var Location = obj.GetCoordinates();

            if (Location.X < -64) obj.SetCurrentCoordinates(spacefield.Width, Location.Y);
            if (Location.X > spacefield.Width + 64) obj.SetCurrentCoordinates(0, Location.Y);
            if (Location.Y < -64) obj.SetCurrentCoordinates(ship.Location.X, spacefield.Height);
            if (Location.Y > spacefield.Height + 64) obj.SetCurrentCoordinates(Location.X, -32);
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
            if (left.IsCollided(right))
            {
                if(left is Asteroid)
                {
                    var leftaster = left as Asteroid;
                    leftaster.Collide(right);
                }
            }           
        }

        private Queue<int> toDeleteQ = new Queue<int>(32);

        private List<int> toDelete = new List<int>(512);

        void SimulateSpaceObjectsTimeFrame(double dt)
        {
            BoundaryCollisionHandle(ship);

            for(int i = 0; i < spaceObjects.Count; i++)
            {
                //if(spaceObjects[i] != null)
               //{
                    spaceObjects[i].SimulateTimeFrame(dt);
                    BoundaryCollisionHandle(spaceObjects[i]);
                    Collision(spaceObjects[i]);
                    if (spaceObjects[i].IsDead())
                        toDelete.Add(i);
                //}
            }

            //while(toDeleteQ.Count != 0)
            //{
            //    var idToDelete = toDeleteQ.Dequeue();
            //    if (idToDelete == spaceObjects.Count - 1)
            //        spaceObjects.RemoveAt(spaceObjects.Count - 1);
            //    else if(idToDelete < spaceObjects.Count - 1)
            //    {
            //        spaceObjects[idToDelete] = spaceObjects[spaceObjects.Count - 1];
            //        spaceObjects.RemoveAt(spaceObjects.Count - 1);
            //    }          
            //}

            for(int i = 0; i < toDelete.Count; i++)
            {
                
                spaceObjects[toDelete[i] - i] = spaceObjects[spaceObjects.Count - 1];
                spaceObjects.RemoveAt(spaceObjects.Count - 1);
            }

            toDelete.Clear();

            while (toSpawn.Count != 0)
                spaceObjects.Add(toSpawn.Dequeue());
        }



        public void UpdateSimulation(double dt)
        {
            SimulateSpaceObjectsTimeFrame(dt);              
        }        

        private Vec2 GetRandomPositionOutsidePlayzone()
        {
            var xcord = random.Next() * 1920 % 30000;
            var ycord = random.Next() * 1080 % 30000;
            return new Vec2(xcord, ycord);
        }

        private Vec2 GetRandomVelocity()
        {
            var xvel = random.Next(-10, 10) / 100d;
            var yvel = random.Next(-10, 10) / 100d;
            return new Vec2(xvel, yvel);
        }

        public void CreateRandomAsteroid()
        {
            //if(spaceObjects.Count <= 32 && toSpawn.Count <= 16)
            //{
            //    Vec2 pos = GetRandomPositionOutsidePlayzone();
            //    Vec2 vel = GetRandomVelocity();
            //    var asteroid = new Asteroid(pos, vel * 3, asteroidTextureList[random.Next(0, asteroidTextureList.Count - 1)]);
            //    asteroid.SetCooldown(2000);
            //    toSpawn.Enqueue(asteroid);
            //}
            CreateAsteroidInCircle(2000);
            //return new Asteroid(pos, vel, asteroidTextureList[random.Next(0, asteroidTextureList.Count-1)]);
        }

        public void CreateAsteroidInCircle(double spawnRadius)
        {
            circularParameter %= Math.PI * 2;
            if(spaceObjects.Count <= 32 && toSpawn.Count <= 16)
            {
                Vec2 pos = new Vec2()
                {
                    X = spawnRadius * Math.Cos(circularParameter) + spacefield.Width / 2,
                    Y = spawnRadius * Math.Sin(circularParameter) + spacefield.Height / 2
                };
                circularParameter += Math.PI / 6;
                Vec2 vel = GetRandomVelocity();
                var asteroid = new Asteroid(pos, vel * 3, asteroidTextureList[random.Next(0, asteroidTextureList.Count - 1)]);
                asteroid.SetCooldown(300);
                toSpawn.Enqueue(asteroid);
            }
        }

        public void Respawn()
        {
            if (ship.IsDead())
            {
                ship.SetDeadState(false);
                ship.SetCurrentCoordinates(spacefield.Width / 2, spacefield.Height / 2);
                spaceObjects.Add(ship);
            }
        }
    }
}
