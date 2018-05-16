using System.Collections.Generic;

public class CreatureStats
{
    public int NumberOfStats { get; set; } //Used when calculating the average
    public List<string> FieldHeaders { get; set; }
    public List<string> StringStats { get; set; }
    public List<int> IntStats { get; set; }
    public List<float> FloatStats { get; set; }

    public CreatureStats()
    {
        NumberOfStats = 1;
        FieldHeaders = new List<string>();
        StringStats = new List<string>();
        IntStats = new List<int>();
        FloatStats = new List<float>();
    }
}