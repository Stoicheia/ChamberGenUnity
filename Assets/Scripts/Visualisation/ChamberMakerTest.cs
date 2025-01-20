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

        public static ChamberGlobal ProtoChamberOrth4(int radius, float exitNodeDist01)
        {
            return ProtoChamberOrth(radius, exitNodeDist01, 0b1111);
        }

        public static ChamberGlobal ProtoChamberOrth(int radius, float exitNodeDist01, byte includedDirectionsBitmaskENWS)
        {
            bool east = (includedDirectionsBitmaskENWS & 0b1000) > 0;
            bool north = (includedDirectionsBitmaskENWS & 0b0100) > 0;
            bool west = (includedDirectionsBitmaskENWS & 0b0010) > 0;
            bool south = (includedDirectionsBitmaskENWS & 0b0001) > 0;
 
            ChamberGlobal chamber = new ChamberGlobal(radius);
            List<ExitNodeGlobal> exits = new List<ExitNodeGlobal>();
            if(east)
                exits.Add(new ExitNodeGlobal(exitNodeDist01, 0, 0));
            if(west)
                exits.Add(new ExitNodeGlobal(exitNodeDist01, (float)Math.PI, 180));
            if(north)
                exits.Add(new ExitNodeGlobal(exitNodeDist01, (float)Math.PI/2, 90));
            if(south)
                exits.Add(new ExitNodeGlobal(exitNodeDist01, (float)Math.PI*3/2, 270));
            chamber.SetExitNodes(exits);
            return chamber;
        }
    }
}