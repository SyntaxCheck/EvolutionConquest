public class MapStatistics
{
    public int AliveCreatures { get; set; }
    public int DeadCreatures { get; set; }
    public int FoodOnMap { get; set; }
    public int EggsOnMap { get; set; }
    public int UniqueSpecies { get; set; }
    public double PercentHerbavore { get; set; }
    public double PercentCarnivore { get; set; }
    public double PercentScavenger { get; set; }
    public double PercentOmnivore { get; set; }

    public MapStatistics()
    { }
}