using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Egg : SpriteBase
{
    public Creature Creature { get; set; }
    public int TicksTillHatched { get; set; }
    public int ElapsedTicks { get; set; }
    public float Camo { get; set; }

    public Egg()
    {
    }

    public void AdvanceTick()
    {
        ElapsedTicks++;
    }
}