﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Food : SpriteBase
{
    public float FoodStrength { get; set; } //Only creatures with a Herbavore level at or above this can eat this food. Simulates hard to eat food or hard to reach food
    public int FoodType { get; set; } //Carcass = -1, Blue = 0, Red = 1, Green = 2. Only Herbavores with their highest food type of this can eat this color of food
    public Color FoodColor { get; set; } //Based on the Type
    public string DisplayText { get; set; }
    public Vector2 TextSize { get; set; }
    public float Lifespan { get; set; } //How long the food lives
    public int ElapsedTicks { get; set; } //How many ticks the food has been alive for
    public bool MarkedForDelete { get; set; }

    public Food()
    {
        MarkedForDelete = false;
    }
}