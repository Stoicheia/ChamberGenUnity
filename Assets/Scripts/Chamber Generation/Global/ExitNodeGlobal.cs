using System;
using System.Collections.Generic;

namespace ChamberGen
{
    public class ExitNodeGlobal
    {
        public float Distance01 { get; set; }
        public float AngleRad { get; set; }
        public float OutgoingPathAngle { get; set; }
        public ChamberGlobal ParentChamber { get; set; }
        public ChamberGlobal Connection { get; set; }

        public ExitNodeGlobal(float distance01, float angleRad, float outgoingPathAngle)
        {
            Distance01 = distance01;
            AngleRad = angleRad;
            OutgoingPathAngle = outgoingPathAngle;
            Connection = null;
        }

        public void ConnectTo(ChamberGlobal to)
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
    }
}