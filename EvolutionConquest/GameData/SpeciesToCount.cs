using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpeciesToCount
{
    private string creatureType;

    public Random Rand { get; set; }
    public string Name { get; set; }
    public int Id { get; set; }
    public string CreatureType
    {
        get
        {
            return creatureType;
        }
        set
        {
            creatureType = value;
            if (creatureType.Substring(0, 1) == "H")
            {
                //seriesColor = System.Drawing.Color.Green;
                ChartColor = System.Drawing.Color.FromArgb(Rand.Next(0, 50), Rand.Next(100, 255), Rand.Next(0, 50));
            }
            else if (creatureType.Substring(0, 1) == "C")
            {
                //seriesColor = System.Drawing.Color.Red;
                ChartColor = System.Drawing.Color.FromArgb(Rand.Next(175, 255), Rand.Next(0, 50), Rand.Next(0, 50));
            }
            else if (creatureType.Substring(0, 1) == "S")
            {
                //seriesColor = System.Drawing.Color.Purple;
                ChartColor = System.Drawing.Color.FromArgb(Rand.Next(0, 50), Rand.Next(0, 50), Rand.Next(150, 255));
            }
            else if (creatureType.Substring(0, 1) == "O")
            {
                //seriesColor = System.Drawing.Color.Yellow;
                ChartColor = System.Drawing.Color.FromArgb(Rand.Next(225, 255), Rand.Next(200, 230), Rand.Next(0, 100));
            }
        }
    }
    public List<int> CountsOverTime { get; set; }
    public System.Drawing.Color ChartColor { get; set; }

    public SpeciesToCount()
    {
        CountsOverTime = new List<int>();
    }
}