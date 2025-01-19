namespace Utility
{
    public struct LineInt
    {
        public VectorInt From;
        public VectorInt To;
        public int X1 => From.x;
        public int Y1 => From.y;
        public int X2 => To.x;
        public int Y2 => To.y;

        public LineInt(VectorInt from, VectorInt to)
        {
            From = from;
            To = to;
        }
    }
}