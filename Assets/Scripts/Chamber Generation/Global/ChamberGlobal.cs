using System.Collections.Generic;
using Utility;

namespace ChamberGen
{
    public struct ChamberGlobal
    {
        public int Radius { get; set; }
        public VectorInt? Position { get; set; }
        public bool IsPlaced => Position.HasValue;
        public List<ExitNodeGlobal> ExitNodes { get; set; }
    }
}