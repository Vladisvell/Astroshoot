using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

namespace Astroshooter
{
    class Ship
    {
        public readonly Image ShipTexture;
        private Vector currentCoordinates = new Vector();
        public double Direction {get; private set; }

        public Image GetTexture() => ShipTexture;

        public Vector GetCurrentCoordinates() => currentCoordinates;

        public void ChangeDirection(double delta) => Direction += delta;

        public Ship(Vector spawnPoint)
        {
            ShipTexture = Image.FromFile("textures/ship/ship.png");
            currentCoordinates.X = spawnPoint.X;
            currentCoordinates.Y = spawnPoint.Y;
            Direction = 90;
        }
        
        public Ship(Image CustomTexture)
        {
            ShipTexture = CustomTexture;
        }

    }
}
