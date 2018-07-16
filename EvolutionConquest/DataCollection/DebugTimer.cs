using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DebugTimer
{
    public double TimeUpdateTick { get; set; }
    public double TimeUpdateTickEggHatching { get; set; }
    public double TimeUpdateTickCreature { get; set; }
    public double TimeUpdateTickFood { get; set; }
    public double TimeUpdateTickPlant { get; set; }
    public double TimeUpdateOffTick { get; set; }
    public double TimeUpdateOffTickInterval { get; set; }
    public double TimeUpdateOffTickIntervalGraphs { get; set; }
    public double TimeUpdateOffTickIntervalMapStats { get; set; }
    public double TimeUpdateOffTickIntervalObjectCleanup { get; set; }
    public double TimeUpdateOffTickHandleCollisionsAndMovement { get; set; }
    public double TimeUpdateOffTickSpawnFood { get; set; }
    public double TimeUpdateOffTickEventCheckCleanup { get; set; }
    public double TimeUpdateHandleInputs { get; set; }
    public double TimeUpdateMoveCreature { get; set; }
    public double TimeDrawWorldObjects { get; set; }
    public double TimeDrawClimates { get; set; }
    public double TimeDrawCreatures { get; set; }
    public double TimeDrawEggs { get; set; }
    public double TimeDrawFood { get; set; }
    public double TimeDrawPlants { get; set; }
    public double TimeDrawBorders { get; set; }
    public double TimeDrawHighlightCreatures { get; set; }
    public double TimeDrawDebugData { get; set; }
    public double TimeDrawDebugDataForCreature { get; set; }
    public double TimeDrawDebugPanel { get; set; }
    public double TimeDrawHUD { get; set; }
    public double TimeDrawCreatureStatsOrEventLogPanel { get; set; }
    public double TimeDrawMapStatistics { get; set; }
    public double TimeDrawControlsPanel { get; set; }
    public double TimeDrawChartBorder { get; set; }
    public double TimeDrawSettingsPanel { get; set; }
    public double TimeDrawFPS { get; set; }
    public double TimeDrawMarkers { get; set; }
    public double TimeDrawDebugDataHUD { get; set; }
    public double TimeDrawPanelWithText { get; set; }

    public DebugTimer()
    {
        TimeUpdateTick = TimeUpdateTickEggHatching = TimeUpdateTickCreature = TimeUpdateTickFood = TimeUpdateTickPlant = TimeUpdateOffTick = TimeUpdateOffTickInterval = TimeUpdateOffTickIntervalGraphs = TimeUpdateOffTickIntervalMapStats = TimeUpdateOffTickIntervalObjectCleanup = TimeUpdateOffTickHandleCollisionsAndMovement = TimeUpdateOffTickSpawnFood = TimeUpdateOffTickEventCheckCleanup = TimeUpdateHandleInputs = TimeUpdateMoveCreature = TimeDrawWorldObjects = TimeDrawClimates = TimeDrawCreatures = TimeDrawEggs = TimeDrawFood = TimeDrawPlants = TimeDrawBorders = TimeDrawHighlightCreatures = TimeDrawDebugData = TimeDrawDebugDataForCreature = TimeDrawDebugPanel = TimeDrawHUD = TimeDrawCreatureStatsOrEventLogPanel = TimeDrawMapStatistics = TimeDrawControlsPanel = TimeDrawChartBorder = TimeDrawSettingsPanel = TimeDrawFPS = TimeDrawMarkers = TimeDrawDebugDataHUD = TimeDrawPanelWithText = 0;
    }
}