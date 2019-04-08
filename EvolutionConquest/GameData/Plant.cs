using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Plant : SpriteBase
{
    private const int PLANT_SPREAD_RADIUS_MIN = 3;
    private const int PLANT_SPREAD_RADIUS_MAX = 10;

    private int[] RotationVals = { 0, 90, 180, 270 };
    private bool _markForDelete;
    private bool _superTree;
    private bool _disableGrowOnEatInterval;
    private float _lifespan;
    private int _eatCooldownTicks;
    private int _spreadCooldownTicks;
    private int _growCooldownTicks;
    private int _growDelayOnEatTicks;

    public List<TextureContainer> TexturesList { get; set; }
    public string CurrentTexture;
    public float Lifespan
    {
        get
        {
            return _lifespan;
        }
        set
        {
            _lifespan = value;
            LifespanActual = _lifespan / (30 / TicksPerSecond);
        }
    } //How long the plant lives
    public float LifespanActual { get; set; } //How long the plant lives adjusted for tick rate
    public float FoodAmount { get; set; } //Amount of food that this plant currently has
    public float FoodAmountCap { get; set; } //Maximum amount of food this plant can hold
    public float FoodAmountGivenOnGrow { get; set; } //Amount of food to add when the plant grows
    public float FoodAmountGivenOnEat { get; set; } //Amount of food a creature gets when they eat the plant
    public float FoodStrength { get; set; } //Only creatures with a Herbavore level at or above this can eat this food. Simulates hard to eat food or hard to reach food
    public int NumberOfSaplings { get; set; } //How many baby plants to create when spreading
    public int EatCooldownTicks
    {
        get
        {
            return _eatCooldownTicks;
        }
        set
        {
            _eatCooldownTicks = value;
            EatCooldownTicksActual = _eatCooldownTicks / (30 / TicksPerSecond);
        }
    } //How long of a cooldown before a creature can eat from the plant again
    public int EatCooldownTicksActual { get; set; } //How long of a cooldown before a creature can eat from the plant again adjusted for tick rate
    public int SpreadCooldownTicks
    {
        get
        {
            return _spreadCooldownTicks;
        }
        set
        {
            _spreadCooldownTicks = value;
            SpreadCooldownTicksActual = _spreadCooldownTicks / (30 / TicksPerSecond);
        }
    } //How long to wait before spreading new plants
    public int SpreadCooldownTicksActual { get; set; } //How long to wait before spreading new plants adjusted for tick rate
    public int GrowCooldownTicks
    {
        get
        {
            return _growCooldownTicks;
        }
        set
        {
            _growCooldownTicks = value;
            GrowCooldownTicksActual = _growCooldownTicks / (30 / TicksPerSecond);
        }
    } //How long to wait before growing the plant
    public int GrowCooldownTicksActual { get; set; } //How long to wait before growing the plant adjusted for tick rate
    public int GrowDelayOnEatTicks
    {
        get
        {
            return _growDelayOnEatTicks;
        }
        set
        {
            _growDelayOnEatTicks = value;
            GrowDelayOnEatTicksActual = _growDelayOnEatTicks / (30 / TicksPerSecond);
        }
    } //How long of a delay to apply when a plant is eaten before it can spread
    public int GrowDelayOnEatTicksActual { get; set; } //How long of a delay to apply when a plant is eaten before it can spread adjusted for tick rate
    public int FoodType { get; set; } //Blue = 0, Red = 1, Green = 2. Only Herbavores with their highest food type of this can eat this color of food
    public int TicksSinceBirth { get; set; } //Used to calculate tree plant
    public int TicksSinceLastEaten { get; set; } //Used for determining when to grow/spread
    public int TicksSinceLastGrow { get; set; } //Used for determining when to grow
    public int TicksSinceLastSpread { get; set; } //Used for determining when to spread/seed new plants
    public int TotalTimesEatenFrom { get; set; } //Statistical purposes
    public bool FoodHasBeenEatenSinceLastTick { get; set; }
    public bool AllowedToSpread { get; set; } //When set to TRUE the plant can enter spread logic. If false the are is too saturated with plants
    public bool SpreadPlant { get; set; } //When set to TRUE main game logic will spread new plants in the update event
    public bool SuperTree
    {
        get
        {
            return _superTree;
        }
        set
        {
            _superTree = value;
            if (_superTree)
            {
                LifespanActual = LifespanActual * 100;
            }
        }
    } //Super trees has 100x lifespan
    public List<PlantCreatureInteraction> Interactions { get; set; }
    public string DisplayText { get; set; } //Text showing how much food is available
    public Vector2 TextSize { get; set; }
    public float Rotation { get; set; } //How the plant is facing: 0, 90, 180, 270
    public List<Point> ExpandedGridPositions { get; set; }
    public List<Vector2> SaplingSpawnPoints { get; set; }
    public int PlantSpreadNorthCheckRadius { get; set; }
    public int PlantSpreadSouthCheckRadius { get; set; }
    public int PlantSpreadEastCheckRadius { get; set; }
    public int PlantSpreadWestCheckRadius { get; set; }
    public bool NoVerticalSaplingPreference { get; set; } //Saplings can be either south or north of the current plant
    public bool NoHorizontalSaplingPreference { get; set; } //Saplings can be either east or west of the current plant
    public bool PreferSaplingNorth { get; set; } //Do saplings prefer north
    public bool PreferSaplingWest { get; set; } //Do saplings prefer west
    public bool MarkForDelete
    {
        get
        {
            return _markForDelete;
        }
        set
        {
            _markForDelete = value;
            DrawObject = !value;
        }
    }

    public Plant()
    {
        CurrentTexture = String.Empty;
        ExpandedGridPositions = new List<Point>();
        SaplingSpawnPoints = new List<Vector2>();
        TexturesList = new List<TextureContainer>();
        FoodHasBeenEatenSinceLastTick = false;
        SpreadPlant = false;
        AllowedToSpread = true;
        NoVerticalSaplingPreference = true;
        NoHorizontalSaplingPreference = true;
        PreferSaplingNorth = false;
        PreferSaplingWest = false;
        MarkForDelete = false;
        SuperTree = false;
        TicksSinceBirth = TicksSinceLastEaten = TicksSinceLastGrow = TicksSinceLastSpread = TotalTimesEatenFrom = 0;
        Interactions = new List<PlantCreatureInteraction>();
    }

    public float Eat(int creatureID)
    {
        Interactions.Add(new PlantCreatureInteraction() { CreatureID = creatureID, ElapsedTicks = 0 });

        if (FoodAmount > 0)
        {
            TotalTimesEatenFrom++;
            FoodAmount -= 1; //Deplete by 1 even though we dont award a full point
            if (FoodAmount < 0)
                FoodAmount = 0;
        }

        FoodHasBeenEatenSinceLastTick = true;

        return FoodAmount <= 0 ? 0 : FoodAmountGivenOnEat;
    }
    public void InitializeNewPlant(Random rand, List<TextureContainer> textureList, GameData gameData)
    {
        TicksPerSecond = gameData.TicksPerSecond;
        TexturesList = textureList;
        CurrentTexture = "T1";
        Texture = TexturesList.First(t => t.Name == CurrentTexture).Texture;
        Lifespan = rand.Next(4750, 6000);
        FoodAmount = 5;
        FoodAmountCap = rand.Next(100, 500);
        FoodAmountGivenOnGrow = rand.Next(1, 3);
        FoodAmountGivenOnEat = 0.05f;
        FoodStrength = 0;
        EatCooldownTicks = rand.Next(100, 200);
        NumberOfSaplings = rand.Next(0, 2);
        //SpreadCooldownTicks = rand.Next(2000, 2200);
        SpreadCooldownTicks = rand.Next(1600, 1600);
        GrowCooldownTicks = rand.Next(350, 450);
        GrowDelayOnEatTicks = rand.Next(50, 60);
        FoodType = 0; //NOT IMPLEMENTED
        _disableGrowOnEatInterval = gameData.DISABLE_GROW_ON_EAT_INTERVAL;
        Rotation = (float)(Math.PI / 180) * RotationVals[rand.Next(0, 4)];
        PlantSpreadNorthCheckRadius = rand.Next(PLANT_SPREAD_RADIUS_MIN, PLANT_SPREAD_RADIUS_MAX);
        PlantSpreadSouthCheckRadius = rand.Next(PLANT_SPREAD_RADIUS_MIN, PLANT_SPREAD_RADIUS_MAX);
        PlantSpreadEastCheckRadius = rand.Next(PLANT_SPREAD_RADIUS_MIN, PLANT_SPREAD_RADIUS_MAX);
        PlantSpreadWestCheckRadius = rand.Next(PLANT_SPREAD_RADIUS_MIN, PLANT_SPREAD_RADIUS_MAX);
        SuperTree = (rand.Next(0, 100) > 75); //75% chance for initial trees to be a super tree
        if (SuperTree)
        {
            CurrentTexture = "TS1";
            Texture = TexturesList.First(t => t.Name == CurrentTexture).Texture;
            if (NumberOfSaplings == 0)
                NumberOfSaplings = 1;
        }
    }
    public void AdvanceTick(Random rand)
    {
        TicksSinceBirth++;
        TicksSinceLastGrow++;
        TicksSinceLastSpread++;
        TicksSinceLastEaten++;

        for (int i = Interactions.Count - 1; i >= 0; i--)
        {
            Interactions[i].ElapsedTicks++;
        }

        if (FoodHasBeenEatenSinceLastTick)
        {
            FoodHasBeenEatenSinceLastTick = false;
            TicksSinceLastEaten = 0;
        }

        if (TicksSinceLastGrow >= GrowCooldownTicksActual && (_disableGrowOnEatInterval || TicksSinceLastEaten >= GrowDelayOnEatTicksActual))
        {
            TicksSinceLastGrow = 0;
            FoodAmount += FoodAmountGivenOnGrow;

            if (FoodAmount > FoodAmountCap)
                FoodAmount = FoodAmountCap;
        }

        if (AllowedToSpread && TicksSinceLastSpread >= SpreadCooldownTicksActual)
        {
            TicksSinceLastSpread = 0;
            SpreadPlant = true;
        }

        DisplayText = Math.Round(FoodAmount,0).ToString() + "/" + Math.Round(FoodAmountCap,0).ToString() + " (" + FoodStrength.ToString() + ")";
    }
    public Plant GetBabyPlant(Random rand)
    {
        Plant babyPlant = new Plant();

        babyPlant.WorldSize = WorldSize;
        babyPlant.ClimateHeightPercent = ClimateHeightPercent;
        babyPlant.TicksPerSecond = TicksPerSecond;
        babyPlant.TexturesList = TexturesList;
        babyPlant.CurrentTexture = "T0";
        babyPlant.Texture = TexturesList.First(t => t.Name == babyPlant.CurrentTexture).Texture;
        babyPlant.Lifespan = Lifespan + Mutate(rand, FoodAmountCap, 50);
        babyPlant.FoodAmount = 0;
        babyPlant.FoodAmountCap = FoodAmountCap + Mutate(rand, FoodAmountCap, 2);
        babyPlant.FoodAmountGivenOnGrow = FoodAmountGivenOnGrow + Mutate(rand, FoodAmountGivenOnGrow, 0.5f);
        babyPlant.FoodAmountGivenOnEat = FoodAmountGivenOnEat + Mutate(rand, FoodAmountGivenOnEat, 0.05f);
        babyPlant.FoodStrength = FoodStrength;
        babyPlant.EatCooldownTicks = EatCooldownTicks + (int)Math.Round(Mutate(rand, EatCooldownTicks, 10f), 0);
        babyPlant.SpreadCooldownTicks = SpreadCooldownTicks + (int)Math.Round(Mutate(rand, SpreadCooldownTicks, 10f), 0);
        babyPlant.GrowCooldownTicks = GrowCooldownTicks + (int)Math.Round(Mutate(rand, GrowCooldownTicks, 10f), 0);
        babyPlant.GrowDelayOnEatTicks = GrowDelayOnEatTicks + (int)Math.Round(Mutate(rand, GrowDelayOnEatTicks, 2f), 0);
        babyPlant.FoodType = FoodType;
        babyPlant.Rotation = (float)(Math.PI / 180) * RotationVals[rand.Next(0, 4)];
        babyPlant.NumberOfSaplings = rand.Next(0, 2);
        babyPlant.PlantSpreadNorthCheckRadius = PlantSpreadNorthCheckRadius + MutateSpreadRadius(rand, PlantSpreadNorthCheckRadius, 1, 10);
        babyPlant.PlantSpreadSouthCheckRadius = PlantSpreadSouthCheckRadius + MutateSpreadRadius(rand, PlantSpreadSouthCheckRadius, 1, 10);
        babyPlant.PlantSpreadEastCheckRadius = PlantSpreadEastCheckRadius + MutateSpreadRadius(rand, PlantSpreadEastCheckRadius, 1, 10);
        babyPlant.PlantSpreadWestCheckRadius = PlantSpreadWestCheckRadius + MutateSpreadRadius(rand, PlantSpreadWestCheckRadius, 1, 10);
        babyPlant.SuperTree = (rand.Next(0, 10000) > 9990); //0.001% chance for a new SuperTree baby
        if (babyPlant.SuperTree)
        {
            babyPlant.CurrentTexture = "TS0";
            babyPlant.Texture = TexturesList.First(t => t.Name == babyPlant.CurrentTexture).Texture;
            if (babyPlant.NumberOfSaplings == 0)
                babyPlant.NumberOfSaplings = 1;
        }

        return babyPlant;
    }
    public void GetExpandedGridPositions(GameData gameData)
    {
        for (int i = 0; i < GridPositions.Count(); i++)
        {
            ExpandedGridPositions.Add(GridPositions[i]);

            Point left = new Point(GridPositions[i].X - 1, GridPositions[i].Y);
            Point right = new Point(GridPositions[i].X + 1, GridPositions[i].Y);
            Point top = new Point(GridPositions[i].X, GridPositions[i].Y - 1);
            Point bottom = new Point(GridPositions[i].X, GridPositions[i].Y + 1);
            Point topLeft = new Point(GridPositions[i].X - 1, GridPositions[i].Y - 1);
            Point topRight = new Point(GridPositions[i].X + 1, GridPositions[i].Y - 1);
            Point bottomRight = new Point(GridPositions[i].X + 1, GridPositions[i].Y + 1);
            Point bottomLeft = new Point(GridPositions[i].X - 1, GridPositions[i].Y + 1);

            if (!ExpandedGridPositions.Contains(left) && left.X >= 0)
                ExpandedGridPositions.Add(left);
            if (!ExpandedGridPositions.Contains(right) && right.X < gameData.MapGridData.GetLength(0) - 1)
                ExpandedGridPositions.Add(right);
            if (!ExpandedGridPositions.Contains(top) && top.Y >= 0)
                ExpandedGridPositions.Add(top);
            if (!ExpandedGridPositions.Contains(bottom) && bottom.Y < gameData.MapGridData.GetLength(1) - 1)
                ExpandedGridPositions.Add(bottom);
            if (!ExpandedGridPositions.Contains(topLeft) && topLeft.X >= 0 && topLeft.Y >= 0)
                ExpandedGridPositions.Add(topLeft);
            if (!ExpandedGridPositions.Contains(topRight) && topRight.X < gameData.MapGridData.GetLength(0) - 1 && topRight.Y >= 0)
                ExpandedGridPositions.Add(topRight);
            if (!ExpandedGridPositions.Contains(bottomRight) && bottomRight.X < gameData.MapGridData.GetLength(0) - 1 && bottomRight.Y < gameData.MapGridData.GetLength(1) - 1)
                ExpandedGridPositions.Add(bottomRight);
            if (!ExpandedGridPositions.Contains(bottomLeft) && bottomLeft.X >= 0 && bottomLeft.Y < gameData.MapGridData.GetLength(1) - 1)
                ExpandedGridPositions.Add(bottomLeft);
        }
    }
    public override void OnPositionSet()
    {
        Vector2 left = new Vector2(Position.X - Texture.Width, Position.Y);
        Vector2 right = new Vector2(Position.X + Texture.Width, Position.Y);
        Vector2 top = new Vector2(Position.X, Position.Y - Texture.Height);
        Vector2 bottom = new Vector2(Position.X, Position.Y + Texture.Height);
        Vector2 topLeft = new Vector2(Position.X - Texture.Width, Position.Y - Texture.Height);
        Vector2 topRight = new Vector2(Position.X + Texture.Width, Position.Y - Texture.Height);
        Vector2 bottomRight = new Vector2(Position.X + Texture.Width, Position.Y + Texture.Height);
        Vector2 bottomLeft = new Vector2(Position.X - Texture.Width, Position.Y + Texture.Height);

        if (IsInBounds(left))
            SaplingSpawnPoints.Add(left);
        if (IsInBounds(right))
            SaplingSpawnPoints.Add(right);
        if (IsInBounds(top))
            SaplingSpawnPoints.Add(top);
        if (IsInBounds(bottom))
            SaplingSpawnPoints.Add(bottom);
        if (IsInBounds(topLeft))
            SaplingSpawnPoints.Add(topLeft);
        if (IsInBounds(topRight))
            SaplingSpawnPoints.Add(topRight);
        if (IsInBounds(bottomRight))
            SaplingSpawnPoints.Add(bottomRight);
        if (IsInBounds(bottomLeft))
            SaplingSpawnPoints.Add(bottomLeft);
    }
    public void CheckTexture()
    {
        string textureStartChars = "T";

        if (SuperTree)
            textureStartChars = "TS";

        if (FoodAmount >= 50)
        {
            if (CurrentTexture != (textureStartChars + "5"))
            {
                CurrentTexture = (textureStartChars + "5");
                Texture = TexturesList.First(t => t.Name == (textureStartChars + "5")).Texture;
            }
        }
        else if (FoodAmount >= 30)
        {
            if (CurrentTexture != (textureStartChars + "4"))
            {
                CurrentTexture = (textureStartChars + "4");
                Texture = TexturesList.First(t => t.Name == (textureStartChars + "4")).Texture;
            }
        }
        else if (FoodAmount >= 20)
        {
            if (CurrentTexture != (textureStartChars + "3"))
            {
                CurrentTexture = (textureStartChars + "3");
                Texture = TexturesList.First(t => t.Name == (textureStartChars + "3")).Texture;
            }
        }
        else if (FoodAmount >= 10)
        {
            if (CurrentTexture != (textureStartChars + "2"))
            {
                CurrentTexture = (textureStartChars + "2");
                Texture = TexturesList.First(t => t.Name == (textureStartChars + "2")).Texture;
            }
        }
        else if (FoodAmount >= 5)
        {
            if (CurrentTexture != (textureStartChars + "1"))
            {
                CurrentTexture = (textureStartChars + "1");
                Texture = TexturesList.First(t => t.Name == (textureStartChars + "1")).Texture;
            }
        }
        else
        {
            if (CurrentTexture != (textureStartChars + "0"))
            {
                CurrentTexture = (textureStartChars + "0");
                Texture = TexturesList.First(t => t.Name == (textureStartChars + "0")).Texture;
            }
        }
    }

    //Helper functions
    private float Mutate(Random rand, float baseValue, float range)
    {
        double randDouble = rand.NextDouble();
        float mutationValue = (float)randDouble * range;

        //Do not allow mutations to result in a value below 0;
        if (baseValue - mutationValue > 0)
        {
            if (rand.Next(0, 100) > 55) //55% Chance to mutate positively
            {
                mutationValue = mutationValue * -1;
            }
        }

        return mutationValue;
    }
    private int MutateSpreadRadius(Random rand, int baseValue, int mutationAmountMax, int chanceToMutate)
    {
        int mutationValue = rand.Next(0, mutationAmountMax);

        //Do not allow mutations to result in a value below 0;
        if (baseValue - mutationValue > 0)
        {
            if (rand.Next(0, 100000) > 50000) //50% Chance to mutate positively
            {
                mutationValue = mutationValue * -1;
            }
        }

        if (rand.Next(0, 100) > (100 - chanceToMutate)) //Chance to do a mutation
            return mutationValue;
        else
            return 0;
    }
    private bool IsInBounds(Vector2 vec)
    {
        if (vec.X <= (Texture.Width / 2) || vec.X + (Texture.Width / 2) >= WorldSize)
        {
            return false;
        }
        if (vec.Y <= (Texture.Height / 2) || vec.Y + (Texture.Height / 2) >= WorldSize)
        {
            return false;
        }

        return true;
    }
}