using System;
using System.Collections.Generic;
using ChamberGen;
using Utility;

namespace Visualisation
{
    public static class ChamberMakerTest
    {
        public static ChamberGlobal PlaceChamberR(int x, int y)
        {
            ChamberGlobal chamber = new ChamberGlobal(20);
            List<ExitNodeGlobal> exits = new List<ExitNodeGlobal>();
            exits.Add(new ExitNodeGlobal(0.9f, 0, 0));
            chamber.SetExitNodes(exits);
            ChamberGlobal placedChamber = chamber.Instantiate(new VectorInt(x, y));
            return placedChamber;
        }
        
        public static ChamberGlobal PlaceChamberL(int x, int y)
        {
            ChamberGlobal chamber = new ChamberGlobal(20);
            List<ExitNodeGlobal> exits = new List<ExitNodeGlobal>();
            exits.Add(new ExitNodeGlobal(0.9f, (float)Math.PI, 180));
            chamber.SetExitNodes(exits);
            ChamberGlobal placedChamber = chamber.Instantiate(new VectorInt(x, y));
            return placedChamber;
        }
    }
}