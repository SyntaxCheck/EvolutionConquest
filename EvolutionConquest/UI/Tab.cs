using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Tab
{
    public int TabNumber { get; set; }
    public Texture2D TabImage { get; set; }
    public string TabText { get; set; }
    public UIControls Controls { get; set; }
    public Rectangle ButtonRectangle { get; set; }

    public Tab()
    {
    }
}