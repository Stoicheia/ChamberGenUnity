using System.Collections.Generic;

namespace ChamberGen
{
    public class GlobalMap : GlobalMapBase
    {
        public List<ChamberGlobal> Chambers => _chambers;
        public int Width { get; set; }
        public int Height { get; set; }
        private List<ChamberGlobal> _chambers;

        public GlobalMap(int width, int height)
        {
            _chambers = new List<ChamberGlobal>();
            Width = width;
            Height = height;
        }
        public override void AddChamber(ChamberGlobal chamber)
        {
            _chambers.Add(chamber);
        }
    }

    public abstract class GlobalMapBase
    {
        public abstract void AddChamber(ChamberGlobal chamber);
    }
}