using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace ChamberGen
{
    public class ChamberGlobal
    {
        public int Radius { get; set; }
        public int X => Position?.x ?? int.MinValue;
        public int Y => Position?.y ?? int.MinValue;
        public VectorInt? Position { get; set; }
        public bool IsPlaced => Position.HasValue;
        public List<ExitNodeGlobal> ExitNodes { get; set; }

        public ChamberGlobal(int radius)
        {
            Radius = radius;
        }

        public void SetExitNodes(List<ExitNodeGlobal> exitNodes)
        {
            if (!IsPlaced)
            {
                ExitNodes = exitNodes;
            }
            else
            {
                ExitNodes = exitNodes.Select(x => x.Instantiate(parentChamber: this)).ToList();
            }

            for (int i = 0; i < exitNodes.Count; i++)
            {
                exitNodes[i].IndexOf = i;
            }
        }

        // prototype design pattern (similar to Unity prefabs)
        public ChamberGlobal Instantiate(VectorInt position)
        {
            ChamberGlobal instance = new ChamberGlobal(Radius);
            instance.Position = position;
            instance.Radius = Radius;
            instance.ExitNodes = ExitNodes.Select(x => x.Instantiate(parentChamber: instance)).ToList();
            for (int i = 0; i < instance.ExitNodes.Count; i++)
            {
                instance.ExitNodes[i].IndexOf = i;
            }
            return instance;
        }

        // Iteration over chambers will double-count lines, but that's okay for visualisation
        public List<LineInt> GetOutgoingLines()
        {
            List<LineInt> lines = new List<LineInt>();
            foreach (ExitNodeGlobal node in ExitNodes)
            {
                if(!node.HasConnection) continue;
                VectorInt thisNodePos = node.GetExitNodePosOnMap();
                VectorInt otherNodePos = node.Connection.GetExitNodePosOnMap();
                LineInt line = new LineInt(thisNodePos, otherNodePos);
                lines.Add(line);
            }

            return lines;
        }

        public bool TryConnectToNode(int index, ChamberGlobal toChamber)
        {
            ExitNodeGlobal thisExitNode = ExitNodes[index];
            float exitNodeAngle = thisExitNode.OutgoingPathAngleDegrees;
            var otherExitNodeIndex = toChamber.FindIndexOfAngle(exitNodeAngle);
            if (otherExitNodeIndex.HasValue)
            {
                ExitNodeGlobal otherExitNode = toChamber.ExitNodes[otherExitNodeIndex.Value];
                otherExitNode.SetConnection(thisExitNode);
                thisExitNode.SetConnection(otherExitNode);
                return true;
            }

            return false;
        }

        public bool ConnectToNode(ExitNodeGlobal node, ExitNodeGlobal other)
        {
            if (!other.ParentChamber.IsPlaced)
            {
                throw new Exception("Can only connect nodes of placed chambers");
            }
            if (node.ParentChamber != this) return false;
            node.SetConnection(other);
            other.SetConnection(node);
            return true;
        }
        
        public bool TryConnectToNode(ExitNodeGlobal node, ChamberGlobal toChamber)
        {
            if (node.ParentChamber != this) return false;
            float exitNodeAngle = node.OutgoingPathAngleDegrees;
            var otherExitNodeIndex = toChamber.FindIndexOfAngle(exitNodeAngle);
            if (otherExitNodeIndex.HasValue)
            {
                ExitNodeGlobal otherExitNode = toChamber.ExitNodes[otherExitNodeIndex.Value];
                otherExitNode.SetConnection(node);
                node.SetConnection(otherExitNode);
                return true;
            }

            return false;
        }

        public ExitNodeGlobal GetRandomExitNode(Random random)
        {
            return ExitNodes[random.Next(ExitNodes.Count)];
        }

        public ExitNodeGlobal GetExitNode(int angle)
        {
            return ExitNodes.FirstOrDefault(x => x.OutgoingPathAngleDegrees == angle);
        }

        public ExitNodeGlobal GetExitNodeByIndex(int index)
        {
            return ExitNodes[index];
        }

        public VectorInt GetExitNodePosRelative(ExitNodeGlobal exitNode)
        {
            VectorFloat inCircle01 = exitNode.Distance01 * new VectorFloat(Math.Cos(exitNode.AngleRad), Math.Sin(exitNode.AngleRad));
            VectorInt relPos = (inCircle01 * Radius).ToVectorInt();
            return relPos;
        }
        
        public VectorInt GetExitNodePosOnMap(ExitNodeGlobal exitNode)
        {
            VectorFloat inCircle01 = exitNode.Distance01 * new VectorFloat(Math.Cos(exitNode.AngleRad), Math.Sin(exitNode.AngleRad));
            VectorInt relPos = (inCircle01 * Radius).ToVectorInt();
            return Position.Value + relPos;
        }
        
        private int? FindIndexOfAngle(float angleTo)
        {
            for (int i = 0; i < ExitNodes.Count; i++)
            {
                ExitNodeGlobal node = ExitNodes[i];
                float angle = node.OutgoingPathAngleDegrees;
                if (Math.Abs(angleTo - angle) < 0.001f)
                {
                    return i;
                }
            }

            return null;
        }

        public bool ContainsAngle(float angle)
        {
            return ExitNodes.Any(x => x.OutgoingPathAngleDegrees == angle);
        }
    }
}