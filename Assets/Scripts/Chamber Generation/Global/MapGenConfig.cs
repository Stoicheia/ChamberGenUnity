using System;
using System.Collections.Generic;
using Utility;

namespace ChamberGen
{
    public class MapGenConfig
    {
        public int Width => _width;
        public int Height => _height;
        public int MinDistance => _minDistance;
        public int MaxPlacementTries => _maxPlacementTries;
        public Random Random => _rng;
        
        private WeightedRandomPool<ChamberGlobal> _chamberPool;
        private int _width;
        private int _height;
        private int _minDistance;
        private int _maxPlacementTries;
        private Random _rng;
        
        public MapGenConfig(int w, int h, WeightedRandomPool<ChamberGlobal> chamberPool, int minDistance, int maxPlacementTries)
        {
            _width = w;
            _height = h;
            _chamberPool = chamberPool;
            _minDistance = minDistance;
            _maxPlacementTries = maxPlacementTries;
            _rng = new Random();
        }

        public ChamberGlobal GetRandomChamber()
        {
            return _chamberPool.GetRandom();
        }

        public ExitNodeGlobal GetRandomExitNode(float angle, Random random)
        {
            List<ChamberGlobal> allChambers = GetAllChambers();
            List<ChamberGlobal> validChambers = new List<ChamberGlobal>();
            foreach (var chamber in allChambers)
            {
                if (chamber.ContainsAngle(angle))
                {
                    validChambers.Add(chamber);
                }
            }
            return _chamberPool.GetRandomInSubset(random, validChambers).GetExitNode(angle);
        }

        public List<ChamberGlobal> GetAllChambers()
        {
            return _chamberPool.GetAll();
        }
    }
}