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
        Vector GetCoordinates();
        Size GetSize();
        
        bool IsCollided(SpaceObject spaceObject);

        void SimulateTimeFrame(double dt);
    }
}
