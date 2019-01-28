﻿using Microsoft.Xna.Framework;
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
    private const int MAX_PLANTS_PER_GRIDSPACE = 4;
    private const double MAX_PLANT_DENSITY = 0.6;

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
                            foreach(Point p in _gameData.Plants[i].GridPositions)
                            {
                                Point topLeft = new Point(p.X - 5, p.Y - 5);
                                Point bottomRight = new Point(p.X + 5, p.Y + 5);

                                if (topLeft.X < 0) topLeft.X = 0;
                                if (topLeft.X > _gameData.MapGridData.GetLength(0)) topLeft.X = _gameData.MapGridData.GetLength(0);
                                if (topLeft.Y < 0) topLeft.Y = 0;
                                if (topLeft.Y > _gameData.MapGridData.GetLength(1)) topLeft.Y = _gameData.MapGridData.GetLength(1);

                                if (bottomRight.X < 0) bottomRight.X = 0;
                                if (bottomRight.X > _gameData.MapGridData.GetLength(0)) bottomRight.X = _gameData.MapGridData.GetLength(0);
                                if (bottomRight.Y < 0) bottomRight.Y = 0;
                                if (bottomRight.Y > _gameData.MapGridData.GetLength(1)) bottomRight.Y = _gameData.MapGridData.GetLength(1);

                                int plantCount = 0;
                                int gridCount = 0;
                                for (int j = topLeft.X; j < bottomRight.X; j++)
                                {
                                    for (int k = topLeft.Y; k < bottomRight.Y; k++)
                                    {
                                        plantCount += _gameData.MapGridData[j, k].Plants.Where(t => !t.MarkForDelete).Count();
                                        gridCount++;
                                    }
                                }

                                int projectedMaxPlants = gridCount * MAX_PLANTS_PER_GRIDSPACE;
                                double plantDensity = plantCount / projectedMaxPlants;

                                if (plantDensity > MAX_PLANT_DENSITY)
                                {
                                    _gameData.Plants[i].AllowedToSpread = false;
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