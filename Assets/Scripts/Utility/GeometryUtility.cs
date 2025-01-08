using System;

namespace Utility
{
    public static class GeometryUtility
    {
        public static float GetCircleDistance(VectorInt pos1, float rad1, VectorInt pos2, float rad2)
        {
            float centerDistance = (pos1 - pos2).Magnitude();
            float circleDistance = centerDistance - rad1 - rad2;
            circleDistance = Math.Max(0, circleDistance);
            return circleDistance;
        }
    }
}