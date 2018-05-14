using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FoodShapeGenerator
{
    public FoodShapeGenerator()
    {
    }

    public Texture2D CreateFoodTexture(GraphicsDevice device)
    {
        Texture2D texture;
        int IMAGE_WIDTH = 4;
        int IMAGE_HEIGHT = 4;

        texture = new Texture2D(device, IMAGE_WIDTH, IMAGE_HEIGHT);
        Color[] colors = new Color[IMAGE_WIDTH * IMAGE_HEIGHT];

        //Color foodColor = Color.DarkGreen;
        Color foodColor = Color.White; //Color will be determined at draw time based on food type

        List<Color> colorList = new List<Color>();
        //Layer1
        colorList.Add(Color.Transparent);
        colorList.Add(foodColor);
        colorList.Add(foodColor);
        colorList.Add(Color.Transparent);
        //Layer2
        colorList.Add(foodColor);
        colorList.Add(foodColor);
        colorList.Add(foodColor);
        colorList.Add(foodColor);
        //Layer3
        colorList.Add(foodColor);
        colorList.Add(foodColor);
        colorList.Add(foodColor);
        colorList.Add(foodColor);
        //Layer4
        colorList.Add(Color.Transparent);
        colorList.Add(foodColor);
        colorList.Add(foodColor);
        colorList.Add(Color.Transparent);
        ////Layer1
        //colorList.Add(Color.Transparent);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(Color.Transparent);
        ////Layer2
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        ////Layer3
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        ////Layer4
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        ////Layer5
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        ////Layer6
        //colorList.Add(Color.Transparent);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(foodColor);
        //colorList.Add(Color.Transparent);

        colors = colorList.ToArray();

        texture.SetData(colors);

        return texture;
    }
}