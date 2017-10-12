using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CreatureShapeGenerator
{
    public CreatureShapeGenerator()
    {
    }

    public Texture2D CreateCreatureScavengerTexture(GraphicsDevice device, bool sight)
    {
        Texture2D texture;
        int IMAGE_WIDTH = 12;
        int IMAGE_HEIGHT = 12;

        texture = new Texture2D(device, IMAGE_WIDTH, IMAGE_HEIGHT);
        Color[] colors = new Color[IMAGE_WIDTH * IMAGE_HEIGHT];

        Color SPINE = Color.Black;
        Color EYES = Color.White;
        if (sight)
            EYES = Color.Blue;

        List<Color> colorList = new List<Color>();
        //Layer1
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer2
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer3
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer4
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer5
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(SPINE);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer6
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(SPINE);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer7
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(SPINE);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer8
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(SPINE);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer9
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(SPINE);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer10
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer11
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer12
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);

        colors = colorList.ToArray();

        texture.SetData(colors);

        return texture;
    }
    public Texture2D CreateCreatureCarnivoreTexture(GraphicsDevice device, bool sight)
    {
        Texture2D texture;
        int IMAGE_WIDTH = 12;
        int IMAGE_HEIGHT = 12;

        texture = new Texture2D(device, IMAGE_WIDTH, IMAGE_HEIGHT);
        Color[] colors = new Color[IMAGE_WIDTH * IMAGE_HEIGHT];

        Color EYES = Color.White;
        if (sight)
            EYES = Color.Blue;

        List<Color> colorList = new List<Color>();
        //Layer1
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer2
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer3
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer4
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer5
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer6
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer7
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer8
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer9
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer10
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer11
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer12
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);

        colors = colorList.ToArray();

        texture.SetData(colors);

        return texture;
    }
    public Texture2D CreateCreatureHerbavoreTexture(GraphicsDevice device, bool sight)
    {
        Texture2D texture;
        int IMAGE_WIDTH = 12;
        int IMAGE_HEIGHT = 12;

        texture = new Texture2D(device, IMAGE_WIDTH, IMAGE_HEIGHT);
        Color[] colors = new Color[IMAGE_WIDTH * IMAGE_HEIGHT];

        Color EYES = Color.White;
        if (sight)
            EYES = Color.Blue;

        List<Color> colorList = new List<Color>();
        //Layer1
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer2
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer3
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer4
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(EYES);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(EYES);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer5
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer6
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer7
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer8
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer9
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer10
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer11
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer12
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);

        colors = colorList.ToArray();

        texture.SetData(colors);

        return texture;
    }
    public Texture2D CreateCreatureOmnivoreTexture(GraphicsDevice device, bool sight)
    {
        Texture2D texture;
        int IMAGE_WIDTH = 12;
        int IMAGE_HEIGHT = 12;

        texture = new Texture2D(device, IMAGE_WIDTH, IMAGE_HEIGHT);
        Color[] colors = new Color[IMAGE_WIDTH * IMAGE_HEIGHT];

        Color EYES = Color.White;
        if (sight)
            EYES = Color.Blue;

        List<Color> colorList = new List<Color>();
        //Layer1
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer2
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer3
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer4
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer5
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer6
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(EYES);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer7
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer8
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer9
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer10
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer11
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer12
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);

        colors = colorList.ToArray();

        texture.SetData(colors);

        return texture;
    }
}