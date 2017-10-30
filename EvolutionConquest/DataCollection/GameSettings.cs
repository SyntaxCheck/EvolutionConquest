using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameSettings
{
    //Database Settings
    public string ServerName { get; set; }
    public string DatabaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    //Game Settings
    public int WorldSize { get; set; }
    public int ClimateHeightPercent { get; set; }
    public float StartingFoodRatio { get; set; }
    public float FoodGenerationValue { get; set; }
    public float StartingCreatureRatio { get; set; }
    public float TicksUntilFoodUpgradeStarts { get; set; }
    public float TicksBetweenFoodUpgrades { get; set; }
    public int FoodUpgradeAmount { get; set; }
    public int FoodUpgradeChancePercent { get; set; }
    public int MaxFoodLevel { get; set; }
    public float EnergyGivenFromFood { get; set; }
    public float EnergyConsumptionFromLayingEgg { get; set; }
    public float EnergyDepletionFromMovement { get; set; }
    public int CarnivoreLevelBuffer { get; set; }

    public GameSettings()
    {
    }
}