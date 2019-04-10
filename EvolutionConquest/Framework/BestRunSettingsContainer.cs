using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BestRunSettingsContainer
{
    public double FitnessScore { get; set; }
    public GameSettings Settings { get; set; }
    public CreatureSettings CreatureSettings { get; set; }
    public MutationSettings MutationSettings { get; set; }
    public int MAX_UNDIGESTED_FOOD { get; set; }
    public int CARCASS_LIFESPAN { get; set; }
    public int INITIAL_SPAWN_FOOD_VARIANCE { get; set; }
    public int INITIAL_SPAWN_FOOD_AVG_LIFESPAN { get; set; }

    public BestRunSettingsContainer()
    {
        //Init this to a low value so that the first run the finishes with a somewhat decent score is the new baseline
        FitnessScore = -9999;
    }
}