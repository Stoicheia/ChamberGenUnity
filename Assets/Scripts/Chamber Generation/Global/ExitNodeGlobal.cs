using System.Collections.Generic;

namespace ChamberGen
{
    public struct ExitNodeGlobal
    {
        public float Distance01 { get; set; }
        public float AngleRad { get; set; }
        public List<float> OutgoingPathAngles { get; set; }

        public ExitNodeGlobal(float distance01, float angleRad, List<float> outgoingPaths)
        {
            Distance01 = distance01;
            AngleRad = angleRad;
            OutgoingPathAngles = outgoingPaths;
        }
    }
}