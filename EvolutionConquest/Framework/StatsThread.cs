using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

public class StatsThread
{
    private const string CSV_NAME = "StatsOverTime.csv";
    private GameData _gameData;
    private Random _rand;
    private Chart _chart;
    private int NumberOfTimesChartDataUpdatedLast;
    private bool hasHeaderBeenWritten;

    public StatsThread(GameData sharedGameData, Random sharedRand)
    {
        NumberOfTimesChartDataUpdatedLast = 0;
        hasHeaderBeenWritten = false;
        _gameData = sharedGameData;
        _rand = sharedRand;

        //Create the chart
        _chart = new Chart();
        _chart.Width = 600;
        _chart.Height = 300;
        _chart.Text = "Test";
        _chart.Visible = false;
        ChartArea chartArea1 = new ChartArea();
        chartArea1.Name = "ChartArea1";
        _chart.ChartAreas.Add(chartArea1);
        Legend legend = new Legend();
        _chart.Legends.Add(legend);
    }

    public void Start()
    {
        CheckForNewStatsLoop();
    }

    private void CheckForNewStatsLoop()
    {
        while (true)
        {
            DateTime startTime = DateTime.Now;

            try
            {
                _gameData.LockChart.Locker = "StatsThread";
                lock (_gameData.LockChart)
                {
                    UpdateChart();
                    UpdateGraph();
                    using (var chartimage = new MemoryStream())
                    {
                        System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(_chart.Width, _chart.Height);
                        _chart.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0,0,bitmap.Width,bitmap.Height));
                        bitmap.Save("Test.bmp");
                    }
                }
                _gameData.LockChart.Locker = "";

                //When the main game graphs update then we should write a new record to the CSV file
                if (NumberOfTimesChartDataUpdatedLast < _gameData.NumberOfTimesChartDataUpdated)
                {
                    NumberOfTimesChartDataUpdatedLast = _gameData.NumberOfTimesChartDataUpdated;
                    List<CreatureStats> creatureStats = new List<CreatureStats>();
                    List<CreatureStats> speciesStatsAvg = new List<CreatureStats>();
                    List<CreatureStats> speciesStatsMin = new List<CreatureStats>();
                    List<CreatureStats> speciesStatsMax = new List<CreatureStats>();

                    //Build a deep copy of the array to avoid errors reading the list while creatures are being added/removed
                    List<Creature> deepCopy = new List<Creature>(_gameData.Creatures.ToArray());

                    foreach (Creature c in deepCopy)
                    {
                        CreatureStats cs = c.GetCreatureStatistics(_gameData.GameSeed, _gameData.SessionID, _gameData.TotalElapsedSeconds);
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

                                System.IO.File.AppendAllLines(System.IO.Path.Combine(_gameData.SessionID.ToString(), CSV_NAME), new List<string> { headers });
                            }
                            System.IO.File.AppendAllLines(System.IO.Path.Combine(_gameData.SessionID.ToString(), CSV_NAME), new List<string> { csvRow });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(_gameData.SessionID.ToString(), "ErrorLogStats.txt"), DateTime.Now.ToString() + " - STATS Uncaught error: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
            }
            finally
            {
                int sleepAmount = 5000 - (int)(DateTime.Now - startTime).TotalMilliseconds;

                if (sleepAmount <= 0)
                    sleepAmount = 0;

                Thread.Sleep(sleepAmount);
            }
        }
    }
    private void UpdateChart()
    {
        //Generate Graph data
        _gameData.CalculateChartData(_rand); //This will populat the Chart Data in _gameData. Even if we hide the chart we need to keep track of ChartData
    }
    private void UpdateGraph()
    {
        if (_gameData.ShowChart)
        {
            _gameData.LockChart.Locker = "Main";
            lock (_gameData.LockChart)
            {
                if (!_chart.Visible && _gameData.ChartDataTop.Count > 0 && !_gameData.ShowSettingsPanel)
                {
                    _chart.Visible = true;
                }
                if (_chart.Series != null)
                {
                    _chart.Series.Clear();
                    for (int i = 0; i < _gameData.ChartDataTop.Count; i++)
                    {
                        int? count = _gameData.ChartDataTop[i].CountsOverTime[_gameData.ChartDataTop[i].CountsOverTime.Count - 1];
                        string name = String.Empty;
                        //System.Drawing.Color seriesColor = System.Drawing.Color.White;

                        name = _gameData.ChartDataTop[i].Name;

                        if (name.Length > 15)
                            name = name.Substring(0, 12) + "...";

                        if (count != null)
                        {
                            name += "(" + count + ")";
                        }
                        if (!String.IsNullOrEmpty(_gameData.ChartDataTop[i].CreatureType))
                        {
                            name = "(" + _gameData.ChartDataTop[i].CreatureType.Substring(0, 1) + ")" + name;
                        }

                        _chart.Series.Add(name);
                        _chart.Series[name].XValueType = ChartValueType.Int32;
                        _chart.Series[name].ChartType = SeriesChartType.StackedArea100;
                        _chart.Series[name].BorderWidth = 3;
                        if (_gameData.ChartDataTop[i].ChartColor != System.Drawing.Color.White)
                        {
                            _chart.Series[name].Color = _gameData.ChartDataTop[i].ChartColor;
                        }

                        for (int k = 0; k < _gameData.ChartDataTop[i].CountsOverTime.Count; k++)
                        {
                            _chart.Series[name].Points.AddXY(k, _gameData.ChartDataTop[i].CountsOverTime[k]);
                        }
                    }
                }
            }
            _gameData.LockChart.Locker = "";
        }

        _gameData.GameChart = _chart;
    }
}