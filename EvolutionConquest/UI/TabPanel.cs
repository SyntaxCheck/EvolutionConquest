using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TabPanel
{
    public List<Tab> Tabs { get; set; }
    public int ActiveTab { get; set; }

    public TabPanel()
    {
        ActiveTab = 0;
    }
}