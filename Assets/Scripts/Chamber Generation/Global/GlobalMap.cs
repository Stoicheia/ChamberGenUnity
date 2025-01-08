using System.Collections.Generic;

namespace ChamberGen
{
    public class GlobalMap : GlobalMapBase
    {
        private List<ChamberGlobal> _chambers;

        public GlobalMap()
        {
            _chambers = new List<ChamberGlobal>();
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