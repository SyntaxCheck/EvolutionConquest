using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class StatsThread
{
    private GameData _sharedGameData;
    private int NumberOfTimesChartDataUpdatedLast;
    private bool hasHeaderBeenWritten;

    public StatsThread(GameData sharedGameData)
    {
        NumberOfTimesChartDataUpdatedLast = 0;
        hasHeaderBeenWritten = false;
        _sharedGameData = sharedGameData;
    }

    public void Start()
    {
        CheckForNewStatsLoop();
    }

    private void CheckForNewStatsLoop()
    {
        while (true)
        {
            //When the main game graphs update then we should write a new record to the CSV file
            if (NumberOfTimesChartDataUpdatedLast < _sharedGameData.NumberOfTimesChartDataUpdated)
            {
                NumberOfTimesChartDataUpdatedLast = _sharedGameData.NumberOfTimesChartDataUpdated;
                List<CreatureStats> creatureStats = new List<CreatureStats>();
                List<CreatureStats> speciesStatsAvg = new List<CreatureStats>();
                List<CreatureStats> speciesStatsMin = new List<CreatureStats>();
                List<CreatureStats> speciesStatsMax = new List<CreatureStats>();

                foreach (Creature c in _sharedGameData.Creatures)
                {
                    CreatureStats cs = c.GetCreatureStatistics(_sharedGameData.GameSeed, _sharedGameData.SessionID, _sharedGameData.TotalElapsedSeconds);
                    creatureStats.Add(cs);

                    //Add AVG stats
                    bool found = false;
                    for (int i = 0; i < speciesStatsAvg.Count; i++)
                    {
                        if (speciesStatsAvg[i].IntStats[0] == c.SpeciesId)
                        {
                            speciesStatsAvg[i].NumberOfStats++;

                            for (int k = 0; k < speciesStatsAvg[i].IntStats.Count; k++)
                            {
                                speciesStatsAvg[i].IntStats[k] += cs.IntStats[k];
                            }
                            for (int k = 0; k < speciesStatsAvg[i].FloatStats.Count; k++)
                            {
                                speciesStatsAvg[i].FloatStats[k] += cs.FloatStats[k];
                            }

                            break;
                        }
                    }

                    if (!found)
                    {
                        speciesStatsAvg.Add(cs);
                    }
                }

                //Divide the stats to get the average
                for (int i = 0; i < speciesStatsAvg.Count; i++)
                {
                    for (int k = 0; k < speciesStatsAvg[i].IntStats.Count; k++)
                    {
                        speciesStatsAvg[i].IntStats[k] = speciesStatsAvg[i].IntStats[k] / speciesStatsAvg[i].NumberOfStats;
                    }
                    for (int k = 0; k < speciesStatsAvg[i].FloatStats.Count; k++)
                    {
                        speciesStatsAvg[i].FloatStats[k] = speciesStatsAvg[i].FloatStats[k] / speciesStatsAvg[i].NumberOfStats;
                    }
                }

                //Write the Headers
                if (!hasHeaderBeenWritten)
                {
                    hasHeaderBeenWritten = true;

                    //System.IO.File.AppendAllLines(System.IO.Path.Combine(_sharedGameData.SessionID.ToString(), "Settings.csv"));
                }

                //System.Windows.Forms.MessageBox.Show(NumberOfTimesChartDataUpdatedLast.ToString());
            }

            Thread.Sleep(100);
        }
    }
}