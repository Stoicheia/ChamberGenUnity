using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace ChamberGen
{
    public class ChamberGlobal
    {
        public int Radius { get; set; }
        public VectorInt? Position { get; set; }
        public bool IsPlaced => Position.HasValue;
        public List<ExitNodeGlobal> ExitNodes { get; set; }

        // google prototype design pattern (similar to Unity prefabs)
        public ChamberGlobal Instantiate(VectorInt position)
        {
            Position = position;
            ChamberGlobal instance = new ChamberGlobal();
            instance.Position = position;
            instance.ExitNodes = ExitNodes;
            instance.ExitNodes.ForEach(x =>
            {
                x.Clear();
                x.AssignTo(this);
            });
            instance.Radius = Radius;
            return instance;
        }

        public bool TryConnectToNode(int index, ChamberGlobal toChamber)
        {
            ExitNodeGlobal thisExitNode = ExitNodes[index];
            float exitNodeAngle = thisExitNode.OutgoingPathAngle;
            var otherExitNodeIndex = toChamber.FindIndexOfAngle(exitNodeAngle);
            if (otherExitNodeIndex.HasValue)
            {
                ExitNodeGlobal otherExitNode = toChamber.ExitNodes[otherExitNodeIndex.Value];
                otherExitNode.ConnectTo(this);
                thisExitNode.ConnectTo(toChamber);
                return true;
            }

            return false;
        }

        public ExitNodeGlobal GetRandomExitNode(Random random)
        {
            return ExitNodes[random.Next(ExitNodes.Count)];
        }

        public ExitNodeGlobal GetExitNode(float angle)
        {
            return ExitNodes.FirstOrDefault(x => x.OutgoingPathAngle == angle);
        }
        
        private int? FindIndexOfAngle(float angleTo)
        {
            float angleFrom = -angleTo;

            for (int i = 0; i < ExitNodes.Count; i++)
            {
                ExitNodeGlobal node = ExitNodes[i];
                float angle = node.OutgoingPathAngle;
                if (Math.Abs(angleFrom - angle) < 0.001f)
                {
                    return i;
                }
            }

            return null;
        }

        public bool ContainsAngle(float angle)
        {
            return ExitNodes.Any(x => x.OutgoingPathAngle == angle);
        }
    }
}