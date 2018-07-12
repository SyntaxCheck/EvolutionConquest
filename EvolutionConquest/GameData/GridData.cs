using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GridData
{
    public Rectangle CellRectangle { get; set; }
    public List<Creature> Creatures { get; set; }
    public List<Egg> Eggs { get; set; }
    public List<Food> Food { get; set; }
    public List<Plant> Plants { get; set; }

    public GridData()
    {
    }
}