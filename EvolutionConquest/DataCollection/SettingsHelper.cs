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
            initialSetting.StartingFoodRatio = 0.0005f;
            initialSetting.FoodGenerationValue = 250000;
            initialSetting.TicksUntilFoodUpgradeStarts = 54000;
            initialSetting.TicksBetweenFoodUpgrades = 4500;
            initialSetting.StartingCreatureRatio = 0.5f;
            initialSetting.FoodUpgradeAmount = 1;
            initialSetting.FoodUpgradeChancePercent = 20;
            initialSetting.MaxFoodLevel = 50;
            initialSetting.EnergyGivenFromFood = 100;
            initialSetting.EnergyConsumptionFromLayingEgg = 50;
            initialSetting.EnergyDepletionFromMovement = 0.01f;
            initialSetting.CarnivoreLevelBuffer = 5;

            WriteSettings(path, initialSetting);
        }

        json = File.ReadAllText(path);
        settings = new JavaScriptSerializer().Deserialize<GameSettings>(json);

        return settings;
    }
    public static void WriteSettings(string path, GameSettings settings)
    {
        string json = new JavaScriptSerializer().Serialize(settings);
        File.WriteAllText(path, JsonHelper.FormatJson(json));
    }
}