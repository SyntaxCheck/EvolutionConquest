using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Player
{
    public Player()
    {
    }

    public void HandleInput(InputState inputState, PlayerIndex? controllingPlayer, ref GameData gameData)
    {
        PlayerIndex playerIndex;

        if (inputState.IsNewKeyPress(Keys.F11, controllingPlayer, out playerIndex))
        {
            gameData.ShowChart = !gameData.ShowChart;
        }
        if (inputState.IsNewKeyPress(Keys.F12, controllingPlayer, out playerIndex))
        {
            gameData.ShowControls = !gameData.ShowControls;
        }
        if (inputState.IsNewKeyPress(Keys.F, controllingPlayer, out playerIndex))
        {
            gameData.HighlightSpecies = !gameData.HighlightSpecies;
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
        if (inputState.IsNewKeyPress(Keys.F3, controllingPlayer, out playerIndex))
        {
            gameData.EggMarkers = !gameData.EggMarkers;
        }
        if (inputState.IsNewKeyPress(Keys.F4, controllingPlayer, out playerIndex))
        {
            gameData.HerbavoreMarkers = !gameData.HerbavoreMarkers;
        }
        if (inputState.IsNewKeyPress(Keys.F5, controllingPlayer, out playerIndex))
        {
            gameData.CarnivoreMarkers = !gameData.CarnivoreMarkers;
        }
        if (inputState.IsNewKeyPress(Keys.F6, controllingPlayer, out playerIndex))
        {
            gameData.ScavengerMarkers = !gameData.ScavengerMarkers;
        }
        if (inputState.IsNewKeyPress(Keys.F7, controllingPlayer, out playerIndex))
        {
            gameData.OmnivoreMarkers = !gameData.OmnivoreMarkers;
        }
    }
}