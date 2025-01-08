using System;
using System.Collections.Generic;
using System.Drawing;
using Utility;

namespace ChamberGen
{
    public class MapGenerator
    {
        private VectorInt _mapDimensions;
        private int _minDistance;
        private int _k;

        public MapGenerator(int mapWidth, int mapHeight, int minDistance, int k)
        {
            _mapDimensions = new VectorInt(mapWidth, mapHeight);
        }
        
        public GlobalMap GenerateMap()
        {
            int mapWidth = _mapDimensions.x;
            int mapHeight = _mapDimensions.y;
            
            ChamberGrid grid = new ChamberGrid();
            
        }

        
        struct ChamberGrid
        {
            private List<ChamberGlobal>[,] _chamberGrid;
            private List<ChamberGlobal> _allChambers;
            private int _gridSize;
            
            public ChamberGrid(int gridW, int gridH, int gridSize)
            {
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
                return Place(GetGridIntersections(chamber, pos), chamber, pos);
            }

            public HashSet<VectorInt> Place(HashSet<VectorInt> gridIntersections, ChamberGlobal chamber, VectorInt pos)
            {
                foreach (VectorInt gridCoord in gridIntersections)
                {
                    _chamberGrid[gridCoord.x, gridCoord.y].Add(chamber);
                }
                _allChambers.Add(chamber);
                chamber.Position = pos;
                return gridIntersections;
            }
            
            public bool CanPlace(ChamberGlobal chamber, VectorInt pos, float minDistance)
            {
                HashSet<VectorInt> chamberGridOverlaps = GetGridIntersections(chamber, pos);
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
                    foreach (ChamberGlobal chamber in _chamberGrid[coord.x, coord.y])
                    {
                        nearbyChambers.Add(chamber);
                    }
                }
                return nearbyChambers;
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
            public HashSet<VectorInt> GetGridIntersections(ChamberGlobal chamber, VectorInt pos)
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
                return coord * _gridSize + new VectorInt(_gridSize/2, _gridSize/2);
            }
        }
    }
}