using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CarcassShapeGenerator
{
    public CarcassShapeGenerator()
    {
    }

    public Texture2D CreateCarcassTexture(GraphicsDevice device)
    {
        Texture2D texture;
        int IMAGE_WIDTH = 12;
        int IMAGE_HEIGHT = 12;

        texture = new Texture2D(device, IMAGE_WIDTH, IMAGE_HEIGHT);
        Color[] colors = new Color[IMAGE_WIDTH * IMAGE_HEIGHT];

        Color BORDER = Color.Black;
        Color BONE = Color.White;

        List<Color> colorList = new List<Color>();
        //Layer1
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        //Layer2
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        //Layer3
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(Color.Black);
        //Layer4
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        //Layer5
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BORDER);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        //Layer6
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer7
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer8
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        //Layer9
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        //Layer10
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        //Layer11
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BONE);
        colorList.Add(BONE);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        //Layer12
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(BORDER);
        colorList.Add(BORDER);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);

        colors = colorList.ToArray();

        texture.SetData(colors);

        return texture;
    }
}