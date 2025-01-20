using System.Collections.Generic;
using ChamberGen;
using Utility;

namespace Visualisation
{
    public static class DrawTests
    {
        public static void DrawTest1(string dest)
        {
            MapDrawer mapDrawer = new MapDrawer();
            GlobalMap map = new GlobalMap(400, 400);
            ChamberGlobal chamberR = ChamberMakerTest.PlaceChamberR(50, 200);
            ChamberGlobal chamberL = ChamberMakerTest.PlaceChamberL(200, 200);
            chamberL.ConnectToNode(chamberL.GetExitNodeByIndex(0), chamberR.GetExitNodeByIndex(0));
            map.AddChamber(chamberL);
            map.AddChamber(chamberR);
            mapDrawer.DrawMap(map, dest);
        }

        public static void DrawTestFull(string dest)
        {
            MapDrawer mapDrawer = new MapDrawer();
            //GlobalMap map = new GlobalMap(1000, 1000);
            
            // List of allowed chambers
            ChamberGlobal c4 = ChamberMakerTest.ProtoChamberOrth(20, 0.9f, 0b1111);
            ChamberGlobal c4Big = ChamberMakerTest.ProtoChamberOrth(50, 0.9f, 0b1111);
            ChamberGlobal c3R = ChamberMakerTest.ProtoChamberOrth(20, 0.9f, 0b0111);
            ChamberGlobal c3L = ChamberMakerTest.ProtoChamberOrth(20, 0.9f, 0b1101);
            WeightedRandomPool<ChamberGlobal> pool = new WeightedRandomPool<ChamberGlobal>(new Dictionary<ChamberGlobal, float>()
            {
                {c4Big, 1},
                {c4, 2},
                {c3R, 1},
                {c3L, 1}
            });
            
            // Generate
            MapGenConfig config = new MapGenConfig(1000, 1000, pool, 50, 15);
            MapGenerator mapGen = new MapGenerator(config);
            GlobalMap map = mapGen.GenerateMap();
            
            // Draw
     
            mapDrawer.DrawMap(map, dest);
        }
    }
}