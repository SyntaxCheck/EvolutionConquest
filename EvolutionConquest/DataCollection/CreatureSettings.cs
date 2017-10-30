using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CreatureSettings
{
    public float StartingEggIntervalMin { get; set; }
    public float StartingEggIntervalMax { get; set; }
    public float StartingEggIncubationMin { get; set; }
    public float StartingEggIncubationMax { get; set; }
    public float StartingFoodDigestionMin { get; set; }
    public float StartingFoodDigestionMax { get; set; }
    public float StartingSpeedMin { get; set; }
    public float StartingSpeedMax { get; set; }
    public float StartingLifespanMin { get; set; }
    public float StartingLifespanMax { get; set; }
    public float StartingHerbavoreLevelMin { get; set; }
    public float StartingHerbavoreLevelMax { get; set; }
    public float StartingCarnivoreLevelMin { get; set; }
    public float StartingCarnivoreLevelMax { get; set; }
    public float StartingScavengerLevelMin { get; set; }
    public float StartingScavengerLevelMax { get; set; }
    public float StartingOmnivoreLevelMin { get; set; }
    public float StartingOmnivoreLevelMax { get; set; }
    public float StartingColdToleranceMin { get; set; }
    public float StartingColdToleranceMax { get; set; }
    public float StartingHotToleranceMin { get; set; }
    public float StartingHotToleranceMax { get; set; }
    public float StartingEnergy { get; set; }

    public CreatureSettings()
    {
    }
}