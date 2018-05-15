﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameData
{
    private int nextSpeciesId;
    private int topSpeciesId; //ID for the species with the most creatures
    private int topSpeciesCount; //The count when this was last calculated
    private string topSpeciesName; //Text species name for the species with the most creatures
    private bool initialThreshholdForTopSpeciesLogged;
    private int initialThreshhold = 5;

    public GameSettings Settings { get; set; }
    public CreatureSettings CreatureSettings { get; set; }
    public MutationSettings MutationSettings { get; set; }
    public MapStatistics MapStatistics { get; set; } //Map stats for the top bar on the HUD
    public List<Creature> Creatures { get; set; } //List of creatures on the map
    public List<Creature> DeadCreatures { get; set; } //Used for writing stats at the end
    public List<Egg> Eggs { get; set; } //Eggs on the map
    public List<Food> Food { get; set; } //Food on the map
    public Creature Focus { get; set; } //Camera focus, the camera class will follow whatever Creature is selected here
    public int FocusIndex { get; set; } //Camera focus index, this value is used when Paging between Creatures
    public List<SpeciesToCount> ChartData { get; set; }
    public List<SpeciesToCount> ChartDataTop { get; set; }
    public List<string> EventLog { get; set; } //Log of events that occur so that it is easier to follow what is happening in game
    public GridData[,] MapGridData { get; set; }
    public int NextSpeciesId
    {
        get
        {
            nextSpeciesId = nextSpeciesId + 1;
            return nextSpeciesId;
        }
        set
        {
            nextSpeciesId = value;
        }
    }
    public bool ResetGame { get; set; }
    public bool BuildSettingsPanel { get; set; }
    public bool ShowChart { get; set; }
    public bool ShowControls { get; set; }
    public bool HighlightSpecies { get; set; }
    public bool EggMarkers { get; set; }
    public bool HerbavoreMarkers { get; set; }
    public bool CarnivoreMarkers { get; set; }
    public bool ScavengerMarkers { get; set; }
    public bool OmnivoreMarkers { get; set; }
    public bool ShowCreatureStats { get; set; }
    public bool ShowFoodStrength { get; set; }
    public bool ShowDebugData { get; set; }
    public bool ShowSettingsPanel { get; set; }
    public bool ShowEventLogPanel { get; set; }

    private const int CREATURES_COUNT_FOR_CHART = 15;

    public GameData()
    {
        MapStatistics = new MapStatistics();
        ChartData = new List<SpeciesToCount>();
        ChartDataTop = new List<SpeciesToCount>();
        EventLog = new List<string>();
        EventLog.Add("Game Started!");
        Creatures = new List<Creature>();
        DeadCreatures = new List<Creature>();
        Eggs = new List<Egg>();
        Food = new List<Food>();
        Focus = null; //Init the focus to null to not follow any creatures
        FocusIndex = -1;
        nextSpeciesId = 0;
        ShowChart = true;
        ShowControls = true;
        HighlightSpecies = false;
        HerbavoreMarkers = false;
        CarnivoreMarkers = false;
        ScavengerMarkers = false;
        OmnivoreMarkers = false;
        ShowCreatureStats = true;
        ShowFoodStrength = false;
        ShowDebugData = false;
        ShowSettingsPanel = false;
        ResetGame = false;
        BuildSettingsPanel = false;
        ShowEventLogPanel = true;
        topSpeciesId = -1;
        topSpeciesName = String.Empty;
    }

    public void CalculateMapStatistics()
    {
        MapStatistics.AliveCreatures = Creatures.Count;
        MapStatistics.DeadCreatures = DeadCreatures.Count;
        MapStatistics.FoodOnMap = Food.Count;
        MapStatistics.EggsOnMap = Eggs.Count;
        MapStatistics.PercentHerbavore = Math.Round((double)Creatures.Where(o => o.IsHerbavore && !o.IsOmnivore).Count() / MapStatistics.AliveCreatures, 2);
        MapStatistics.PercentCarnivore = Math.Round((double)Creatures.Where(o => o.IsCarnivore && !o.IsOmnivore).Count() / MapStatistics.AliveCreatures, 2);
        MapStatistics.PercentScavenger = Math.Round((double)Creatures.Where(o => o.IsScavenger).Count() / MapStatistics.AliveCreatures, 2);
        MapStatistics.PercentOmnivore = Math.Round((double)Creatures.Where(o => o.IsOmnivore).Count() / MapStatistics.AliveCreatures, 2);
        MapStatistics.UniqueSpecies = GetUniqueSpeciesCount();

        double percentTotal = MapStatistics.PercentHerbavore + MapStatistics.PercentCarnivore + MapStatistics.PercentScavenger + MapStatistics.PercentOmnivore;
        if (percentTotal != 1)
        {
            if (percentTotal > 1)
            {
                MapStatistics.PercentHerbavore = MapStatistics.PercentHerbavore - (percentTotal - 1);
            }
            else
            {
                MapStatistics.PercentHerbavore = MapStatistics.PercentHerbavore + (1 - percentTotal);
            }
        }
    }
    public int GetUniqueSpeciesCount()
    {
        return Creatures.Select(o => o.Species).Distinct().Count();
    }
    public void InitializeChartData(Random _rand)
    {
        //Initialization
        for (int i = 0; i < Creatures.Count; i++)
        {
            SpeciesToCount sc = new SpeciesToCount();
            sc.Rand = _rand;
            sc.Name = Creatures[i].Species;
            sc.CreatureType = Creatures[i].GetCreatureTypeText();
            sc.Id = Creatures[i].SpeciesId;
            sc.CountsOverTime.Add(1);

            ChartData.Add(sc);
        }
    }
    public void CalculateChartData(Random _rand)
    {
        if (ChartData.Count == 0)
        {
            InitializeChartData(_rand);
        }
        else
        {
            //Expand the CountsOverTime variable in ChartData
            foreach (SpeciesToCount sc in ChartData)
            {
                sc.CountsOverTime.Add(0);
            }

            List<SpeciesDistinct> newList = new List<SpeciesDistinct>();
            int preXcount = ChartData[ChartData.Count - 1].CountsOverTime.Count;
            foreach (Creature c in Creatures)
            {
                bool found = false;
                foreach (SpeciesToCount sc in ChartData)
                {
                    if (c.SpeciesId == sc.Id)
                    {
                        found = true;
                        sc.CountsOverTime[sc.CountsOverTime.Count - 1]++;
                        break;
                    }
                }
                if (!found) //New Species was introduced, we need to fill the data so the StackedArea chart does not complain
                {
                    SpeciesToCount sc = new SpeciesToCount();
                    sc.Rand = _rand;
                    sc.Name = c.Species;
                    sc.CreatureType = c.GetCreatureTypeText();
                    sc.Id = c.SpeciesId;

                    ChartData.Add(sc);

                    for (int i = 0; i < preXcount; i++)
                    {
                        sc.CountsOverTime.Add(0); //Fill in all the rows first
                    }
                    sc.CountsOverTime[sc.CountsOverTime.Count - 1]++;
                }
            }

            //Cleanup ChartData for Species that are Extinct
            for (int i = ChartData.Count - 1; i >= 0; i--)
            {
                //Update the top count while searching for extinct species
                if (ChartData[i].Id == topSpeciesId)
                {
                    topSpeciesCount = ChartData[i].CountsOverTime[ChartData[i].CountsOverTime.Count - 1];
                }
                if (ChartData[i].CountsOverTime[ChartData[i].CountsOverTime.Count - 1] == 0)
                {
                    //Make sure that the species does not have a creature in an egg before removing
                    bool foundInEgg = false;
                    foreach (Egg e in Eggs)
                    {
                        if (e.Creature.SpeciesId == ChartData[i].Id)
                        {
                            foundInEgg = true;
                            break;
                        }
                    }

                    if(!foundInEgg)
                        ChartData.RemoveAt(i);
                }
            }

            //Cleanup ChartData when species evolve and all species are evolved species since the beginning part of the graph becomes useless
            bool allZero = true;
            while (allZero)
            {
                for (int i = 0; i < ChartData.Count; i++)
                {
                    if (ChartData[i].CountsOverTime[0] != 0)
                    {
                        allZero = false;
                        break;
                    }
                }
                if (allZero)
                {
                    for (int i = 0; i < ChartData.Count; i++)
                    {
                        ChartData[i].CountsOverTime.RemoveAt(0);
                    }
                }
            }

            ChartDataTop.Clear();
            List<SpeciesToCount> topList = ChartData.Where(t => t.CountsOverTime[t.CountsOverTime.Count - 1] > 0).OrderByDescending(t => t.CountsOverTime[t.CountsOverTime.Count - 1]).ToList();
            int countToGet = topList.Count();
            if (countToGet > CREATURES_COUNT_FOR_CHART)
                countToGet = CREATURES_COUNT_FOR_CHART;

            for (int i = 0; i < countToGet; i++)
            {
                ChartDataTop.Add(topList[i]);
            }

            //Check to see if we have a new species lead
            int checkedSpeciesCount = topList[0].CountsOverTime[topList[0].CountsOverTime.Count - 1];

            if (checkedSpeciesCount > initialThreshhold)
            {
                initialThreshholdForTopSpeciesLogged = true;
            }

            if (initialThreshholdForTopSpeciesLogged)
            {
                if (checkedSpeciesCount > topSpeciesCount)
                {
                    if (topSpeciesId != topList[0].Id)
                    {
                        if (!String.IsNullOrEmpty(topSpeciesName))
                            EventLog.Add(topSpeciesName + " is no longer the dominant species");

                        topSpeciesCount = checkedSpeciesCount;
                        topSpeciesId = topList[0].Id;
                        topSpeciesName = topList[0].Name;

                        EventLog.Add(topSpeciesName + " is now the dominant species");
                    }
                }
            }
        }
    }
    public void SetIndexPositionsForCreatures()
    {
        for (int i = 0; i < Creatures.Count; i++)
        {
            if (Creatures[i] == Focus)
            {
                FocusIndex = i;
                break;
            }
        }
    }
    public void AddDeadCreatureToList(Creature creature)
    {
        DeadCreatures.Add(creature);
    }
    public void AddFoodToGrid(Food food)
    {
        foreach (Point p in food.GridPositions)
        {
            MapGridData[p.X, p.Y].Food.Add(food);
        }
    }
    public void AddEggToGrid(Egg egg)
    {
        foreach (Point p in egg.GridPositions)
        {
            MapGridData[p.X, p.Y].Eggs.Add(egg);
        }
    }
    public void AddCreatureToGrid(Creature creature)
    {
        foreach (Point p in creature.GridPositions)
        {
            MapGridData[p.X, p.Y].Creatures.Add(creature);
        }
    }
    public void AddCreatureDeltaToGrid(Creature creature, List<Point> toBeAdded)
    {
        foreach (Point p in toBeAdded)
        {
            MapGridData[p.X, p.Y].Creatures.Add(creature);
        }
    }
    public void RemoveCreatureFromGrid(Creature creature, List<Point> toBeRemoved)
    {
        foreach (Point p in toBeRemoved)
        {
            MapGridData[p.X, p.Y].Creatures.Remove(creature);
        }
    }
    public void RemoveFoodFromGrid(Food food, List<Point> toBeRemoved)
    {
        foreach (Point p in toBeRemoved)
        {
            MapGridData[p.X, p.Y].Food.Remove(food);
        }
    }
    public void RemoveEggFromGrid(Egg egg, List<Point> toBeRemoved)
    {
        foreach (Point p in toBeRemoved)
        {
            MapGridData[p.X, p.Y].Eggs.Remove(egg);
        }
    }
    public List<string> GetEventsForDisplay(int countToDisplay)
    {
        List<string> logText = new List<string>();
        int startingPos = 0;

        startingPos = EventLog.Count - countToDisplay;

        if (startingPos < 0)
        {
            return EventLog;
        }
        else
        {
            for (int i = startingPos; i < EventLog.Count; i++)
            {
                logText.Add(EventLog[i]);
            }

            return logText;
        }
    }
    public void PruneEventLog(int countToKeep)
    {
        if (EventLog.Count > countToKeep)
        {
            for (int i = 0; i < (EventLog.Count - countToKeep); i++)
            {
                EventLog.RemoveAt(0);
            }
        }
    }
}