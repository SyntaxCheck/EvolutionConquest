using EvolutionConquest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public class Player
{
    private bool isSliderActive;

    public Player()
    {
        isSliderActive = false;
    }

    public void HandleInput(InputState inputState, PlayerIndex? controllingPlayer, ref GameData gameData, TabPanel tabPanel)
    {
        PlayerIndex playerIndex;

        if (inputState.IsNewKeyPress(Keys.F, controllingPlayer, out playerIndex))
        {
            gameData.HighlightSpecies = !gameData.HighlightSpecies;
        }
        if (inputState.IsNewKeyPress(Keys.F12, controllingPlayer, out playerIndex))
        {
            gameData.ShowControls = !gameData.ShowControls;
        }
        if (inputState.IsNewKeyPress(Keys.F11, controllingPlayer, out playerIndex))
        {
            gameData.ShowChart = !gameData.ShowChart;
        }
        if (inputState.IsNewKeyPress(Keys.F10, controllingPlayer, out playerIndex))
        {
            gameData.ShowCreatureStats = !gameData.ShowCreatureStats;
        }
        if (inputState.IsNewKeyPress(Keys.F9, controllingPlayer, out playerIndex))
        {
            gameData.ShowFoodStrength = !gameData.ShowFoodStrength;
        }
        if (inputState.IsNewKeyPress(Keys.F8, controllingPlayer, out playerIndex))
        {
            gameData.ShowDebugData = !gameData.ShowDebugData;
        }
        if (inputState.IsNewKeyPress(Keys.F7, controllingPlayer, out playerIndex))
        {
            gameData.OmnivoreMarkers = !gameData.OmnivoreMarkers;
        }
        if (inputState.IsNewKeyPress(Keys.F6, controllingPlayer, out playerIndex))
        {
            gameData.ScavengerMarkers = !gameData.ScavengerMarkers;
        }
        if (inputState.IsNewKeyPress(Keys.F5, controllingPlayer, out playerIndex))
        {
            gameData.CarnivoreMarkers = !gameData.CarnivoreMarkers;
        }
        if (inputState.IsNewKeyPress(Keys.F4, controllingPlayer, out playerIndex))
        {
            gameData.HerbavoreMarkers = !gameData.HerbavoreMarkers;
        }
        if (inputState.IsNewKeyPress(Keys.F3, controllingPlayer, out playerIndex))
        {
            gameData.EggMarkers = !gameData.EggMarkers;
        }
        if (inputState.IsNewKeyPress(Keys.F2, controllingPlayer, out playerIndex))
        {
            gameData.ShowEventLogPanel = !gameData.ShowEventLogPanel;
        }
        if (inputState.IsNewKeyPress(Keys.F1, controllingPlayer, out playerIndex))
        {
            gameData.ShowSettingsPanel = !gameData.ShowSettingsPanel;
        }

        //Mouse logic
        if (gameData.ShowSettingsPanel)
        {
            if (inputState.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                //Check tab click
                for (int i = 0; i < tabPanel.Tabs.Count; i++)
                {
                    if (tabPanel.Tabs[i].ButtonRectangle.Contains(inputState.CurrentMouseState.Position))
                    {
                        tabPanel.ActiveTab = i;
                        break;
                    }
                }

                //Check for Save click
                if (tabPanel.SaveButton.ButtonRectangle.Contains(inputState.CurrentMouseState.Position))
                {
                    //Save all the settings
                    //Tab 0 - World Settings
                    gameData.Settings.WorldSize = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.WorldSize).First().CurrentValue;
                    gameData.Settings.ClimateHeightPercent = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.ClimatePercent).First().CurrentValue;
                    gameData.Settings.StartingFoodRatio = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingFoodRatio).First().CurrentValue;
                    gameData.Settings.FoodGenerationValue = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.FoodGenerationValue).First().CurrentValue;
                    gameData.Settings.TicksUntilFoodUpgradeStarts = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.SecondsUntilFoodUpgradesStart).First().CurrentValue;
                    gameData.Settings.TicksBetweenFoodUpgrades = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.SecondsBetweenFoodUpgrades).First().CurrentValue;
                    gameData.Settings.FoodUpgradeAmount = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.FoodUpgradeAmount).First().CurrentValue;
                    gameData.Settings.MaxFoodLevel = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.MaxFoodlevel).First().CurrentValue;
                    gameData.Settings.StartingCreatureRatio = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingCreatureRatio).First().CurrentValue;
                    gameData.Settings.EnergyGivenFromFood = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.EnergyGivenFromFood).First().CurrentValue;
                    gameData.Settings.EnergyConsumptionFromLayingEgg = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.EnergyLossFromLayingEgg).First().CurrentValue;
                    gameData.Settings.EnergyDepletionFromMovement = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.EnergyDepletionFromMovementRate).First().CurrentValue;
                    gameData.Settings.EnergyDepletionPercentFromComplexity = (int)tabPanel.Tabs[0].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.EnergyDepletionPercentFromComplexity).First().CurrentValue;

                    //Tab 1 - Creature settings
                    gameData.CreatureSettings.StartingEggIntervalMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingEggIntervalMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingEggIntervalMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingEggIntervalMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingEggIncubationMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingEggIncubationMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingEggIncubationMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingEggIncubationMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingFoodDigestionMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingFoodDigestionMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingFoodDigestionMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingFoodDigestionMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingSpeedMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingSpeedMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingSpeedMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingSpeedMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingLifespanMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingLifespanMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingLifespanMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingLifespanMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingHerbavoreLevelMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingHerbavoreLevelMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingHerbavoreLevelMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingHerbavoreLevelMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingCarnivoreLevelMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingCarnivoreLevelMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingCarnivoreLevelMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingCarnivoreLevelMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingScavengerLevelMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingScavengerLevelMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingScavengerLevelMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingScavengerLevelMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingOmnivoreLevelMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingOmnivoreLevelMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingOmnivoreLevelMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingOmnivoreLevelMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingHotToleranceMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingHotToleranceMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingHotToleranceMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingHotToleranceMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingColdToleranceMin = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingColdToleranceMin).First().CurrentValue;
                    gameData.CreatureSettings.StartingColdToleranceMax = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingColdToleranceMax).First().CurrentValue;
                    gameData.CreatureSettings.StartingEnergy = (int)tabPanel.Tabs[1].Controls.Sliders.Where(t => t.SliderCode == EvolutionConquest.SettingEnum.StartingEnergy).First().CurrentValue;

                    gameData.MutationSettings.Attraction = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.Attraction).First().CurrentValue;
                    gameData.MutationSettings.Camo = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.Camo).First().CurrentValue;
                    gameData.MutationSettings.Carnivore = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.Carnivore).First().CurrentValue;
                    gameData.MutationSettings.ChanceToIncreaseValue = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.ChanceToIncreaseValue).First().CurrentValue;
                    gameData.MutationSettings.ChangeAmount = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.ChangeAmount).First().CurrentValue;
                    gameData.MutationSettings.MutationBonusPercent = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.MutationBonusPercent).First().CurrentValue;
                    gameData.MutationSettings.Cloning = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.Cloning).First().CurrentValue;
                    gameData.MutationSettings.ColdClimateTolerance = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.ColdClimateTolerance).First().CurrentValue;
                    gameData.MutationSettings.EggCamo = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.EggCamo).First().CurrentValue;
                    gameData.MutationSettings.EggIncubation = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.EggIncubation).First().CurrentValue;
                    gameData.MutationSettings.EggInterval = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.EggInterval).First().CurrentValue;
                    gameData.MutationSettings.EggToxicity = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.EggToxicity).First().CurrentValue;
                    gameData.MutationSettings.FoodType = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.FoodType).First().CurrentValue;
                    gameData.MutationSettings.FoodDigestion = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.FoodDigestion).First().CurrentValue;
                    gameData.MutationSettings.Herbavore = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.Herbavore).First().CurrentValue;
                    gameData.MutationSettings.HotClimateTolerance = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.HotClimateTolerance).First().CurrentValue;
                    gameData.MutationSettings.Lifespan = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.Lifespan).First().CurrentValue;
                    gameData.MutationSettings.Omnivore = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.Omnivore).First().CurrentValue;
                    gameData.MutationSettings.Scavenger = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.Scavenger).First().CurrentValue;
                    gameData.MutationSettings.Sight = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.Sight).First().CurrentValue;
                    gameData.MutationSettings.Speed = (int)tabPanel.Tabs[2].Controls.Sliders.Where(t => t.SliderCode == SettingEnum.Speed).First().CurrentValue;
                    
                    SettingsHelper.WriteSettings("Settings.json", gameData.Settings);
                    SettingsHelper.WriteCreatureSettings("CreatureSettings.json", gameData.CreatureSettings);
                    SettingsHelper.WriteMutationSettings("MutationSettings.json", gameData.MutationSettings);
                    gameData.ResetGame = true;
                    gameData.ShowSettingsPanel = false;
                }
                //Check for Close click
                else if (tabPanel.CloseButton.ButtonRectangle.Contains(inputState.CurrentMouseState.Position))
                {
                    gameData.ShowSettingsPanel = false;
                    gameData.BuildSettingsPanel = true;
                }
                else if (tabPanel.DefaultButton.ButtonRectangle.Contains(inputState.CurrentMouseState.Position))
                {
                    GameSettings tmp = gameData.Settings;
                    SettingsHelper.SetDefaultWorld(ref tmp);
                    gameData.Settings = tmp;

                    CreatureSettings tmpCreature = gameData.CreatureSettings;
                    SettingsHelper.SetDefaultCreature(ref tmpCreature);
                    gameData.CreatureSettings = tmpCreature;

                    MutationSettings tmpMutation = gameData.MutationSettings;
                    SettingsHelper.SetDefaultMutation(ref tmpMutation);
                    gameData.MutationSettings = tmpMutation;

                    SettingsHelper.WriteSettings("Settings.json", gameData.Settings);
                    SettingsHelper.WriteCreatureSettings("CreatureSettings.json", gameData.CreatureSettings);
                    SettingsHelper.WriteMutationSettings("MutationSettings.json", gameData.MutationSettings);
                    gameData.ResetGame = true;
                    gameData.ShowSettingsPanel = false;
                }

                    //Check slider interaction
                for (int i = 0; i < tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders.Count; i++)
                {
                    if (inputState.CurrentMouseState.Position.X >= tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].MarkerRectangle.Left && inputState.CurrentMouseState.Position.X <= tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].MarkerRectangle.Right
                        && inputState.CurrentMouseState.Position.Y >= tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].MarkerRectangle.Top && inputState.CurrentMouseState.Position.Y <= tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].MarkerRectangle.Bottom)
                    {
                        isSliderActive = true;
                        tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].SliderActive = true;
                        break;
                    }
                }
            }
            else
            {
                if (isSliderActive)
                {
                    for (int i = 0; i < tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders.Count; i++)
                    {
                        tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].SliderActive = false;
                    }
                    isSliderActive = false;
                }
            }
            if (isSliderActive)
            {
                for (int i = 0; i < tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders.Count; i++)
                {
                    if (tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].SliderActive)
                    {
                        tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].MarkerTexturePosition = new Vector2(inputState.CurrentMouseState.X, tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].MarkerTexturePosition.Y);

                        if (tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].SliderCode == EvolutionConquest.SettingEnum.WorldSize)
                        {
                            if (tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].CurrentValue % 50 != 0)
                            {
                                tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].CurrentValue = (float)(Math.Round(tabPanel.Tabs[tabPanel.ActiveTab].Controls.Sliders[i].CurrentValue / 50.0) * 50.0);
                            }
                        }

                        break;
                    }
                }
            }
        }
    }

    private float GetSettingValue(SettingEnum code, GameData gameData)
    {
        switch (code)
        {
            case SettingEnum.WorldSize:
                return gameData.Settings.WorldSize;
            case SettingEnum.ClimatePercent:
                return gameData.Settings.ClimateHeightPercent;
            case SettingEnum.StartingFoodRatio:
                return gameData.Settings.StartingFoodRatio;
            case SettingEnum.FoodGenerationValue:
                return gameData.Settings.FoodGenerationValue;
            case SettingEnum.SecondsUntilFoodUpgradesStart:
                return gameData.Settings.TicksUntilFoodUpgradeStarts;
            case SettingEnum.SecondsBetweenFoodUpgrades:
                return gameData.Settings.TicksBetweenFoodUpgrades;
            case SettingEnum.FoodUpgradeAmount:
                return gameData.Settings.FoodUpgradeAmount;
            case SettingEnum.FoodUpgradePercentChange:
                return gameData.Settings.FoodUpgradeChancePercent;
            case SettingEnum.MaxFoodlevel:
                return gameData.Settings.MaxFoodLevel;
            case SettingEnum.StartingCreatureRatio:
                return gameData.Settings.StartingCreatureRatio;
            case SettingEnum.EnergyGivenFromFood:
                return gameData.Settings.EnergyGivenFromFood;
            case SettingEnum.EnergyLossFromLayingEgg:
                return gameData.Settings.EnergyConsumptionFromLayingEgg;
            case SettingEnum.EnergyDepletionFromMovementRate:
                return gameData.Settings.EnergyDepletionFromMovement;
            case SettingEnum.EnergyDepletionPercentFromComplexity:
                return gameData.Settings.EnergyDepletionPercentFromComplexity;
            case SettingEnum.StartingEggIntervalMin:
                return gameData.CreatureSettings.StartingEggIntervalMin;
            case SettingEnum.StartingEggIntervalMax:
                return gameData.CreatureSettings.StartingEggIntervalMax;
            case SettingEnum.StartingEggIncubationMin:
                return gameData.CreatureSettings.StartingEggIncubationMin;
            case SettingEnum.StartingEggIncubationMax:
                return gameData.CreatureSettings.StartingEggIncubationMax;
            case SettingEnum.StartingFoodDigestionMin:
                return gameData.CreatureSettings.StartingFoodDigestionMin;
            case SettingEnum.StartingFoodDigestionMax:
                return gameData.CreatureSettings.StartingFoodDigestionMax;
            case SettingEnum.StartingSpeedMin:
                return gameData.CreatureSettings.StartingSpeedMin;
            case SettingEnum.StartingSpeedMax:
                return gameData.CreatureSettings.StartingSpeedMax;
            case SettingEnum.StartingLifespanMin:
                return gameData.CreatureSettings.StartingLifespanMin;
            case SettingEnum.StartingLifespanMax:
                return gameData.CreatureSettings.StartingLifespanMax;
            case SettingEnum.StartingHerbavoreLevelMin:
                return gameData.CreatureSettings.StartingHerbavoreLevelMin;
            case SettingEnum.StartingHerbavoreLevelMax:
                return gameData.CreatureSettings.StartingHerbavoreLevelMax;
            case SettingEnum.StartingCarnivoreLevelMin:
                return gameData.CreatureSettings.StartingCarnivoreLevelMin;
            case SettingEnum.StartingCarnivoreLevelMax:
                return gameData.CreatureSettings.StartingCarnivoreLevelMax;
            case SettingEnum.StartingScavengerLevelMin:
                return gameData.CreatureSettings.StartingScavengerLevelMin;
            case SettingEnum.StartingScavengerLevelMax:
                return gameData.CreatureSettings.StartingScavengerLevelMax;
            case SettingEnum.StartingOmnivoreLevelMin:
                return gameData.CreatureSettings.StartingOmnivoreLevelMin;
            case SettingEnum.StartingOmnivoreLevelMax:
                return gameData.CreatureSettings.StartingOmnivoreLevelMax;
            case SettingEnum.StartingColdToleranceMin:
                return gameData.CreatureSettings.StartingColdToleranceMin;
            case SettingEnum.StartingColdToleranceMax:
                return gameData.CreatureSettings.StartingColdToleranceMax;
            case SettingEnum.StartingHotToleranceMin:
                return gameData.CreatureSettings.StartingHotToleranceMin;
            case SettingEnum.StartingHotToleranceMax:
                return gameData.CreatureSettings.StartingHotToleranceMax;
            case SettingEnum.StartingEnergy:
                return gameData.CreatureSettings.StartingEnergy;
        }

        return 0f;
    }
}