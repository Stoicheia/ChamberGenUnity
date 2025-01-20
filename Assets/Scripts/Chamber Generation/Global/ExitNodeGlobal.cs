using System;
using System.Collections.Generic;
using Utility;

namespace ChamberGen
{
    public class ExitNodeGlobal
    {
        public int IndexOf { get; set; }
        public float Distance01 { get; set; }
        public float AngleRad { get; set; }
        public int OutgoingPathAngleDegrees { get; set; }
        public ChamberGlobal ParentChamber { get; set; }
        public ExitNodeGlobal Connection { get; set; }
        public bool HasConnection => Connection != null;

        public ExitNodeGlobal(float distance01, float angleRad, int outgoingPathAngleDegrees)
        {
            Distance01 = distance01;
            AngleRad = angleRad;
            OutgoingPathAngleDegrees = outgoingPathAngleDegrees;
            Connection = null;
        }

        public ExitNodeGlobal Instantiate(ChamberGlobal parentChamber)
        {
            ExitNodeGlobal instance = new ExitNodeGlobal(Distance01, AngleRad, OutgoingPathAngleDegrees);
            instance.ParentChamber = parentChamber;
            return instance;
        }

        public void SetConnection(ExitNodeGlobal to)
        {
            Connection = to;
        }

        public void Clear()
        {
            Connection = null;
        }

        public void AssignTo(ChamberGlobal parent)
        {
            ParentChamber = parent;
        }
        
        public VectorInt GetExitNodePosRelative()
        {
            return ParentChamber.GetExitNodePosRelative(this);
        }
        
        public VectorInt GetExitNodePosOnMap()
        {
            return ParentChamber.GetExitNodePosOnMap(this);
        }

    }
}