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

            SetDefaultWorld(ref initialSetting);

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

            SetDefaultCreature(ref initialSetting);

            WriteCreatureSettings(path, initialSetting);
        }

        json = File.ReadAllText(path);
        settings = new JavaScriptSerializer().Deserialize<CreatureSettings>(json);

        return settings;
    }
    public static MutationSettings ReadMutationSettings(string path)
    {
        MutationSettings settings;
        string json = String.Empty;

        if (!File.Exists(path))
        {
            MutationSettings initialSetting = new MutationSettings();

            SetDefaultMutation(ref initialSetting);

            WriteMutationSettings(path, initialSetting);
        }

        json = File.ReadAllText(path);
        settings = new JavaScriptSerializer().Deserialize<MutationSettings>(json);

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
    public static void WriteMutationSettings(string path, MutationSettings settings)
    {
        string json = new JavaScriptSerializer().Serialize(settings);
        File.WriteAllText(path, JsonHelper.FormatJson(json));
    }
    public static void SetDefaultWorld(ref GameSettings settingsIn)
    {
        settingsIn.WorldSize = 5000;
        settingsIn.ClimateHeightPercent = 20;
        settingsIn.StartingFoodRatio = 200f;
        settingsIn.FoodGenerationValue = 10f;
        settingsIn.TicksUntilFoodUpgradeStarts = 1800;
        settingsIn.TicksBetweenFoodUpgrades = 150;
        settingsIn.StartingPlantRatio = 100f;
        settingsIn.StartingCreatureRatio = 10f;
        settingsIn.FoodUpgradeAmount = 1;
        settingsIn.FoodUpgradeChancePercent = 20;
        settingsIn.MaxFoodLevel = 50;
        settingsIn.EnergyGivenFromFood = 100;
        settingsIn.EnergyConsumptionFromLayingEgg = 50;
        settingsIn.EnergyDepletionFromMovement = 20f;
        settingsIn.EnergyDepletionPercentFromComplexity = 90f;
        settingsIn.CarnivoreLevelBuffer = 5;
    }
    public static void SetDefaultCreature(ref CreatureSettings settingsIn)
    {
        settingsIn.StartingEggIntervalMin = 40;
        settingsIn.StartingEggIntervalMax = 50;
        settingsIn.StartingEggIncubationMin = 40;
        settingsIn.StartingEggIncubationMax = 80;
        settingsIn.StartingFoodDigestionMin = 5;
        settingsIn.StartingFoodDigestionMax = 25;
        settingsIn.StartingSpeedMin = 5;
        settingsIn.StartingSpeedMax = 25;
        settingsIn.StartingLifespanMin = 110;
        settingsIn.StartingLifespanMax = 140;
        settingsIn.StartingHerbavoreLevelMin = 1;
        settingsIn.StartingHerbavoreLevelMax = 2;
        settingsIn.StartingCarnivoreLevelMin = 0;
        settingsIn.StartingCarnivoreLevelMax = 0;
        settingsIn.StartingScavengerLevelMin = 0;
        settingsIn.StartingScavengerLevelMax = 0;
        settingsIn.StartingOmnivoreLevelMin = 0;
        settingsIn.StartingOmnivoreLevelMax = 0;
        settingsIn.StartingColdToleranceMin = 0;
        settingsIn.StartingColdToleranceMax = 10;
        settingsIn.StartingHotToleranceMin = 0;
        settingsIn.StartingHotToleranceMax = 10;
        settingsIn.StartingEnergy = 350;
    }
    public static void SetDefaultMutation(ref MutationSettings settingsIn)
    {
        settingsIn.ChanceToIncreaseValue = 60f;
        settingsIn.ChangeAmount = 1f;
        settingsIn.MutationBonusPercent = 50f;
        settingsIn.Attraction = 3f;
        settingsIn.Camo = 3f;
        settingsIn.Carnivore = 5f;
        settingsIn.Cloning = 3f;
        settingsIn.ColdClimateTolerance = 15f;
        settingsIn.EggCamo = 5f;
        settingsIn.EggIncubation = 25f;
        settingsIn.EggInterval = 25f;
        settingsIn.EggToxicity = 5f;
        settingsIn.FoodDigestion = 25f;
        settingsIn.Herbavore = 15f;
        settingsIn.HotClimateTolerance = 15f;
        settingsIn.Lifespan = 25f;
        settingsIn.Omnivore = 5f;
        settingsIn.Scavenger = 5f;
        settingsIn.Sight = 3f;
        settingsIn.Speed = 10f;
        settingsIn.FoodType = 10f;
    }
}