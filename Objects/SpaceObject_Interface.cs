using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Astroshooter
{
    public interface SpaceObject
    {
        Vec2 GetCoordinates();
        Size GetSize();
        
        bool IsCollided(SpaceObject spaceObject);

        void SimulateTimeFrame(double dt);

        void SetCurrentCoordinates(double x, double y);

        Image GetImage();

        bool IsDead();
    }
}
