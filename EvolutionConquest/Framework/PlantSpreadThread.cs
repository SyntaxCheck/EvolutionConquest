using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class PlantSpreadThread
{
    private GameData _gameData;
    private Random _rand;
    private int GRID_CELL_SIZE;
    private int TickCount;
    private SpriteFont _foodFont;
    private const int MAX_PLANTS_PER_GRIDSPACE = 6;
    private const double MAX_PLANT_DENSITY = 0.4;

    public PlantSpreadThread(GameData sharedGameData, Random sharedRand, int sharedGridCellSize)
    {
        _gameData = sharedGameData;
        _rand = sharedRand;
        GRID_CELL_SIZE = sharedGridCellSize;
        _foodFont = _gameData.DebugFont;
        TickCount = 0;
    }

    public void Start()
    {
        CheckForSpreadLoop();
    }

    private void CheckForSpreadLoop()
    {
        while (true)
        {
            Thread.Sleep(10);
            try
            {
                if (_gameData.TickElapsedPlants)
                {
                    _gameData.TickElapsedPlants = false;
                    TickCount++;

                    lock (_gameData.LockPlants)
                    {
                        for (int i = _gameData.Plants.Count - 1; i >= 0; i--)
                        {
                            if (!_gameData.Plants[i].MarkForDelete)
                            {
                                _gameData.Plants[i].AdvanceTick(_rand);

                                if (_gameData.Plants[i].TicksSinceBirth >= _gameData.Plants[i].LifespanActual)
                                {
                                    _gameData.Plants[i].MarkForDelete = true;
                                }
                                else
                                {
                                    if (TickCount > 10)
                                    {
                                        //Check if plant can spread
                                        _gameData.Plants[i].CheckTexture();

                                        if (_gameData.Plants[i].SpreadPlant && _gameData.Plants[i].NumberOfSaplings > 0)
                                        {
                                            _gameData.Plants[i].SpreadPlant = false;
                                            List<Vector2> potentialSpawnLocations = new List<Vector2>();

                                            //Build list of potential spawn locations by checking the attachment points within the grid for plants occupying that space
                                            foreach (Vector2 attachPoints in _gameData.Plants[i].SaplingSpawnPoints)
                                            {
                                                bool spaceOpen = true;
                                                for (int k = 0; k < _gameData.Plants[i].ExpandedGridPositions.Count; k++)
                                                {
                                                    if (spaceOpen)
                                                    {
                                                        for (int j = 0; j < _gameData.MapGridData[_gameData.Plants[i].ExpandedGridPositions[k].X, _gameData.Plants[i].ExpandedGridPositions[k].Y].Plants.Count; j++)
                                                        {
                                                            if (_gameData.MapGridData[_gameData.Plants[i].ExpandedGridPositions[k].X, _gameData.Plants[i].ExpandedGridPositions[k].Y].Plants[j].Bounds.Contains(attachPoints))
                                                            {
                                                                spaceOpen = false;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }

                                                if (spaceOpen)
                                                {
                                                    potentialSpawnLocations.Add(attachPoints);
                                                }
                                            }

                                            //Filter the list based on Spawn direction preferences
                                            if (!_gameData.Plants[i].NoHorizontalSaplingPreference || !_gameData.Plants[i].NoVerticalSaplingPreference)
                                            {
                                                List<int> indexRemoveList = new List<int>();
                                                for (int k = 0; k < potentialSpawnLocations.Count; k++)
                                                {
                                                    bool isNorth = (_gameData.Plants[i].Position.Y < potentialSpawnLocations[k].Y);
                                                    bool isWest = (_gameData.Plants[i].Position.X > potentialSpawnLocations[k].X);

                                                    if (!_gameData.Plants[i].NoVerticalSaplingPreference)
                                                    {
                                                        if (_gameData.Plants[i].PreferSaplingNorth && !isNorth)
                                                        {
                                                            indexRemoveList.Add(k);
                                                        }
                                                        else if (!_gameData.Plants[i].PreferSaplingNorth && isNorth)
                                                        {
                                                            indexRemoveList.Add(k);
                                                        }
                                                    }
                                                    if (!_gameData.Plants[i].NoHorizontalSaplingPreference)
                                                    {
                                                        if (_gameData.Plants[i].PreferSaplingWest && !isWest)
                                                        {
                                                            indexRemoveList.Add(k);
                                                        }
                                                        else if (!_gameData.Plants[i].PreferSaplingWest && isWest)
                                                        {
                                                            indexRemoveList.Add(k);
                                                        }
                                                    }
                                                }

                                                //Get rid of duplicates
                                                indexRemoveList = indexRemoveList.Distinct().ToList();
                                                //Do not delete more spawn locations than we have and make sure to have enough spawn loactions for all the saplings
                                                if (indexRemoveList.Count < (potentialSpawnLocations.Count - _gameData.Plants[i].NumberOfSaplings))
                                                {
                                                    for (int k = potentialSpawnLocations.Count - 1; k >= 0; k--)
                                                    {
                                                        foreach (int irl in indexRemoveList)
                                                        {
                                                            if (k == irl)
                                                            {
                                                                potentialSpawnLocations.RemoveAt(k);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            //Pick random spawns from the spawnlocation list
                                            for (int k = 0; k < _gameData.Plants[i].NumberOfSaplings; k++)
                                            {
                                                if (potentialSpawnLocations.Count > 0)
                                                {
                                                    int chosenIndex = _rand.Next(0, potentialSpawnLocations.Count); //Select an index randomly
                                                    Vector2 chosenLoc = potentialSpawnLocations[chosenIndex];
                                                    potentialSpawnLocations.RemoveAt(chosenIndex); //Remove that index from the list so we do not spawn multiple plants in the same spot

                                                    Plant sapling = _gameData.Plants[i].GetBabyPlant(_rand);

                                                    //Randomize the location making sure not to move the plant out of bounds
                                                    chosenLoc.X = chosenLoc.X + _rand.Next(sapling.Texture.Width / 3 * -1, sapling.Texture.Width / 3);
                                                    chosenLoc.Y = chosenLoc.Y + _rand.Next(sapling.Texture.Height / 3 * -1, sapling.Texture.Height / 3);

                                                    if (chosenLoc.X <= sapling.Texture.Width / 2)
                                                    {
                                                        chosenLoc.X = sapling.Texture.Width / 2;
                                                    }
                                                    if (chosenLoc.X >= _gameData.Settings.WorldSize - (sapling.Texture.Width / 2))
                                                    {
                                                        chosenLoc.X = _gameData.Settings.WorldSize - (sapling.Texture.Width / 2);
                                                    }
                                                    if (chosenLoc.Y <= sapling.Texture.Height / 2)
                                                    {
                                                        chosenLoc.Y = sapling.Texture.Height / 2;
                                                    }
                                                    if (chosenLoc.Y >= _gameData.Settings.WorldSize - (sapling.Texture.Height / 2))
                                                    {
                                                        chosenLoc.Y = _gameData.Settings.WorldSize - (sapling.Texture.Height / 2);
                                                    }

                                                    sapling.Position = chosenLoc;
                                                    sapling.GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);
                                                    sapling.GetExpandedGridPositions(_gameData); //Calculate expanded grid positions after the position has been set
                                                    sapling.DisplayText = sapling.FoodAmount.ToString() + "/" + sapling.FoodAmountCap;
                                                    sapling.TextSize = _foodFont.MeasureString(sapling.DisplayText);

                                                    if (_gameData.CurrentMaxFoodLevel > sapling.FoodStrength)
                                                    {
                                                        sapling.FoodStrength = _gameData.CurrentMaxFoodLevel;
                                                    }

                                                    _gameData.Plants.Add(sapling);
                                                    _gameData.AddPlantToGrid(sapling);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (TickCount > 10)
                    {
                        TickCount = 0;
                    }
                }
                else
                {
                    //Validate that a plant is allowed to spread. Check The surrounding grid to see if there are too many plants already. This simulates running out of space for sunlight and water
                    try
                    {
                        for (int i = _gameData.Plants.Count - 1; i >= 0; i--)
                        {
                            _gameData.Plants[i].AllowedToSpread = false;

                            foreach (Point p in _gameData.Plants[i].GridPositions)
                            {
                                Point topLeft = new Point(p.X - _gameData.Plants[i].PlantSpreadWestCheckRadius, p.Y - _gameData.Plants[i].PlantSpreadNorthCheckRadius);
                                Point bottomRight = new Point(p.X + _gameData.Plants[i].PlantSpreadEastCheckRadius, p.Y + _gameData.Plants[i].PlantSpreadSouthCheckRadius);

                                if (topLeft.X < 0) topLeft.X = 0;
                                if (topLeft.X > _gameData.MapGridData.GetLength(0)) topLeft.X = _gameData.MapGridData.GetLength(0);
                                if (topLeft.Y < 0) topLeft.Y = 0;
                                if (topLeft.Y > _gameData.MapGridData.GetLength(1)) topLeft.Y = _gameData.MapGridData.GetLength(1);

                                if (bottomRight.X < 0) bottomRight.X = 0;
                                if (bottomRight.X > _gameData.MapGridData.GetLength(0)) bottomRight.X = _gameData.MapGridData.GetLength(0);
                                if (bottomRight.Y < 0) bottomRight.Y = 0;
                                if (bottomRight.Y > _gameData.MapGridData.GetLength(1)) bottomRight.Y = _gameData.MapGridData.GetLength(1);

                                int plantCount = 0;
                                int plantCountNorth = 0;
                                int plantCountSouth = 0;
                                int plantCountWest = 0;
                                int plantCountEast = 0;
                                int gridCount = 0;
                                int gridCountNorth = 0;
                                int gridCountSouth = 0;
                                int gridCountWest = 0;
                                int gridCountEast = 0;
                                for (int j = topLeft.X; j < bottomRight.X; j++)
                                {
                                    for (int k = topLeft.Y; k < bottomRight.Y; k++)
                                    {
                                        int tmpCount = _gameData.MapGridData[j, k].Plants.Where(t => !t.MarkForDelete).Count();
                                        plantCount += tmpCount;
                                        gridCount++;

                                        if (k > p.Y)
                                        {
                                            plantCountNorth += tmpCount;
                                            gridCountNorth++;
                                        }
                                        else
                                        {
                                            plantCountSouth += tmpCount;
                                            gridCountSouth++;
                                        }
                                        if (j > p.X)
                                        {
                                            plantCountEast += tmpCount;
                                            gridCountEast++;
                                        }
                                        else
                                        {
                                            plantCountWest += tmpCount;
                                            gridCountWest++;
                                        }
                                    }
                                }

                                //int maxGridCount = (MAX_PLANT_GRIDRADIUS * 2) * (MAX_PLANT_GRIDRADIUS * 2);
                                //int projectedMaxPlants = maxGridCount * MAX_PLANTS_PER_GRIDSPACE;
                                int projectedMaxPlants = gridCount * MAX_PLANTS_PER_GRIDSPACE;
                                double plantDensity = (double)plantCount / (double)projectedMaxPlants;

                                if (plantDensity < MAX_PLANT_DENSITY)
                                {
                                    _gameData.Plants[i].AllowedToSpread = true;

                                    double northRatio = (double)plantCountNorth / (double)gridCountNorth;
                                    double southRatio = (double)plantCountSouth / (double)gridCountSouth;
                                    double eastRatio = (double)plantCountEast / (double)gridCountEast;
                                    double westRatio = (double)plantCountWest / (double)gridCountWest;

                                    if (northRatio == southRatio)
                                    {
                                        _gameData.Plants[i].NoVerticalSaplingPreference = true;
                                        _gameData.Plants[i].PreferSaplingNorth = false;
                                    }
                                    else if (northRatio > southRatio)
                                    {
                                        _gameData.Plants[i].NoVerticalSaplingPreference = false;
                                        _gameData.Plants[i].PreferSaplingNorth = false;
                                    }
                                    else
                                    {
                                        _gameData.Plants[i].NoVerticalSaplingPreference = false;
                                        _gameData.Plants[i].PreferSaplingNorth = true;
                                    }

                                    if (eastRatio == westRatio)
                                    {
                                        _gameData.Plants[i].NoHorizontalSaplingPreference = true;
                                        _gameData.Plants[i].PreferSaplingWest = false;
                                    }
                                    else if (westRatio > eastRatio)
                                    {
                                        _gameData.Plants[i].NoHorizontalSaplingPreference = false;
                                        _gameData.Plants[i].PreferSaplingWest = false;
                                    }
                                    else
                                    {
                                        _gameData.Plants[i].NoHorizontalSaplingPreference = false;
                                        _gameData.Plants[i].PreferSaplingWest = true;
                                    }

                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception) { } //Ignore any errors here, not critical code
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(_gameData.SessionID.ToString(), "ErrorLogPlantSpreadThread.txt"), DateTime.Now.ToString() + " - PLANTS Spread Thread Uncaught error: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
            }
        }
    }
}