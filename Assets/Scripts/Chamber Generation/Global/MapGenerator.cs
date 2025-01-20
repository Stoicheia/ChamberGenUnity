using System;
using System.Collections.Generic;
using System.Drawing;
using Utility;

namespace ChamberGen
{
    public class MapGenerator
    {
        private VectorInt _mapDimensions;
        private MapGenConfig _config;
        private Random _random;
        
        private List<ChamberGlobal> _chamberList;
        private Dictionary<float, List<ExitNodeGlobal>> _angleToPotentialExitNodes;
        
        public MapGenerator(MapGenConfig mapGenConfig)
        {
            _config = mapGenConfig;
            _mapDimensions = new VectorInt(_config.Width, _config.Height);
            _random = mapGenConfig.Random;
        }

        public void Init()
        {
            _angleToPotentialExitNodes = new Dictionary<float, List<ExitNodeGlobal>>();
            _chamberList = _config.GetAllChambers();
            foreach (ChamberGlobal chamber in _chamberList)
            {
                foreach(ExitNodeGlobal node in chamber.ExitNodes)
                {
                    node.AssignTo(chamber);
                    if(!_angleToPotentialExitNodes.ContainsKey(node.OutgoingPathAngleDegrees))
                        _angleToPotentialExitNodes[node.OutgoingPathAngleDegrees] = new List<ExitNodeGlobal>();
                    _angleToPotentialExitNodes[node.OutgoingPathAngleDegrees].Add(node);
                }
            }
        }
        
        public GlobalMap GenerateMap()
        {
            GlobalMap map = new GlobalMap(_mapDimensions.x, _mapDimensions.y);
            Init();
            
            // 1. Make the grid, slightly overestimating size
            int mapWidth = _mapDimensions.x;
            int mapHeight = _mapDimensions.y;
            float gridSize = _config.MinDistance / (float)Math.Sqrt(2);
            float gridWidthUnrounded = mapWidth / gridSize;
            float gridHeightUnrounded = mapHeight / gridSize;
            int gridWidth = (int)Math.Ceiling(gridWidthUnrounded);
            int gridHeight = (int)Math.Ceiling(gridHeightUnrounded);
            ChamberGrid grid = new ChamberGrid(gridWidth, gridHeight, gridSize, _mapDimensions);
            
            // 2. Place the first chamber
            ChamberGlobal firstChamber = _config.GetRandomChamber();
            VectorInt topLeft = new VectorInt(firstChamber.Radius, firstChamber.Radius);
            VectorInt botRight = new VectorInt(mapWidth - firstChamber.Radius, mapHeight - firstChamber.Radius);
            VectorInt randomPos = VectorInt.RandomInRect(topLeft, botRight);
            HashSet<VectorInt> cachedGridOverlaps = new HashSet<VectorInt>();
            List<ExitNodeGlobal> active = new List<ExitNodeGlobal>(); 
            
            // 2.1. Add chamber (and all its exit nodes) to active list helper
            void AddChamber(ChamberGlobal chamber, VectorInt pos)
            {
                ChamberGlobal chamberInstance = chamber.Instantiate(pos);
                int exitNodeCount = chamberInstance.ExitNodes.Count;
                for (int i = 0; i < exitNodeCount; i++)
                {
                    active.Add(chamberInstance.GetExitNodeByIndex(i));
                }
                map.AddChamber(chamberInstance);
            }
            
            // 2.2. practice using the ChamberGrid class (canPlace should always return true
            {
                bool canPlace = grid.CanPlace(firstChamber, randomPos, _config.MinDistance, out cachedGridOverlaps);
                if (canPlace)
                {
                    grid.Place(cachedGridOverlaps, firstChamber, randomPos);
                    AddChamber(firstChamber, randomPos);
                }
                else
                {
                    throw new Exception("The first chamber could not be placed for some reason.");
                }
            }

            // 3. The loop
            while (active.Count > 0)
            {
                int randomIndex = _random.Next(0, active.Count);
                ChamberGlobal activeChamber = active[randomIndex].ParentChamber;
                ExitNodeGlobal activeNode = activeChamber.GetRandomExitNode(_random);

                bool found = false;
                for (int tries = 0; tries < _config.MaxPlacementTries; tries++)
                {
                    float radius = _random.Next(_config.MinDistance, 2 * _config.MinDistance);
                    
                    int theta = activeNode.OutgoingPathAngleDegrees;
                    float thetaRad = activeNode.OutgoingPathAngleDegrees * (float)Math.PI / 180;
                    int oppAngle = GeometryUtility.GetOppositeAngle(theta);
                    ExitNodeGlobal oppositeNode = _config.GetRandomExitNode(oppAngle, _random);
                    if (oppositeNode == null) continue;
                    ChamberGlobal oppositeChamber = oppositeNode.ParentChamber;
                    VectorInt activeMapPos = activeNode.GetExitNodePosOnMap();
                    VectorInt oppositeRelPos = oppositeNode.GetExitNodePosRelative();
                    VectorInt offset = VectorFloat.Polar(radius, thetaRad).ToVectorInt();
                        
                    VectorInt oppositeChamberPos = activeMapPos + offset - oppositeRelPos;
                    bool canPlace = grid.CanPlace(oppositeChamber, oppositeChamberPos, _config.MinDistance, out cachedGridOverlaps);
                    if (canPlace)
                    {
                        ChamberGlobal newChamberInstance = oppositeChamber.Instantiate(oppositeChamberPos);
                        ExitNodeGlobal oppNodeInstance = newChamberInstance.GetExitNodeByIndex(oppositeNode.IndexOf);

                        grid.Place(newChamberInstance, oppositeChamberPos);
                        activeChamber.ConnectToNode(activeNode, oppNodeInstance);
                        AddChamber(newChamberInstance, oppositeChamberPos);
                        found = true;
                        break;
                    }
                }
                
                if (!found)
                {
                    active.Remove(activeNode);
                }
            }

            return map;
        }
        
        struct ChamberGrid
        {
            private int _width;
            private int _height;
            private List<ChamberGlobal>[,] _chamberGrid;
            private List<ChamberGlobal> _allChambers;
            private float _gridSize;
            private VectorInt _mapDimensions;
            
            public ChamberGrid(int gridW, int gridH, float gridSize, VectorInt mapDimensions)
            {
                _width = gridW;
                _height = gridH;
                _mapDimensions = mapDimensions;
                _chamberGrid = new List<ChamberGlobal>[gridW, gridH];
                for (int x = 0; x < gridW; x++)
                {
                    for (int y = 0; y < gridH; y++)
                    {
                        _chamberGrid[x, y] = new List<ChamberGlobal>();
                    }
                }
                _gridSize = gridSize;
                _allChambers = new List<ChamberGlobal>();
            }

            public HashSet<VectorInt> Place(ChamberGlobal chamber, VectorInt pos)
            {
                return Place(GetGridOverlaps(chamber, pos), chamber, pos);
            }

            public HashSet<VectorInt> Place(HashSet<VectorInt> gridIntersections, ChamberGlobal chamber, VectorInt pos)
            {
                foreach (VectorInt gridCoord in gridIntersections)
                {
                    TryAssignToGrid(gridCoord, chamber);
                }
                _allChambers.Add(chamber);
                chamber.Position = pos;
                return gridIntersections;
            }

            private bool TryAssignToGrid(VectorInt gridCoord, ChamberGlobal chamber)
            {
                int x = gridCoord.x;
                int y = gridCoord.y;
                if (x < 0 || x >= _width || y < 0 || y >= _height)
                {
                    return false;
                }
                _chamberGrid[gridCoord.x, gridCoord.y].Add(chamber);
                return true;
            }
            
            public bool CanPlace(ChamberGlobal chamber, VectorInt pos, float minDistance, out HashSet<VectorInt> gridIntersections)
            {
                if (pos.x < chamber.Radius || pos.y < chamber.Radius || pos.x >= _mapDimensions.x - chamber.Radius || pos.y >= _mapDimensions.y - chamber.Radius)
                {
                    gridIntersections = new HashSet<VectorInt>();
                    return false;
                }
                
                HashSet<VectorInt> chamberGridOverlaps = GetGridOverlaps(chamber, pos);
                gridIntersections = chamberGridOverlaps;
                HashSet<ChamberGlobal> nearbyChambers = GetNearbyChambers(chamberGridOverlaps);
                foreach (ChamberGlobal nearby in nearbyChambers)
                {
                    if (!nearby.Position.HasValue)
                    {
                        throw new Exception("A chamber in the grid doesn't have an initialized position.");
                    }
                    float distanceTo = GeometryUtility.GetCircleDistance(nearby.Position.Value, nearby.Radius, pos, chamber.Radius);
                    if (distanceTo < minDistance) return false;
                }

                return true;
            }
            
            private HashSet<ChamberGlobal> GetNearbyChambers(HashSet<VectorInt> centralMass)
            {
                HashSet<VectorInt> nearbyCoords = GetNearbyGridCoords(centralMass);
                HashSet<ChamberGlobal> nearbyChambers = new HashSet<ChamberGlobal>();
                foreach (VectorInt coord in nearbyCoords)
                {
                    foreach (ChamberGlobal chamber in GetChambersAtCoord(coord))
                    {
                        nearbyChambers.Add(chamber);
                    }
                }
                return nearbyChambers;
            }

            private List<ChamberGlobal> GetChambersAtCoord(VectorInt coord)
            {
                if (coord.x < 0 || coord.y < 0 || coord.x >= _width || coord.y >= _height)
                {
                    return new List<ChamberGlobal>();
                }
                return _chamberGrid[coord.x, coord.y];
            }

            private HashSet<VectorInt> GetNearbyGridCoords(HashSet<VectorInt> gridCoords)
            {
                HashSet<VectorInt> nearbyCoords = new HashSet<VectorInt>();
                foreach (VectorInt centralMass in gridCoords)
                {
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            if (Math.Abs(x * y) == 4) continue;
                            nearbyCoords.Add(centralMass + new VectorInt(x, y)); // hashset dakara, duplicates nashi
                        }
                    }
                }

                return nearbyCoords;
            }


            // this will overestimate, but this is fine
            public HashSet<VectorInt> GetGridOverlaps(ChamberGlobal chamber, VectorInt pos)
            {
                HashSet<VectorInt> gridCoords = new HashSet<VectorInt>();
                int radius = chamber.Radius;
                Rectangle boundingBox = new Rectangle(pos.x - chamber.Radius, pos.y - chamber.Radius, radius*2, radius*2);
                
                int top = boundingBox.Top;
                int left = boundingBox.Left;
                int right = boundingBox.Right;
                int bottom = boundingBox.Bottom;
                
                VectorInt topLeft = new VectorInt(left, top);
                VectorInt topRight = new VectorInt(right, top);
                VectorInt bottomLeft = new VectorInt(left, bottom);
                VectorInt bottomRight = new VectorInt(right, bottom);
                
                VectorInt topLeftGrid = Pos2Coord(topLeft);
                VectorInt topRightGrid = Pos2Coord(topRight);
                VectorInt bottomLeftGrid = Pos2Coord(bottomLeft);
                VectorInt bottomRightGrid = Pos2Coord(bottomRight);

                for (int x = topLeftGrid.x; x <= topRightGrid.x; x++)
                {
                    for (int y = topLeftGrid.y; y <= bottomLeftGrid.y; y++)
                    {
                        gridCoords.Add(new VectorInt(x, y));
                    }
                }

                return gridCoords;
            }

            private VectorInt Pos2Coord(VectorInt pos)
            {
                return pos / _gridSize;
            }

            private VectorInt Coord2PosTopLeft(VectorInt coord)
            {
                return coord * _gridSize;
            }

            private VectorInt Coord2PosCenter(VectorInt coord)
            {
                return coord * _gridSize + new VectorInt((int)_gridSize/2, (int)_gridSize/2);
            }
        }
        
        
    }
}