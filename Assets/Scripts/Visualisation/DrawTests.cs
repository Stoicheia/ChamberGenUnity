using ChamberGen;

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
    }
}