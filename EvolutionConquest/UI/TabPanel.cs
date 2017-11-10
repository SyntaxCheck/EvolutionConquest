using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TabPanel
{
    public int PanelWidth { get; set; }
    public int PanelHeight { get; set; }
    public Point Position { get; set; }
    public List<Tab> Tabs { get; set; }
    public int ActiveTab { get; set; }
    public Button SaveButton { get; set; }
    public Button CloseButton { get; set; }
    public Button DefaultButton { get; set; }

    public TabPanel()
    {
        ActiveTab = 0;
    }
}