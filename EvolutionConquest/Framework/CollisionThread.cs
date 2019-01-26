using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class CollisionThread
{
    private GameData _gameData;
    private Random _rand;

    public CollisionThread(GameData sharedGameData, Random shareRand)
    {
        _gameData = sharedGameData;
        _rand = shareRand;
    }

    public void Start()
    {
        CheckForCollisions();
    }

    public void CheckForCollisions()
    {
        while (true)
        {
            try
            {
                _gameData.LockCreatures.Locker = "CollisionThread";
                lock (_gameData.LockCreatures)
                {
                    for (int i = 0; i < _gameData.Creatures.Count; i++)
                    {
                        if (_gameData.Creatures[i].IsAlive)
                        {
                            //Food collision
                            if ((_gameData.Creatures[i].IsHerbavore || _gameData.Creatures[i].IsScavenger) && _gameData.Creatures[i].UndigestedFood < _gameData.MaxCreatureUndigestedFood)
                            {
                                foreach (Point p in _gameData.Creatures[i].GridPositions)
                                {
                                    lock (_gameData.LockFood)
                                    {
                                        for (int k = (_gameData.MapGridData[p.X, p.Y].Food.Count - 1); k >= 0; k--)
                                        {
                                            if (!_gameData.MapGridData[p.X, p.Y].Food[k].MarkForDelete)
                                            {
                                                if (_gameData.Creatures[i].FoodTypeInt == _gameData.MapGridData[p.X, p.Y].Food[k].FoodType && (_gameData.Creatures[i].IsScavenger || _gameData.Creatures[i].Herbavore >= _gameData.MapGridData[p.X, p.Y].Food[k].FoodStrength))
                                                {
                                                    if (_gameData.Creatures[i].Bounds.Intersects(_gameData.MapGridData[p.X, p.Y].Food[k].Bounds))
                                                    {
                                                        _gameData.Creatures[i].UndigestedFood++;
                                                        _gameData.Creatures[i].TotalFoodEaten++;
                                                        _gameData.Creatures[i].Energy += _gameData.Settings.EnergyGivenFromFood + (_gameData.Creatures[i].FoodDigestion / 10); //Slower food digestion means you pull more energy from the food
                                                        _gameData.MapGridData[p.X, p.Y].Food[k].MarkForDelete = true;
                                                        //_gameData.Food.Where(t => t == _gameData.MapGridData[p.X, p.Y].Food[k]).First().MarkForDelete = true;
                                                        //_gameData.RemoveFoodFromGrid(tmpFood, _gameData.MapGridData[p.X, p.Y].Food[k].GridPositions);
                                                        //_gameData.Food.Remove(tmpFood);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Plant Collision
                        if ((_gameData.Creatures[i].IsHerbavore && !_gameData.Creatures[i].IsOmnivore) && _gameData.Creatures[i].UndigestedFood < _gameData.MaxCreatureUndigestedFood)
                        {
                            foreach (Point p in _gameData.Creatures[i].GridPositions)
                            {
                                lock (_gameData.LockPlants)
                                {
                                    ////Temp creature lock to wait for creature cleanup to finish before we process collisions. This code is not important enough to retain a full lock
                                    //lock (_gameData.LockCreatures) { }
                                    for (int k = (_gameData.MapGridData[p.X, p.Y].Plants.Count - 1); k >= 0; k--)
                                    {
                                        if (_gameData.Creatures[i].Herbavore >= _gameData.MapGridData[p.X, p.Y].Plants[k].FoodStrength)
                                        {
                                            if (_gameData.Creatures[i].Bounds.Intersects(_gameData.MapGridData[p.X, p.Y].Plants[k].Bounds))
                                            {
                                                //Make sure the creature is not on cooldown before attempting to eat from the food
                                                if (_gameData.MapGridData[p.X, p.Y].Plants[k].Interactions.Count(t => t.CreatureID == _gameData.Creatures[i].CreatureId) <= 0)
                                                {
                                                    float foodAwarded = _gameData.MapGridData[p.X, p.Y].Plants[k].Eat(_gameData.Creatures[i].CreatureId);

                                                    if (foodAwarded > 0)
                                                    {
                                                        _gameData.Creatures[i].UndigestedFood += foodAwarded;
                                                        _gameData.Creatures[i].TotalFoodEaten++;
                                                        _gameData.Creatures[i].Energy += ((_gameData.Settings.EnergyGivenFromFood + (_gameData.Creatures[i].FoodDigestion / 10)) * foodAwarded) * 1.1f; //Slower food digestion means you pull more energy from the food
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        try
                        {
                            UpdateCreatureSight(i);
                        }
                        catch (Exception) { } //Ignore this error, if the creature list updates during vision checks then we just don't get to change targets this iteration
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(_gameData.SessionID.ToString(), "ErrorLogCollisionThread.txt"), Environment.NewLine + DateTime.Now.ToString() + " - Collision Thread Uncaught error: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
            }
            _gameData.LockCreatures.Locker = "";

            //Thread.Sleep(33); //We do not want to rapidly loop this code since it will slow down the main thread while we hold locks
        }
    }
    private void UpdateCreatureSight(int creatureIndex)
    {
        //Vision is calculated from the center of the creature, if they do not have vision greater than their texture size there is no point calculating for vision
        if (_gameData.Creatures[creatureIndex].UndigestedFood < _gameData.MaxCreatureUndigestedFood && !_gameData.Creatures[creatureIndex].IsLeavingClimate && _gameData.Creatures[creatureIndex].Sight > 0 && _gameData.Creatures[creatureIndex].TicksSinceLastVisionCheck >= _gameData.Creatures[creatureIndex].TicksBetweenVisionChecks)
        {
            _gameData.Creatures[creatureIndex].TicksSinceLastVisionCheck = 0; //Reset the counter

            bool foundTarget = false;
            Vector2 newDestination = Vector2.Zero;
            float newDestinationDistance = 99999999999999f;

            if (_gameData.Creatures[creatureIndex].IsHerbavore)
            {
                foundTarget = CheckForNearestFoodLocation(_gameData.Creatures[creatureIndex].GridPositions, _gameData.Creatures[creatureIndex], ref newDestination, ref newDestinationDistance);
            }
            else if (_gameData.Creatures[creatureIndex].IsCarnivore)
            {
                foundTarget = CheckForNearestPreyLocation(_gameData.Creatures[creatureIndex].GridPositions, _gameData.Creatures[creatureIndex], ref newDestination, ref newDestinationDistance);
            }
            else if (_gameData.Creatures[creatureIndex].IsScavenger)
            {
                List<int> refInvisEggIDs = new List<int>();
                foundTarget = CheckForNearestEggLocation(_gameData.Creatures[creatureIndex].GridPositions, _gameData.Creatures[creatureIndex], ref newDestination, ref newDestinationDistance, ref refInvisEggIDs);

                if (refInvisEggIDs.Count > 0)
                {
                    _gameData.Creatures[creatureIndex].InvisibleEggs.AddRange(refInvisEggIDs);
                }
            }

            //Need to expand the search area if we did not find an object. This will be more noticable as the creature sight increases
            if (!foundTarget)
            {
                List<Point> gridLocations = new List<Point>();
                foreach (Point p in _gameData.Creatures[creatureIndex].GridPositions)
                {
                    if (p.X > 0)
                    {
                        gridLocations.Add(new Point(p.X - 1, p.Y));

                        if (p.Y > 0)
                        {
                            gridLocations.Add(new Point(p.X - 1, p.Y - 1));
                        }
                        if (p.Y < _gameData.MapGridData.GetLength(1) - 1)
                        {
                            gridLocations.Add(new Point(p.X - 1, p.Y + 1));
                        }
                    }
                    if (p.X < _gameData.MapGridData.GetLength(0) - 1)
                    {
                        gridLocations.Add(new Point(p.X + 1, p.Y));

                        if (p.Y > 0)
                        {
                            gridLocations.Add(new Point(p.X + 1, p.Y - 1));
                        }
                        if (p.Y < _gameData.MapGridData.GetLength(1) - 1)
                        {
                            gridLocations.Add(new Point(p.X + 1, p.Y + 1));
                        }
                    }
                    if (p.Y > 0)
                    {
                        gridLocations.Add(new Point(p.X, p.Y - 1));
                    }
                    if (p.Y < _gameData.MapGridData.GetLength(1) - 1)
                    {
                        gridLocations.Add(new Point(p.X, p.Y + 1));
                    }
                }

                //Now that we have a list of grid locations we need to remove the ones we just checked
                for (int j = gridLocations.Count - 1; j >= 0; j--)
                {
                    foreach (Point p in _gameData.Creatures[creatureIndex].GridPositions)
                    {
                        if (gridLocations[j].X == p.X && gridLocations[j].Y == p.Y)
                        {
                            gridLocations.RemoveAt(j);
                            break;
                        }
                    }
                }

                //Check expanded
                if (_gameData.Creatures[creatureIndex].IsHerbavore)
                {
                    foundTarget = CheckForNearestFoodLocation(gridLocations, _gameData.Creatures[creatureIndex], ref newDestination, ref newDestinationDistance);
                }
                else if (_gameData.Creatures[creatureIndex].IsCarnivore)
                {
                    foundTarget = CheckForNearestPreyLocation(gridLocations, _gameData.Creatures[creatureIndex], ref newDestination, ref newDestinationDistance);
                }
                else if (_gameData.Creatures[creatureIndex].IsScavenger)
                {
                    List<int> refInvisEggIDs = new List<int>();
                    foundTarget = CheckForNearestEggLocation(gridLocations, _gameData.Creatures[creatureIndex], ref newDestination, ref newDestinationDistance, ref refInvisEggIDs);

                    if (refInvisEggIDs.Count > 0)
                    {
                        _gameData.Creatures[creatureIndex].InvisibleEggs.AddRange(refInvisEggIDs);
                    }
                }
            }

            if (foundTarget)
            {
                _gameData.Creatures[creatureIndex].CalculatedIntercept = newDestination;
                if (_gameData.Creatures[creatureIndex].Position.X < newDestination.X && _gameData.Creatures[creatureIndex].Position.Y < newDestination.Y)
                {
                    double inner = Math.Abs((_gameData.Creatures[creatureIndex].Position.X - newDestination.X)) / Math.Abs((_gameData.Creatures[creatureIndex].Position.Y - newDestination.Y));
                    double aTan = Math.Atan(inner);
                    float degreesRotation = (float)(Math.PI - aTan);
                    _gameData.Creatures[creatureIndex].Rotation = degreesRotation;
                }
                else if (_gameData.Creatures[creatureIndex].Position.X > newDestination.X && _gameData.Creatures[creatureIndex].Position.Y < newDestination.Y)
                {
                    double inner = Math.Abs((_gameData.Creatures[creatureIndex].Position.X - newDestination.X)) / Math.Abs((_gameData.Creatures[creatureIndex].Position.Y - newDestination.Y));
                    double aTan = Math.Atan(inner);
                    float degreesRotation = (float)(Math.PI + aTan);
                    _gameData.Creatures[creatureIndex].Rotation = degreesRotation;
                }
                else if (_gameData.Creatures[creatureIndex].Position.X > newDestination.X && _gameData.Creatures[creatureIndex].Position.Y > newDestination.Y)
                {
                    double inner = Math.Abs((_gameData.Creatures[creatureIndex].Position.X - newDestination.X)) / Math.Abs((_gameData.Creatures[creatureIndex].Position.Y - newDestination.Y));
                    double aTan = Math.Atan(inner);
                    float degreesRotation = (float)((Math.PI * 2) - aTan);
                    _gameData.Creatures[creatureIndex].Rotation = degreesRotation;
                }
                else if (_gameData.Creatures[creatureIndex].Position.X < newDestination.X && _gameData.Creatures[creatureIndex].Position.Y > newDestination.Y)
                {
                    double inner = Math.Abs((_gameData.Creatures[creatureIndex].Position.X - newDestination.X)) / Math.Abs((_gameData.Creatures[creatureIndex].Position.Y - newDestination.Y));
                    double aTan = Math.Atan(inner);
                    float degreesRotation = (float)(aTan);
                    _gameData.Creatures[creatureIndex].Rotation = degreesRotation;
                }
            }
            else
            {
                _gameData.Creatures[creatureIndex].CalculatedIntercept = Vector2.Zero;
            }
        }
    }
    private bool CheckForNearestFoodLocation(List<Point> gridPositions, Creature creature, ref Vector2 closest, ref float distance)
    {
        bool foundFood = false;

        for (int k = 0; k < gridPositions.Count; k++)
        {
            if (creature.IsHerbavore && !creature.IsOmnivore)
            {
                foreach (Plant f in _gameData.MapGridData[gridPositions[k].X, gridPositions[k].Y].Plants)
                {
                    if (f.FoodStrength <= creature.Herbavore && f.Interactions.Count(t => t.CreatureID == creature.CreatureId) == 0)
                    {
                        float tmpDistance = Vector2.Distance(creature.Position, f.Position);
                        if (tmpDistance <= creature.Sight + creature.TextureCollideDistance) //We are not dividing the TextureCollisionDistance by 2 to give the creature an initial sight boost
                        {
                            if (tmpDistance < distance)
                            {
                                foundFood = true;
                                closest = f.Position;
                                distance = tmpDistance;
                            }
                        }
                    }
                }
            }

            //Move to a Plant as a priority instead of regular food since plants give more benefits
            if (!foundFood)
            {
                foreach (Food f in _gameData.MapGridData[gridPositions[k].X, gridPositions[k].Y].Food)
                {
                    if (!f.MarkForDelete && f.FoodType == creature.FoodTypeInt && f.FoodStrength <= creature.Herbavore)
                    {
                        float tmpDistance = Vector2.Distance(creature.Position, f.Position);
                        if (tmpDistance <= creature.Sight + creature.TextureCollideDistance) //We are not dividing the TextureCollisionDistance by 2 to give the creature an initial sight boost
                        {
                            if (tmpDistance < distance)
                            {
                                foundFood = true;
                                closest = f.Position;
                                distance = tmpDistance;
                            }
                        }
                    }
                }
            }
        }

        return foundFood;
    }
    private bool CheckForNearestEggLocation(List<Point> gridPositions, Creature creature, ref Vector2 closest, ref float distance, ref List<int> invisibleEggIDs)
    {
        invisibleEggIDs = new List<int>();
        bool foundEgg = false;

        for (int k = 0; k < gridPositions.Count; k++)
        {
            foreach (Egg f in _gameData.MapGridData[gridPositions[k].X, gridPositions[k].Y].Eggs)
            {
                if (f.Creature.SpeciesId != creature.SpeciesId && creature.InvisibleEggs.Count(t => t == f.Creature.CreatureId) == 0)
                {
                    float tmpDistance = Vector2.Distance(creature.Position, f.Position);
                    if (tmpDistance <= creature.Sight + creature.TextureCollideDistance) //We are not dividing the TextureCollisionDistance by 2 to give the creature an initial sight boost
                    {
                        if (tmpDistance < distance)
                        {
                            //Chance for egg camo to cancel moving to the egg
                            if (_rand.Next(0, 100) >= 100 - (int)Math.Ceiling(f.Camo))
                            {
                                invisibleEggIDs.Add(f.Creature.CreatureId);
                            }
                            else
                            {
                                foundEgg = true;
                                closest = f.Position;
                                distance = tmpDistance;
                            }
                        }
                    }
                }
            }
            if (!foundEgg)
            {
                foreach (Food f in _gameData.MapGridData[gridPositions[k].X, gridPositions[k].Y].Food)
                {
                    if (f.FoodType == creature.FoodTypeInt && f.FoodStrength <= creature.Herbavore)
                    {
                        float tmpDistance = Vector2.Distance(creature.Position, f.Position);
                        if (tmpDistance <= creature.Sight + creature.TextureCollideDistance) //We are not dividing the TextureCollisionDistance by 2 to give the creature an initial sight boost
                        {
                            if (tmpDistance < distance)
                            {
                                foundEgg = true;
                                closest = f.Position;
                                distance = tmpDistance;
                            }
                        }
                    }
                }
            }
        }

        return foundEgg;
    }
    private bool CheckForNearestPreyLocation(List<Point> gridPositions, Creature creature, ref Vector2 closest, ref float distance)
    {
        bool foundPrey = false;

        for (int k = 0; k < gridPositions.Count; k++)
        {
            foreach (Creature f in _gameData.MapGridData[gridPositions[k].X, gridPositions[k].Y].Creatures)
            {
                if (f.SpeciesId != creature.SpeciesId)
                {
                    //Can eat all herbavores. If we are a true Carnivore we can eat other carnivores. We can eat all Scavengers lower than us.
                    if (f.IsHerbavore || (f.IsScavenger && creature.Carnivore - _gameData.Settings.CarnivoreLevelBuffer > f.Scavenger) || (!creature.IsOmnivore && f.IsCarnivore && creature.Carnivore - _gameData.Settings.CarnivoreLevelBuffer > f.Carnivore))
                    {
                        float tmpDistance = 9999999;
                        Vector2? impactPosition = CollisionDetection.FindCollisionPoint(f.Position, f.Direction * f.Speed, creature.Position, creature.Speed);
                        tmpDistance = Vector2.Distance(creature.Position, (Vector2)impactPosition);

                        if (impactPosition != null && !float.IsNaN(impactPosition.Value.X))
                        {
                            if (tmpDistance <= creature.Sight + creature.TextureCollideDistance) //We are not dividing the TextureCollisionDistance by 2 to give the creature an initial sight boost
                            {
                                if (tmpDistance < distance)
                                {
                                    foundPrey = true;
                                    closest = (Vector2)impactPosition;
                                    distance = tmpDistance;
                                }
                            }
                        }
                    }
                }
            }
        }

        return foundPrey;
    }
}