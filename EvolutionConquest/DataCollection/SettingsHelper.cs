using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

public class SettingsHelper
{
    public SettingsHelper()
    {
    }

    public static GameSettings ReadSettings(string path)
    {
        GameSettings settings;
        string json = String.Empty;

        if (!File.Exists(path))
        {
            GameSettings initialSetting = new GameSettings();

            initialSetting.DatabaseName = "Test";
            initialSetting.ServerName = "YourServerAndInstanceNameHere";
            initialSetting.UserName = "UsrName_Here_Must_Be_SQL_Authentication";
            initialSetting.Password = "YourSecurePasswordHere";
            initialSetting.WorldSize = 5000;
            initialSetting.ClimateHeightPercent = 20;
            initialSetting.StartingFoodRatio = 50f;
            initialSetting.FoodGenerationValue = 25f;
            initialSetting.TicksUntilFoodUpgradeStarts = 1800;
            initialSetting.TicksBetweenFoodUpgrades = 150;
            initialSetting.StartingCreatureRatio = 50f;
            initialSetting.FoodUpgradeAmount = 1;
            initialSetting.FoodUpgradeChancePercent = 20;
            initialSetting.MaxFoodLevel = 50;
            initialSetting.EnergyGivenFromFood = 100;
            initialSetting.EnergyConsumptionFromLayingEgg = 50;
            initialSetting.EnergyDepletionFromMovement = 10f;
            initialSetting.CarnivoreLevelBuffer = 5;

            WriteSettings(path, initialSetting);
        }

        json = File.ReadAllText(path);
        settings = new JavaScriptSerializer().Deserialize<GameSettings>(json);

        return settings;
    }
    public static CreatureSettings ReadCreatureSettings(string path)
    {
        CreatureSettings settings;
        string json = String.Empty;

        if (!File.Exists(path))
        {
            CreatureSettings initialSetting = new CreatureSettings();

            initialSetting.StartingEggIntervalMin = 40;
            initialSetting.StartingEggIntervalMax = 50;
            initialSetting.StartingEggIncubationMin = 40;
            initialSetting.StartingEggIncubationMax = 80;
            initialSetting.StartingFoodDigestionMin = 5;
            initialSetting.StartingFoodDigestionMax = 25;
            initialSetting.StartingSpeedMin = 5;
            initialSetting.StartingSpeedMax = 25;
            initialSetting.StartingLifespanMin = 100;
            initialSetting.StartingLifespanMax = 120;
            initialSetting.StartingHerbavoreLevelMin = 1;
            initialSetting.StartingHerbavoreLevelMax = 2;
            initialSetting.StartingCarnivoreLevelMin = 0;
            initialSetting.StartingCarnivoreLevelMax = 0;
            initialSetting.StartingScavengerLevelMin = 0;
            initialSetting.StartingScavengerLevelMax = 0;
            initialSetting.StartingOmnivoreLevelMin = 0;
            initialSetting.StartingOmnivoreLevelMax = 0;
            initialSetting.StartingColdToleranceMin = 0;
            initialSetting.StartingColdToleranceMax = 10;
            initialSetting.StartingHotToleranceMin = 0;
            initialSetting.StartingHotToleranceMax = 10;
            initialSetting.StartingEnergy = 425;

            WriteCreatureSettings(path, initialSetting);
        }

        json = File.ReadAllText(path);
        settings = new JavaScriptSerializer().Deserialize<CreatureSettings>(json);

        return settings;
    }
    public static void WriteSettings(string path, GameSettings settings)
    {
        string json = new JavaScriptSerializer().Serialize(settings);
        File.WriteAllText(path, JsonHelper.FormatJson(json));
    }
    public static void WriteCreatureSettings(string path, CreatureSettings settings)
    {
        string json = new JavaScriptSerializer().Serialize(settings);
        File.WriteAllText(path, JsonHelper.FormatJson(json));
    }
}