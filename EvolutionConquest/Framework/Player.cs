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
    }
}