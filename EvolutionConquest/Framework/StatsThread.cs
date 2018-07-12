using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class StatsThread
{
    private const string CSV_NAME = "StatsOverTime.csv";
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
            try
            {
                //When the main game graphs update then we should write a new record to the CSV file
                if (NumberOfTimesChartDataUpdatedLast < _sharedGameData.NumberOfTimesChartDataUpdated)
                {
                    NumberOfTimesChartDataUpdatedLast = _sharedGameData.NumberOfTimesChartDataUpdated;
                    List<CreatureStats> creatureStats = new List<CreatureStats>();
                    List<CreatureStats> speciesStatsAvg = new List<CreatureStats>();
                    List<CreatureStats> speciesStatsMin = new List<CreatureStats>();
                    List<CreatureStats> speciesStatsMax = new List<CreatureStats>();

                    //Build a deep copy of the array to avoid errors reading the list while creatures are being added/removed
                    List<Creature> deepCopy = new List<Creature>(_sharedGameData.Creatures.ToArray());

                    foreach (Creature c in deepCopy)
                    {
                        CreatureStats cs = c.GetCreatureStatistics(_sharedGameData.GameSeed, _sharedGameData.SessionID, _sharedGameData.TotalElapsedSeconds);
                        creatureStats.Add(cs);

                        //Add AVG stats
                        bool found = false;
                        for (int i = 0; i < speciesStatsAvg.Count; i++)
                        {
                            if (speciesStatsAvg[i].IntStats[0] == c.SpeciesId)
                            {
                                found = true;
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

                    //(*&@$#%(*&$#(&*%(*#&$%
                    //Headers and ROWs not matching number of columns

                    if (speciesStatsAvg.Count > 0)
                    {
                        string headers = String.Empty;

                        for (int i = 0; i < speciesStatsAvg.Count; i++)
                        {
                            string csvRow = String.Empty;

                            headers += speciesStatsAvg[i].FieldHeaders[0] + ","; //Seed
                            headers += speciesStatsAvg[i].FieldHeaders[1] + ","; //Session ID
                            headers += speciesStatsAvg[i].FieldHeaders[2] + ","; //Game Minutes
                            headers += speciesStatsAvg[i].FieldHeaders[3] + ","; //Species
                            headers += speciesStatsAvg[i].FieldHeaders[7] + ","; //Creature Type
                            csvRow += speciesStatsAvg[i].StringStats[0] + ",";
                            csvRow += speciesStatsAvg[i].StringStats[1] + ",";
                            csvRow += speciesStatsAvg[i].StringStats[2] + ",";
                            csvRow += speciesStatsAvg[i].StringStats[3] + ",";
                            csvRow += speciesStatsAvg[i].StringStats[7] + ",";

                            //Add the Int fields
                            int headerIndex = speciesStatsAvg[i].StringStats.Count; //The headers were inserted in String,Int,Float list order
                            for (int k = 0; k < speciesStatsAvg[i].IntStats.Count; k++)
                            {
                                headers += speciesStatsAvg[i].FieldHeaders[k + headerIndex] + "_AVG,";
                                csvRow += speciesStatsAvg[i].IntStats[k] + ",";
                            }

                            //Add the float fields
                            headerIndex = (speciesStatsAvg[i].StringStats.Count - 1) + (speciesStatsAvg[i].IntStats.Count - 1); //The headers were inserted in String,Int,Float list order
                            for (int k = 0; k < speciesStatsAvg[i].IntStats.Count; k++)
                            {
                                headers += speciesStatsAvg[i].FieldHeaders[k + headerIndex] + "_AVG,";
                                csvRow += speciesStatsAvg[i].FloatStats[k] + ",";
                            }

                            if (!String.IsNullOrEmpty(headers))
                            {
                                headers = headers.Substring(0, headers.Length - 1);
                            }
                            if (!String.IsNullOrEmpty(csvRow))
                            {
                                csvRow = csvRow.Substring(0, csvRow.Length - 1);
                            }

                            //Write the Headers
                            if (!hasHeaderBeenWritten)
                            {
                                hasHeaderBeenWritten = true;

                                System.IO.File.AppendAllLines(System.IO.Path.Combine(_sharedGameData.SessionID.ToString(), CSV_NAME), new List<string> { headers });
                            }
                            System.IO.File.AppendAllLines(System.IO.Path.Combine(_sharedGameData.SessionID.ToString(), CSV_NAME), new List<string> { csvRow });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(System.IO.Path.Combine(_sharedGameData.SessionID.ToString(), "ErrorLogStats.txt"), "STATS Uncaught error: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
            }
            Thread.Sleep(100);
        }
    }
}