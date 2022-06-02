using System;
using System.Collections.Generic;
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
        private double circularParameter = 0;
        private readonly Dictionary<string, CachedSound> soundLibrary = new Dictionary<string, CachedSound>();
        private int score = 0;


        public Controller(Ship ship, SpaceField spacefield, List<SpaceObject> spaceObjects, List<Image> asteroidImages)
        {
            this.ship = ship;
            this.spacefield = spacefield;
            this.spaceObjects = spaceObjects;
            asteroidTextureList = asteroidImages;
            InitializeSoundLibrary();
        }

        private void InitializeSoundLibrary()
        {
            soundLibrary.Add("AsteroidExplode", new CachedSound(@".\audio\explode.wav"));
            soundLibrary.Add("ShipShootSound", new CachedSound(@".\audio\impulse.wav"));
        }

        public void CreateBullet(float angle)
        {
            if (ship.cooldown <= 0)
            {
                var pelleteVelocity = new Vec2();
                pelleteVelocity.X = -Math.Cos(angle / 180 * Math.PI);
                pelleteVelocity.Y = -Math.Sin(angle / 180 * Math.PI);
                spaceObjects.Add(new Bullet(ship.GetCurrentCoordinates(), pelleteVelocity));
                AudioPlaybackEngine.Instance.PlaySound(soundLibrary["ShipShootSound"]);
                ship.SetShootCooldown(300);
            }
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
                    spaceObjects[i].SimulateTimeFrame(dt);
                    BoundaryCollisionHandle(spaceObjects[i]);
                    Collision(spaceObjects[i]);
                    if (spaceObjects[i].IsDead())
                        toDelete.Add(i);
            }

            for(int i = 0; i < toDelete.Count; i++)
            {
                if(spaceObjects[toDelete[i] - i] is Asteroid)
                {
                    if (spacefield.isSoundEnabled)
                        AudioPlaybackEngine.Instance.PlaySound(soundLibrary["AsteroidExplode"]);
                    score += 100;
                    spacefield.UpdateScore(score);
                }
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

        public void CreateRandomAsteroid() => CreateAsteroidInRadius(2000);

        public void CreateAsteroidInRadius(double spawnRadius)
        {
            circularParameter %= Math.PI * 2;
            if(spaceObjects.Count <= 32 && toSpawn.Count <= 16)
            {
                circularParameter += Math.PI / 6;
                Vec2 pos = new Vec2()
                {
                    X = spawnRadius * Math.Cos(circularParameter) + spacefield.Width / 2,
                    Y = spawnRadius * Math.Sin(circularParameter) + spacefield.Height / 2
                };
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
                score = 0;
                spacefield.UpdateScore(score);
                ship.SetCurrentCoordinates(spacefield.Width / 2, spacefield.Height / 2);
                spaceObjects.Add(ship);
            }
        }
    }
}
