using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Creature : SpriteBase
{
    private Vector2 _direction;
    private float _rotation;
    private float _undigestedFood;
    private float _coldClimateTolerance;
    private float _hotClimateTolerance;
    private bool _isAlive;
    private float _foodDigestion;
    private float _lifespan;
    private float _eggInterval;

    /// <summary>
    /// Adding new Properties make sure to Add to the following: Init, LayEgg, IsSameAs, ZeroOutNegativeValues
    /// </summary>

    public int CreatureId { get; set; }
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            DrawObject = value;
        }
    }
    public string DeathCause { get; set; }
    public List<string> Ancestors { get; set; } //Chain of Ancestors (Starts blank) 
    public List<int> AncestorIds { get; set; }
    public bool IsChangingSpecies { get; set; } //Is the creature transitioning to a new species
    public string NewSpeciesName { get; set; } //Name of new species for these offspring
    public int NewSpeciesId { get; set; } //ID for the new species
    public int SpeciesId { get; set; } //Unique ID for species
    public string Species { get; set; } //Random name assigned to specied
    public int OriginalSpeciesId { get; set; }
    public string OriginalSpecies { get; set; } //The starting species, this is to help with Data analysis
    public string SpeciesStrain { get; set; } //This will keep track of specific strains within a species
    public string BabySpeciesStrainCounter { get; set; } //This is used for assigning babies with their strain
    public int Generation { get; set; } //How many generations since the first creature
    public Vector2 Direction
    {
        get { return _direction; }
        set
        {
            _direction = value;
        }
    }
    public float Rotation
    {
        get { return _rotation; }
        set
        {
            _rotation = value;
            Direction = new Vector2((float)Math.Cos(Rotation - MathHelper.ToRadians(90)), (float)Math.Sin(Rotation - MathHelper.ToRadians(90)));
            Direction.Normalize();
        }
    }
    public Vector2 CalculatedIntercept { get; set; } //Simply used for debugging purposes
    public float UndigestedFood
    {
        get { return _undigestedFood; }
        set
        {
            _undigestedFood = value;
        }
    } //Food count waiting to be digested
    public float MaxUndigestedFood { get; set; } //Maximum amount of food that can be eaten
    public int DigestedFood { get; set; } //Food count that has been digested
    public float FoodDigestion
    {
        get
        {
            return _foodDigestion;
        }
        set
        {
            _foodDigestion = value;
            FoodDigestionActual = _foodDigestion / (30 / TicksPerSecond);
        }
    } //How quickly food can be digested and converted into an egg, Also the longer it takes the more Energy the food will give
    public float FoodDigestionActual { get; set; } //Adjusted food digestion based on games ticks per second
    public float FoodTypeBlue { get; set; } //Food Type blue level, if this is higher than Red or Green then the creature can only eat this type
    public float FoodTypeRed { get; set; } //Food Type red level, if this is higher than Blue or Green then the creature can only eat this type
    public float FoodTypeGreen { get; set; } //Food Type green level, if this is higher than Red or Blue then the creature can only eat this type
    public string FoodType
    {
        get
        {
            if (FoodTypeBlue > FoodTypeRed && FoodTypeBlue > FoodTypeGreen)
            {
                return "Blue";
            }
            else if (FoodTypeRed > FoodTypeBlue && FoodTypeRed > FoodTypeGreen)
            {
                return "Red";
            }
            else
            {
                return "Green";
            }
        }
    }
    public int FoodTypeInt
    {
        get
        {
            if (IsScavenger)
            {
                return -1;
            }
            else if (FoodTypeBlue > FoodTypeRed && FoodTypeBlue > FoodTypeGreen)
            {
                return 0;
            }
            else if (FoodTypeRed > FoodTypeBlue && FoodTypeRed > FoodTypeGreen)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
    public Color CreatureColor { get; set; }
    public long BorderCollisionTickNum { get; set; } //Holds the border collision tick number when we last ran into the border, used to fix creatures getting stuck on border
    public int TotalFoodEaten { get; set; } //Statistical count of how many food were eaten
    public int TicksSinceLastEgg { get; set; } //The amount of Game Ticks since the last egg was created
    public int TicksSinceLastDigestedFood { get; set; } //The amount of Game Ticks since the last food was digested
    public float EggInterval
    {
        get
        {
            return _eggInterval;
        }
        set
        {
            _eggInterval = value;
            EggIntervalActual = _eggInterval / (30 / TicksPerSecond);
        }
    } //How often an egg can be output
    public float EggIntervalActual { get; set; } //How often an egg can be output adjusted for current tick rate
    public float EggIncubation { get; set; } //How long it takes for the egg to hatch once created
    public float EggIncubationActual { get; set; } //This is calculated based on their species level, if the creature is a herbavore add Herbavore * 10
    public float EggCamo { get; set; } //How well the egg is hidden from Scavengers
    public List<int> InvisibleEggs { get; set; }
    //NOT IMPLEMENTED
    public float EggToxicity { get; set; } //How toxic the egg is to other creatures
    public int EggsCreated { get; set; }
    public float Speed { get; set; } //How quickly the creature moved through the world
    public float PathDeviationAmount { get; set; } //How much to deviate from current path heading to simulate wandering
    public float PathDeviationFrequency { get; set; } //How often to deviate from the current path heading
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
    } //How long the creature lives
    public float LifespanActual { get; set; } //How long the creature lives adjusted by the current tick rate
    public float Energy { get; set; } //Energy is spent by moving and earned by eating
    public float ElapsedTicks { get; set; } //How many ticks the creature has been alive
    public float Sight { get; set; } //Allows the creature to adjust path if target is within this many units
    public int TicksSinceLastVisionCheck { get; set; } //How long since we evaluated vision
    public int TicksBetweenVisionChecks { get; set; } //How many ticks go by before we can evaluate sight 
    //NOT IMPLEMENTED
    public float Attraction { get; set; }
    public float Herbavore { get; set; } //This controls how strong of food the creature can eat. The food strength simulates food that is difficult to eat or hard to reach. This encourages positive mutations to reach stronger food
    public float Carnivore { get; set; } //Can eat other creatures with Carnivore level of (Carnivore lvl / 2 - 5) or less. This will be the only means of food
    public float Omnivore { get; set; } //Can eat both creatures and plants
    public bool IsHerbavore { get; set; }
    public bool IsCarnivore { get; set; }
    public bool IsScavenger { get; set; }
    public bool IsOmnivore { get; set; } //Probably no point to this since we also mark the IsHerb and IsCarn flags as well
    public float Scavenger { get; set; } //Can eat other creatures eggs including Scavenger eggs. This will be the only means of food
    //NOT IMPLEMENTED
    public float Camo { get; set; } //Hidden from all creatures with a Camo level less than your level
    //NOT IMPLEMENTED
    public float Cloning { get; set; } //Chance for Spontaneous cloning to occur
    public float ColdClimateTolerance
    {
        get
        {
            if (_coldClimateTolerance >= _hotClimateTolerance)
                return _coldClimateTolerance;
            else
                return 0;
        }
        set
        {
            _coldClimateTolerance = value;
        }
    } //How well the creature can handle cold parts of the map before needing to move to neutral
    public float HotClimateTolerance
    {
        get
        {
            if (_hotClimateTolerance >= _coldClimateTolerance)
                return _hotClimateTolerance;
            else
                return 0;
        }
        set
        {
            _hotClimateTolerance = value;
        }
    } //How well the creature can handle hot parts of the map before needing to move to neutral
    public int TicksInHotClimate { get; set; }
    public int TicksInColdClimate { get; set; }
    public bool IsLeavingClimate { get; set; }

    public const string CREATURE_TABLE_NAME = "Creatures";
    public const string ANCESTORS_TABLE_NAME = "Ancestors";
    public const float EGG_CAMO_COST_MULTIPLIER = 10f; //Multiplier on the energy cost for laying the egg

    //public const string CREATURE_TABLE_NAME = "Creatures";
    //public const string ANCESTORS_TABLE_NAME = "Ancestors";
    //public const float EGG_INTERVAL_INIT_MIN = 450;
    //public const float EGG_INTERVAL_INIT_MAX = 450;
    //public const float EGG_INCUBATION_INIT_MIN = 600;
    //public const float EGG_INCUBATION_INIT_MAX = 600;
    //public const float FOOD_DIGESTION_INIT_MIN = 100;
    //public const float FOOD_DIGESTION_INIT_MAX = 100;
    //public const float SPEED_INIT_MIN = 15;
    //public const float SPEED_INIT_MAX = 15;
    //public const float LIFESPAN_INIT_MIN = 1000;
    //public const float LIFESPAN_INIT_MAX = 1000;
    //public const float HERBAVORE_INIT_MIN = 1;
    //public const float HERBAVORE_INIT_MAX = 2;
    //public const float COLD_TOLERANCE_INIT_MIN = 5;
    //public const float COLD_TOLERANCE_INIT_MAX = 5;
    //public const float HOT_TOLERANCE_INIT_MIN = 5;
    //public const float HOT_TOLERANCE_INIT_MAX = 5;
    //public const float ENERGY_INIT = 500;
    //public const int TICKS_BETWEEN_SIGHT_EVAL = 30;

    public Creature()
    {
        Ancestors = new List<string>();
        AncestorIds = new List<int>();
        InvisibleEggs = new List<int>();
        DeathCause = String.Empty;
        IsLeavingClimate = false;
        CalculatedIntercept = Vector2.Zero;
    }

    public void InitNewCreature(Random rand, ref Names names, int speciesId, ref int creatureIdCtr, GameData gameData)
    {
        creatureIdCtr++;
        CreatureId = creatureIdCtr;
        TicksPerSecond = gameData.TicksPerSecond;
        IsAlive = true;
        IsChangingSpecies = false;
        NewSpeciesName = String.Empty;
        SpeciesId = speciesId;
        Species = names.GetRandomNameWithUniqueSpeciesLetter(rand, gameData);
        OriginalSpeciesId = SpeciesId;
        OriginalSpecies = Species;
        SpeciesStrain = String.Empty;
        BabySpeciesStrainCounter = "A";
        Generation = 0;
        Rotation = MathHelper.ToRadians(rand.Next(0, 360));
        UndigestedFood = 0;
        DigestedFood = 0;
        FoodDigestion = rand.Next((int)(gameData.CreatureSettings.StartingFoodDigestionMin * 10f), (int)(gameData.CreatureSettings.StartingFoodDigestionMax * 10f));
        TotalFoodEaten = 0;
        TicksSinceLastDigestedFood = 0;
        TicksSinceLastEgg = 0;
        EggInterval = rand.Next((int)(gameData.CreatureSettings.StartingEggIntervalMin * 10f), (int)(gameData.CreatureSettings.StartingEggIntervalMax * 10f));
        EggIncubation = rand.Next((int)(gameData.CreatureSettings.StartingEggIncubationMin * 10f), (int)(gameData.CreatureSettings.StartingEggIncubationMax * 10f));
        EggsCreated = 0;
        EggCamo = 0;
        EggToxicity = 0;
        Speed = rand.Next((int)gameData.CreatureSettings.StartingSpeedMin, (int)gameData.CreatureSettings.StartingSpeedMax);
        Lifespan = rand.Next((int)(gameData.CreatureSettings.StartingLifespanMin * 10f), (int)(gameData.CreatureSettings.StartingLifespanMax * 10f));
        Energy = gameData.CreatureSettings.StartingEnergy; //No mutations or variance on this
        ElapsedTicks = 0;
        Sight = 0;
        TicksSinceLastVisionCheck = 0;
        TicksBetweenVisionChecks = (int)Math.Ceiling(TicksPerSecond / 2.0);
        Attraction = 0;
        Herbavore = rand.Next((int)gameData.CreatureSettings.StartingHerbavoreLevelMin, (int)gameData.CreatureSettings.StartingHerbavoreLevelMax);
        Carnivore = rand.Next((int)gameData.CreatureSettings.StartingCarnivoreLevelMin, (int)gameData.CreatureSettings.StartingCarnivoreLevelMax);
        Scavenger = rand.Next((int)gameData.CreatureSettings.StartingScavengerLevelMin, (int)gameData.CreatureSettings.StartingScavengerLevelMax);
        Omnivore = rand.Next((int)gameData.CreatureSettings.StartingOmnivoreLevelMin, (int)gameData.CreatureSettings.StartingOmnivoreLevelMax);
        Camo = 0;
        Cloning = 0;
        ColdClimateTolerance = rand.Next((int)gameData.CreatureSettings.StartingColdToleranceMin, (int)gameData.CreatureSettings.StartingColdToleranceMax);
        HotClimateTolerance = rand.Next((int)gameData.CreatureSettings.StartingHotToleranceMin, (int)gameData.CreatureSettings.StartingHotToleranceMax);
        TicksInColdClimate = 0;
        TicksInHotClimate = 0;
        EggIncubationActual = EggIncubation + (Herbavore * 10);
        FoodTypeBlue = 0;
        FoodTypeRed = 0;
        FoodTypeGreen = 0;
        CreatureColor = Color.White;
        MaxUndigestedFood = gameData.MaxCreatureUndigestedFood;
        BorderCollisionTickNum = -999;

        int foodTypeRand = rand.Next(0,3);
        switch (foodTypeRand)
        {
            case 0:
                FoodTypeBlue = 5;
                break;
            case 1:
                FoodTypeRed = 5;
                break;
            default:
                FoodTypeGreen = 5;
                break;
        }

        bool isHerb, isCarn, isScav, isOmni;
        DetermineCreatureType(Herbavore, Carnivore, Scavenger, Omnivore, out isHerb, out isCarn, out isScav, out isOmni);
        IsHerbavore = isHerb;
        IsCarnivore = isCarn;
        IsScavenger = isScav;
        IsOmnivore = isOmni;

        SetCreatureColor();

        foreach (Creature c in gameData.Creatures)
        {
            if (IsCloseTo(this, c))
            {
                Species = c.Species;
                SpeciesId = c.SpeciesId;
                SpeciesStrain = c.BabySpeciesStrainCounter;
                break;
            }
        }
    }
    public void AdvanceTick(Random rand)
    {
        ElapsedTicks++;
        TicksSinceLastEgg++;
        TicksSinceLastVisionCheck++;

        if (UndigestedFood >= 1) //only allow digestion once a food has been eaten
            TicksSinceLastDigestedFood++;

        if (UndigestedFood >= 1 && TicksSinceLastDigestedFood >= FoodDigestionActual)
        {
            TicksSinceLastDigestedFood = 0;
            UndigestedFood--;
            DigestedFood++;
        }

        if (TicksInColdClimate > 0 && !IsInCold)
            TicksInColdClimate = 0;
        if (TicksInHotClimate > 0 && !IsInHot)
            TicksInHotClimate = 0;

        if (IsInCold)
            TicksInColdClimate++;
        if (IsInHot)
            TicksInHotClimate++;

        if (IsInCold && !IsLeavingClimate && TicksInColdClimate / TicksPerSecond > ColdClimateTolerance)
        {
            //Move straight down to get out of the cold climate
            int tmpRotation;
            tmpRotation = rand.Next(170, 190);
            Rotation = MathHelper.ToRadians(tmpRotation);
            IsLeavingClimate = true;
        }
        else if (IsInHot && !IsLeavingClimate && TicksInHotClimate / TicksPerSecond > HotClimateTolerance)
        {
            //Move straight up to get out of the hot climate
            int tmpRotation;

            if (rand.Next(0, 100) > 50)
            {
                tmpRotation = rand.Next(0, 10);
            }
            else
            {
                tmpRotation = rand.Next(350, 360);
            }
                
            Rotation = MathHelper.ToRadians(tmpRotation);
            IsLeavingClimate = true;
        }
        else if(IsLeavingClimate)
        {
            //Check if we are out of the climate to change the rotation back
            if (!IsInCold && !IsInHot)
            {
                IsLeavingClimate = false;
                int tmpRotation;

                if (MathHelper.ToDegrees(Rotation) >= 170 && MathHelper.ToDegrees(Rotation) <= 190) //Cold
                {
                    if (_coldClimateTolerance >= _hotClimateTolerance)
                    {
                        tmpRotation = rand.Next(0, 360);
                    }
                    else
                    {
                        tmpRotation = rand.Next(91, 269);
                    }

                    Rotation = MathHelper.ToRadians(tmpRotation);
                }
                else //Hot
                {
                    if (_hotClimateTolerance >= _coldClimateTolerance)
                    {
                        tmpRotation = rand.Next(0, 360); //If we are a hot climate species let us randomly rotate back into the hot
                    }
                    else
                    {
                        tmpRotation = rand.Next(1, 89);
                        if (rand.Next(0, 100) > 50)
                        {
                            tmpRotation = tmpRotation * -1;
                        }
                    }

                    Rotation = MathHelper.ToRadians(tmpRotation);
                }
            }
        }
    }
    public Egg LayEgg(Random rand, ref Names names, GameData gameData, ref int creatureIdCtr)
    {
        int carnivoreMutationBonus = 0;
        int scavengerMutationBonus = 0;
        int omnivoreMutationBonus = 0;
        Egg egg = new Egg();
        Creature baby = new Creature();

        //No boost for Herbavore since all creatures start out as Herbavore
        if (IsCarnivore)
        {
            carnivoreMutationBonus = (int)Math.Round(gameData.MutationSettings.Carnivore * (gameData.MutationSettings.MutationBonusPercent / 100f), 0);
        }
        else if (IsScavenger)
        {
            scavengerMutationBonus = (int)Math.Round(gameData.MutationSettings.Scavenger * (gameData.MutationSettings.MutationBonusPercent / 100f), 0);
        }
        else if (IsOmnivore)
        {
            omnivoreMutationBonus = (int)Math.Round(gameData.MutationSettings.Omnivore * (gameData.MutationSettings.MutationBonusPercent / 100f), 0);
        }

        TicksSinceLastEgg = 0;
        EggsCreated++;

        creatureIdCtr++;
        baby.CreatureId = creatureIdCtr;
        baby.Ancestors = CopyAncestorList(Ancestors);
        baby.AncestorIds = CopyAncestorIdsList(AncestorIds);
        baby.IsAlive = true;
        baby.WorldSize = WorldSize;
        baby.TicksPerSecond = TicksPerSecond;
        baby.ClimateHeightPercent = ClimateHeightPercent;
        baby.IsChangingSpecies = false;
        baby.NewSpeciesId = -1;
        baby.NewSpeciesName = String.Empty;
        baby.Rotation = MathHelper.ToRadians(rand.Next(0, 360));
        baby.Position = Position;
        baby.SpeciesId = SpeciesId;
        baby.Species = Species;
        baby.OriginalSpecies = OriginalSpecies;
        baby.OriginalSpeciesId = OriginalSpeciesId;
        baby.BabySpeciesStrainCounter = "A";
        baby.Generation = Generation + 1;
        baby.UndigestedFood = 0;
        baby.DigestedFood = 0;
        baby.TotalFoodEaten = 0;
        baby.TicksSinceLastDigestedFood = 0;
        baby.TicksSinceLastEgg = 0;
        baby.ElapsedTicks = 0;
        baby.Energy = gameData.CreatureSettings.StartingEnergy + (GetCreatureLevel() * 10); //No mutation chance on energy
        baby.TicksSinceLastVisionCheck = 0;
        baby.TicksBetweenVisionChecks = TicksBetweenVisionChecks;
        baby.TicksInColdClimate = 0;
        baby.TicksInHotClimate = 0;
        baby.MaxUndigestedFood = MaxUndigestedFood;
        baby.IsLeavingClimate = false;

        //Mutations
        baby.EggCamo = EggCamo + Mutation(rand, gameData.MutationSettings.EggCamo, gameData);
        baby.EggIncubation = EggIncubation + (Mutation(rand, gameData.MutationSettings.EggIncubation, gameData) * 10);
        baby.EggInterval = EggInterval + (Mutation(rand, gameData.MutationSettings.EggInterval, gameData) * 10);
        baby.EggToxicity = EggToxicity + Mutation(rand, gameData.MutationSettings.EggToxicity, gameData);
        baby.FoodDigestion = FoodDigestion + (Mutation(rand, gameData.MutationSettings.FoodDigestion, gameData) * 10);
        baby.FoodTypeBlue = FoodTypeBlue + (Mutation(rand, gameData.MutationSettings.FoodType, gameData) * 10);
        baby.FoodTypeRed = FoodTypeRed + (Mutation(rand, gameData.MutationSettings.FoodType, gameData) * 10);
        baby.FoodTypeGreen = FoodTypeGreen + (Mutation(rand, gameData.MutationSettings.FoodType, gameData) * 10);
        baby.Speed = Speed + Mutation(rand, gameData.MutationSettings.Speed, gameData);
        baby.Lifespan = Lifespan + (Mutation(rand, gameData.MutationSettings.Lifespan, gameData) * 10);
        baby.Sight = Sight + Mutation(rand, gameData.MutationSettings.Sight, gameData);
        baby.Attraction = Attraction + Mutation(rand, gameData.MutationSettings.Attraction, gameData);
        baby.Camo = Camo + Mutation(rand, gameData.MutationSettings.Camo, gameData);
        baby.Cloning = Cloning + Mutation(rand, gameData.MutationSettings.Cloning, gameData);
        baby.ColdClimateTolerance = _coldClimateTolerance + Mutation(rand, gameData.MutationSettings.ColdClimateTolerance - _hotClimateTolerance, gameData);
        baby.HotClimateTolerance = _hotClimateTolerance + Mutation(rand, gameData.MutationSettings.HotClimateTolerance - _coldClimateTolerance, gameData);
        baby.Herbavore = Herbavore + Mutation(rand, gameData.MutationSettings.Herbavore, gameData);
        baby.Carnivore = Carnivore + Mutation(rand, gameData.MutationSettings.Carnivore + carnivoreMutationBonus, gameData);
        baby.Omnivore = Omnivore + Mutation(rand, gameData.MutationSettings.Omnivore + omnivoreMutationBonus, gameData);
        baby.Scavenger = Scavenger + Mutation(rand, gameData.MutationSettings.Scavenger + scavengerMutationBonus, gameData);

        if (baby.Herbavore >= baby.Carnivore && baby.Herbavore >= baby.Scavenger && baby.Herbavore >= baby.Omnivore)
        {
            baby.IsHerbavore = true;
            baby.IsCarnivore = false;
            baby.IsScavenger = false;
            baby.IsOmnivore = false;
            baby.EggIncubationActual = baby.EggIncubation + (baby.Herbavore * 10);
        }
        else if (baby.Carnivore >= baby.Herbavore && baby.Carnivore >= baby.Scavenger && baby.Carnivore >= baby.Omnivore)
        {
            baby.IsHerbavore = false;
            baby.IsCarnivore = true;
            baby.IsScavenger = false;
            baby.IsOmnivore = false;
            baby.EggIncubationActual = baby.EggIncubation + (baby.Carnivore * 10);
        }
        else if (baby.Scavenger >= baby.Herbavore && baby.Scavenger >= baby.Carnivore && baby.Scavenger >= baby.Omnivore)
        {
            baby.IsHerbavore = false;
            baby.IsCarnivore = false;
            baby.IsScavenger = true;
            baby.IsOmnivore = false;
            baby.EggIncubationActual = baby.EggIncubation + (baby.Scavenger * 10);
        }
        else if (baby.Omnivore >= baby.Herbavore && baby.Omnivore >= baby.Carnivore && baby.Omnivore >= baby.Scavenger)
        {
            baby.IsHerbavore = true;
            baby.IsCarnivore = true;
            baby.IsScavenger = false;
            baby.IsOmnivore = true;
            baby.EggIncubationActual = baby.EggIncubation + (baby.Omnivore * 10);
        }

        baby.SetCreatureColor();

        //Only iterate the Species/Strain if something that can Mutate has changed
        if (!IsSameAs(baby))
        {
            if (IsChangingSpecies || SpeciesStrain.Replace(" ", "").Length > 150 || GetCreatureTypeInt() != baby.GetCreatureTypeInt()) //New Species once the Strain goes past 150 different strains or the creature is now a new type
            {
                if (!IsChangingSpecies)
                {
                    NewSpeciesName = names.GetRandomName(rand, Species);
                    NewSpeciesId = gameData.NextSpeciesId;
                    IsChangingSpecies = true;
                }

                //gameData.EventLog.Add("Species '" + Species + "' has mutated into '" + NewSpeciesName + "'");

                baby.Species = NewSpeciesName;
                baby.SpeciesId = NewSpeciesId;
                baby.SpeciesStrain = BabySpeciesStrainCounter;
                baby.Ancestors.Add(Species);
                baby.AncestorIds.Add(SpeciesId);
            }
            else
            {
                baby.SpeciesStrain = SpeciesStrain + " " + BabySpeciesStrainCounter;
                BabySpeciesStrainCounter = Global.GetNextBase26(BabySpeciesStrainCounter);
            }
        }
        else
        {
            baby.SpeciesStrain = SpeciesStrain;
        }

        ZeroOutNegativeValues(ref baby);

        egg.WorldSize = WorldSize;
        egg.ClimateHeightPercent = ClimateHeightPercent;
        egg.Position = Position;
        egg.ElapsedTicks = 0;
        egg.TicksTillHatched = (int)Math.Ceiling(EggIncubationActual) / (30 / TicksPerSecond);
        egg.Camo = EggCamo;
        egg.Creature = baby;

        return egg;
    }
    public List<string> GetCreatureInformation()
    {
        List<string> info = new List<string>();

        info.Add("Species: " + Species + " (" + SpeciesId + ")");
        if (String.IsNullOrEmpty(SpeciesStrain))
        {
            info.Add("Strain: Original");
        }
        else
        {
            if(SpeciesStrain.Replace(" ", "").Length > 20)
                info.Add("Strain: " + SpeciesStrain.Replace(" ", "").Substring(0,20) + "... (" + SpeciesStrain.Replace(" ", "").Length + ")");
            else
                info.Add("Strain: " + SpeciesStrain.Replace(" ", ""));
        }
        if (Ancestors.Count > 0)
        {
            string ancestorsString = String.Empty;
            if (Ancestors.Count > 3)
            {
                ancestorsString += Ancestors[0] + ", " + Ancestors[1] + "..." + Ancestors[Ancestors.Count - 1];
            }
            else
            {
                for (int i = 0; i < Ancestors.Count; i++)
                {
                    ancestorsString += Ancestors[i] + ", ";
                }
            }
            ancestorsString = ancestorsString.Substring(0, ancestorsString.Length - 2);
            info.Add("Ancestors: " + ancestorsString);
        }
        info.Add("Generation: " + Generation);
        info.Add("Lifespan: " + Math.Round(Lifespan / 10.0, 1).ToString());
        info.Add("Age: " + Math.Round((ElapsedTicks / 10.0) * (30 / TicksPerSecond), 1).ToString());
        info.Add("Energy: " + Energy);
        info.Add(" ");
        info.Add("Food Type: " + FoodType);
        info.Add("Food: " + UndigestedFood);
        info.Add("Food Digested: " + DigestedFood);
        info.Add("Last Digested: " + Math.Round((TicksSinceLastDigestedFood / 10.0) * (30 / TicksPerSecond), 1).ToString());
        info.Add("Food Digestion Rate: " + Math.Round(FoodDigestion / 10.0, 1).ToString());
        info.Add("Lifetime Food: " + TotalFoodEaten);
        info.Add(" ");
        info.Add("Egg Interval: " + Math.Round(EggInterval / 10.0, 1).ToString());
        info.Add("Egg Incubation: " + Math.Round(EggIncubation / 10.0, 1).ToString());
        info.Add("Egg Incubation Actual: " + Math.Round(EggIncubationActual / 10.0, 1).ToString());
        info.Add("Egg Camo: " + EggCamo);
        info.Add("Egg Toxicity: " + EggToxicity);
        info.Add("Last Egg: " + Math.Round((TicksSinceLastEgg / 10.0) * (30 / TicksPerSecond), 1).ToString());
        info.Add("Eggs Created: " + EggsCreated);
        info.Add(" ");
        info.Add("Herbavore: " + Herbavore);
        info.Add("Carnivore: " + Carnivore);
        info.Add("Omnivore: " + Omnivore);
        info.Add("Scavenger: " + Scavenger);
        info.Add(" ");
        info.Add("Speed: " + Speed);
        info.Add("Sight: " + Sight);
        info.Add("Camo: " + Camo);
        info.Add("Cloning: " + Cloning);
        info.Add("Cold Tolerance: " + _coldClimateTolerance);
        info.Add("Hot Tolerance: " + _hotClimateTolerance);
        //info.Add(" ");
        //info.Add("Species ID: " + SpeciesId);
        //info.Add("Position: {X:" + ((int)Position.X).ToString().PadLeft(4, ' ') + ", Y:" + ((int)Position.Y).ToString().PadLeft(4, ' '));
        //info.Add("Direction: " + Direction);
        //info.Add("Rotation: " + Rotation + " :: " + MathHelper.ToDegrees(Rotation));

        return info;
    }
    public List<string> GetCreatureStatisticsSQL(int seed, int sessionID, float creatureRatio, float foodRatio, double gameTimeMinutes)
    {
        List<string> sqlStatements = new List<string>();

        string creatureSql = String.Empty;

        creatureSql += "Insert into " + CREATURE_TABLE_NAME + " (" +
            "RunTime," +
            "GameSeed," +
            "SessionID," +
            "MapSize," +
            "CreatureRatio," +
            "FoodRatio," +
            "IsAlive," +
            "DeathCause," +
            "CreatureID," +
            "CreatureName," +
            "SpeciesID," +
            "SpeciesName," +
            "SpeciesStrain," +
            "OriginalSpeciesID," +
            "OriginalSpeciesName," +
            "Generation," +
            "FoodType," +
            "FoodDigestionRate," +
            "EggInterval," +
            "EggIncubation," +
            "EggIncubationActual," +
            "EggCamo," +
            "EggToxicity," +
            "EggsCreatedTotal," +
            "Speed," +
            "LifeSpan," +
            "Sight," +
            "Attraction," +
            "Camo," +
            "Cloning," +
            "Herbavore," +
            "Carnivore," +
            "Omnivore," +
            "Scavenger," +
            "ColdClimateTolerance," +
            "HotClimateTolerance) Values (";
        creatureSql += Math.Round(gameTimeMinutes,4) + ",";
        creatureSql += seed + ",";
        creatureSql += sessionID + ",";
        creatureSql += WorldSize + ",";
        creatureSql += Math.Round(creatureRatio, 10) + ",";
        creatureSql += Math.Round(foodRatio, 10) + ",";
        creatureSql += "'" + IsAlive + "',";
        creatureSql += "'" + DeathCause + "',";
        creatureSql += CreatureId + ",";
        creatureSql += "'" + CreatureId + "',"; //At the momemnt we do not carry an individual creature name. Just use the ID for now
        creatureSql += SpeciesId + ",";
        creatureSql += "'" + Species + "',";
        creatureSql += "'" + SpeciesStrain + "',";
        creatureSql += OriginalSpeciesId + ",";
        creatureSql += "'" + OriginalSpecies + "',";
        creatureSql += Generation + ",";
        creatureSql += "'" + FoodType + "',";
        creatureSql += Math.Round(FoodDigestion, 4) + ",";
        creatureSql += Math.Round(EggInterval, 4) + ",";
        creatureSql += Math.Round(EggIncubation, 4) + ",";
        creatureSql += Math.Round(EggIncubationActual, 4) + ",";
        creatureSql += Math.Round(EggCamo, 4) + ",";
        creatureSql += Math.Round(EggToxicity, 4) + ",";
        creatureSql += EggsCreated + ",";
        creatureSql += Math.Round(Speed, 4) + ",";
        creatureSql += Math.Round(Lifespan, 4) + ",";
        creatureSql += Math.Round(Sight, 4) + ",";
        creatureSql += Math.Round(Attraction, 4) + ",";
        creatureSql += Math.Round(Camo, 4) + ",";
        creatureSql += Math.Round(Cloning, 4) + ",";
        creatureSql += Math.Round(Herbavore, 4) + ",";
        creatureSql += Math.Round(Carnivore, 4) + ",";
        creatureSql += Math.Round(Omnivore, 4) + ",";
        creatureSql += Math.Round(Scavenger, 4) + ",";
        creatureSql += Math.Round(ColdClimateTolerance, 4) + ",";
        creatureSql += Math.Round(HotClimateTolerance, 4);
        creatureSql += ")";

        sqlStatements.Add(creatureSql);

        for (int i = 0; i < AncestorIds.Count; i++)
        {
            string ancestorSQL = String.Empty;

            ancestorSQL += "Insert into " + ANCESTORS_TABLE_NAME + "(SessionID,CreatureID,AncestorID,AncestorName) Values (";
            ancestorSQL += sessionID + ",";
            ancestorSQL += CreatureId + ",";
            ancestorSQL += AncestorIds[i] + ",";
            ancestorSQL += "'" + Ancestors[i] + "'";
            ancestorSQL += ")";

            sqlStatements.Add(ancestorSQL);
        }

        return sqlStatements;
    }
    public CreatureStats GetCreatureStatistics(int seed, int sessionID, double gameTimeSeconds)
    {
        CreatureStats creatureStats = new CreatureStats();

        //String fields
        creatureStats.FieldHeaders.Add("GameRandomSeed");
        creatureStats.StringStats.Add(seed.ToString());
        creatureStats.FieldHeaders.Add("SessionID");
        creatureStats.StringStats.Add(sessionID.ToString());
        creatureStats.FieldHeaders.Add("GameTimeMinutes");
        creatureStats.StringStats.Add(Math.Round(gameTimeSeconds / 60, 2).ToString());
        creatureStats.FieldHeaders.Add("Species");
        creatureStats.StringStats.Add(Species);
        creatureStats.FieldHeaders.Add("Strain");
        creatureStats.StringStats.Add(SpeciesStrain);
        creatureStats.FieldHeaders.Add("OriginalSpecies");
        creatureStats.StringStats.Add(OriginalSpecies);
        creatureStats.FieldHeaders.Add("CreatureID");
        creatureStats.StringStats.Add(CreatureId.ToString());
        creatureStats.FieldHeaders.Add("CreatureType");
        creatureStats.StringStats.Add(GetCreatureTypeText());
        creatureStats.FieldHeaders.Add("DeathCause");
        creatureStats.StringStats.Add(DeathCause);
        creatureStats.FieldHeaders.Add("FoodType");
        creatureStats.StringStats.Add(FoodType);
        creatureStats.FieldHeaders.Add("IsAlive");
        creatureStats.StringStats.Add(IsAlive.ToString());
        creatureStats.FieldHeaders.Add("IsHerbavore");
        creatureStats.StringStats.Add(IsHerbavore.ToString());
        creatureStats.FieldHeaders.Add("IsCarnivore");
        creatureStats.StringStats.Add(IsCarnivore.ToString());
        creatureStats.FieldHeaders.Add("IsScavenger");
        creatureStats.StringStats.Add(IsScavenger.ToString());
        creatureStats.FieldHeaders.Add("IsOmnivore");
        creatureStats.StringStats.Add(IsOmnivore.ToString());

        //Int fields
        creatureStats.FieldHeaders.Add("SpeciesID");
        creatureStats.IntStats.Add(SpeciesId);
        creatureStats.FieldHeaders.Add("OriginalSpeciesID");
        creatureStats.IntStats.Add(OriginalSpeciesId);
        creatureStats.FieldHeaders.Add("Generation");
        creatureStats.IntStats.Add(Generation);
        creatureStats.FieldHeaders.Add("CreatureTypeID");
        creatureStats.IntStats.Add(GetCreatureTypeInt());
        creatureStats.FieldHeaders.Add("EggsCreated");
        creatureStats.IntStats.Add(EggsCreated);
        creatureStats.FieldHeaders.Add("DigestedFood");
        creatureStats.IntStats.Add(DigestedFood);
        creatureStats.FieldHeaders.Add("TotalFoodEaten");
        creatureStats.IntStats.Add(TotalFoodEaten);
        creatureStats.FieldHeaders.Add("FoodTypeID");
        creatureStats.IntStats.Add(FoodTypeInt);

        //Float Stats
        creatureStats.FieldHeaders.Add("UndigestedFood");
        creatureStats.FloatStats.Add(UndigestedFood);
        creatureStats.FieldHeaders.Add("Sight");
        creatureStats.FloatStats.Add(Sight);
        creatureStats.FieldHeaders.Add("Speed");
        creatureStats.FloatStats.Add(Speed);
        creatureStats.FieldHeaders.Add("Lifespan");
        creatureStats.FloatStats.Add(Lifespan);
        creatureStats.FieldHeaders.Add("HowLongHasCreatureBeenAlive");
        creatureStats.FloatStats.Add(ElapsedTicks);
        creatureStats.FieldHeaders.Add("Energy");
        creatureStats.FloatStats.Add(Energy);
        creatureStats.FieldHeaders.Add("EggIncubationLength");
        creatureStats.FloatStats.Add(EggIncubation);
        creatureStats.FieldHeaders.Add("EggIncubationLengthActual");
        creatureStats.FloatStats.Add(EggIncubationActual);
        creatureStats.FieldHeaders.Add("TimeBetweenLayingEggs");
        creatureStats.FloatStats.Add(EggInterval);
        //creatureStats.FieldHeaders.Add("EggToxicity");
        //creatureStats.FloatStats.Add(EggToxicity);
        creatureStats.FieldHeaders.Add("FoodDigestionSpeed");
        creatureStats.FloatStats.Add(FoodDigestion);
        creatureStats.FieldHeaders.Add("FoodTypeBlueLevel");
        creatureStats.FloatStats.Add(FoodTypeBlue);
        creatureStats.FieldHeaders.Add("FoodTypeGreenLevel");
        creatureStats.FloatStats.Add(FoodTypeGreen);
        creatureStats.FieldHeaders.Add("FoodTypeRedLevel");
        creatureStats.FloatStats.Add(FoodTypeRed);
        creatureStats.FieldHeaders.Add("CreatureTypeLevel");
        creatureStats.FloatStats.Add(GetCreatureLevel());
        creatureStats.FieldHeaders.Add("HerbavoreLevel");
        creatureStats.FloatStats.Add(Herbavore);
        creatureStats.FieldHeaders.Add("CarnivoreLevel");
        creatureStats.FloatStats.Add(Carnivore);
        creatureStats.FieldHeaders.Add("ScavengerLevel");
        creatureStats.FloatStats.Add(Scavenger);
        creatureStats.FieldHeaders.Add("OmnivoreLevel");
        creatureStats.FloatStats.Add(Omnivore);
        creatureStats.FieldHeaders.Add("ColdClimateTolerance");
        creatureStats.FloatStats.Add(ColdClimateTolerance);
        creatureStats.FieldHeaders.Add("HotClimateTolerance");
        creatureStats.FloatStats.Add(HotClimateTolerance);
        //creatureStats.FieldHeaders.Add("Attraction");
        //creatureStats.FloatStats.Add(Attraction);
        //creatureStats.FieldHeaders.Add("Camo");
        //creatureStats.FloatStats.Add(Camo);
        //creatureStats.FieldHeaders.Add("Cloning");
        //creatureStats.FloatStats.Add(Cloning);
        //creatureStats.FieldHeaders.Add("EggCamo");
        //creatureStats.FloatStats.Add(EggCamo);

        return creatureStats;
    }
    public float GetEggCreateEnergyLoss(float baseEnergyLost)
    {
        if (Camo > 0)
        {
            baseEnergyLost += Camo * EGG_CAMO_COST_MULTIPLIER;
        }

        return baseEnergyLost;
    }
    public void EggCreateEnergyLoss(float baseEnergyLost)
    {
        baseEnergyLost = GetEggCreateEnergyLoss(baseEnergyLost);

        Energy -= baseEnergyLost;
    }
    public float GetCreatureLevel()
    {
        if (IsOmnivore)
        {
            return Omnivore;
        }
        else if (IsHerbavore)
        {
            return Herbavore;
        }
        else if (IsCarnivore)
        {
            return Carnivore;
        }
        else if (IsScavenger)
        {
            return Scavenger;
        }

        return 0;
    }
    public float CalculateCreatureEnergyDepletion(GameData gameData)
    {
        float calculatedEnergyLoss = 0f;
        float depletionFromMovement = gameData.Settings.EnergyDepletionFromMovement;
        float energyDepletionPercentFromComplexity = gameData.Settings.EnergyDepletionPercentFromComplexity;

        //Omnivores are more complex to digest both Plants/Animals so have them deplete energy faster
        if (IsOmnivore)
        {
            energyDepletionPercentFromComplexity = energyDepletionPercentFromComplexity * 1.1f;
        }

        //Having sight will increase the speed in which energy depletes based on the world setting and their Sight level. If the setting is 50% increase in energy consumption and the sight level is 10 then it will result in a 60% increase to energy lost
        if (Sight > 0)
        {
            depletionFromMovement = depletionFromMovement + (depletionFromMovement * ((gameData.Settings.EnergyDepletionPercentFromComplexity + Sight) / 100f));
        }

        calculatedEnergyLoss = Speed * (depletionFromMovement / 1000f);

        return calculatedEnergyLoss;
    }

    //Helper functions
    private float Mutation(Random rand, float mutationChance, GameData gameData)
    {
        bool didMutationHappen = false;

        if (mutationChance > 0)
        {
            if (rand.Next(0, 100) >= (100 - mutationChance))
            {
                didMutationHappen = true;
            }

            if (didMutationHappen)
            {
                if (rand.Next(0, 10) > (10 - gameData.MutationSettings.ChanceToIncreaseValue))
                {
                    return gameData.MutationSettings.ChangeAmount;
                }
                else
                {
                    return gameData.MutationSettings.ChangeAmount * -1;
                }
            }
        }

        return 0f;
    }
    private float CalculateIntercept(Creature target)
    {
        //Vector2 totarget = target.Position - tower.Position;

        //float a = Vector2.Dot(target.Speed, target.velocity) - (bullet.velocity * bullet.velocity);
        //float b = 2 * Vector2.Dot(target.Speed, totarget);
        //float c = Vector2.Dot(totarget, totarget);

        //float p = -b / (2 * a);
        //float q = (float)Math.Sqrt((b * b) - 4 * a * c) / (2 * a);

        //float t1 = p - q;
        //float t2 = p + q;
        //float t;

        //if (t1 > t2 && t2 > 0)
        //{
        //    t = t2;
        //}
        //else
        //{
        //    t = t1;
        //}

        //Vector aimSpot = target.position + target.velocity * t;
        //Vector bulletPath = aimSpot - tower.position;
        //float timeToImpact = bulletPath.Length() / bullet.speed;//speed must be in units per second
        return 0;
    }
    private bool IsSameAs(Creature compareCreature)
    {
        if (EggCamo != compareCreature.EggCamo || EggInterval != compareCreature.EggInterval ||
            EggIncubation != compareCreature.EggIncubation || EggToxicity != compareCreature.EggToxicity ||
            FoodDigestion != compareCreature.FoodDigestion || Speed != compareCreature.Speed ||
            Lifespan != compareCreature.Lifespan || Sight != compareCreature.Sight ||
            Attraction != compareCreature.Attraction || Camo != compareCreature.Camo ||
            Cloning != compareCreature.Cloning || HotClimateTolerance != compareCreature.HotClimateTolerance ||
            ColdClimateTolerance != compareCreature.ColdClimateTolerance || Herbavore != compareCreature.Herbavore ||
            Carnivore != compareCreature.Carnivore || Scavenger != compareCreature.Scavenger ||
            Omnivore != compareCreature.Omnivore || FoodType != compareCreature.FoodType)
            return false;

        return true;
    }
    private bool IsCloseTo(Creature compareCreature1, Creature compareCreature2)
    {
        float differenceAmount = 0f;

        if (compareCreature1.IsHerbavore == compareCreature2.IsHerbavore && compareCreature1.IsCarnivore == compareCreature2.IsCarnivore && compareCreature1.IsScavenger == compareCreature2.IsScavenger && compareCreature1.IsOmnivore == compareCreature2.IsOmnivore)
        {
            differenceAmount += Math.Abs(compareCreature1.EggCamo - compareCreature2.EggCamo);
            differenceAmount += Math.Abs((compareCreature1.EggIncubation / 10) - (compareCreature2.EggIncubation / 10));
            differenceAmount += Math.Abs((compareCreature1.EggInterval / 10) - (compareCreature2.EggInterval / 10));
            differenceAmount += Math.Abs(compareCreature1.EggToxicity - compareCreature2.EggToxicity);
            differenceAmount += Math.Abs((compareCreature1.FoodDigestion / 10) - (compareCreature2.FoodDigestion / 10));
            differenceAmount += Math.Abs(compareCreature1.Speed - compareCreature2.Speed);
            differenceAmount += Math.Abs((compareCreature1.Lifespan / 10) - (compareCreature2.Lifespan / 10));
            differenceAmount += Math.Abs(compareCreature1.Sight - compareCreature2.Sight);
            differenceAmount += Math.Abs(compareCreature1.Attraction - compareCreature2.Attraction);
            differenceAmount += Math.Abs(compareCreature1.Camo - compareCreature2.Camo);
            differenceAmount += Math.Abs(compareCreature1.Cloning - compareCreature2.Cloning);
            differenceAmount += Math.Abs(compareCreature1.ColdClimateTolerance - compareCreature2.ColdClimateTolerance);
            differenceAmount += Math.Abs(compareCreature1.HotClimateTolerance - compareCreature2.HotClimateTolerance);
            differenceAmount += Math.Abs(compareCreature1.Herbavore - compareCreature2.Herbavore);
            differenceAmount += Math.Abs(compareCreature1.Carnivore - compareCreature2.Carnivore);
            differenceAmount += Math.Abs(compareCreature1.Omnivore - compareCreature2.Omnivore);
            differenceAmount += Math.Abs(compareCreature1.Scavenger - compareCreature2.Scavenger);

            if (compareCreature1.FoodType != compareCreature2.FoodType)
                differenceAmount += 3; //3 points since there are 3 different types of food and eating a different type of food should have a high weight
        }
        else
        {
            return false;
        }

        if (differenceAmount < 10)
            return true;

        return false;
    }
    private List<string> CopyAncestorList(List<string> toCopyList)
    {
        List<string> newList = new List<string>();

        for (int i = 0; i < toCopyList.Count; i++)
        {
            newList.Add(toCopyList[i]);
        }

        return newList;
    }
    private List<int> CopyAncestorIdsList(List<int> toCopyList)
    {
        List<int> newList = new List<int>();

        for (int i = 0; i < toCopyList.Count; i++)
        {
            newList.Add(toCopyList[i]);
        }

        return newList;
    }
    private void ZeroOutNegativeValues(ref Creature creature)
    {
        if (creature.EggCamo < 0) creature.EggCamo = 0;
        if (creature.EggIncubation < 0) creature.EggIncubation = 0;
        if (creature.EggInterval < 0) creature.EggInterval = 0;
        if (creature.EggToxicity < 0) creature.EggToxicity = 0;
        if (creature.FoodDigestion < 0) creature.FoodDigestion = 0;
        if (creature.FoodDigestion < 0) creature.FoodDigestion = 0;
        if (creature.Lifespan < 0) creature.Lifespan = 0;
        if (creature.Lifespan < 0) creature.Lifespan = 0;
        if (creature.Sight < 0) creature.Sight = 0;
        if (creature.Attraction < 0) creature.Attraction = 0;
        if (creature.Camo < 0) creature.Camo = 0;
        if (creature.Cloning < 0) creature.Cloning = 0;
        if (creature.ColdClimateTolerance < 0) creature.ColdClimateTolerance = 0;
        if (creature.HotClimateTolerance < 0) creature.HotClimateTolerance = 0;
        if (creature.Herbavore < 0) creature.Herbavore = 0;
        if (creature.Carnivore < 0) creature.Carnivore = 0;
        if (creature.Omnivore < 0) creature.Omnivore = 0;
        if (creature.Scavenger < 0) creature.Scavenger = 0;
        if (creature.FoodTypeBlue < 0) creature.FoodTypeBlue = 0;
        if (creature.FoodTypeRed < 0) creature.FoodTypeRed = 0;
        if (creature.FoodTypeGreen < 0) creature.FoodTypeGreen = 0;
    }
    private void DetermineCreatureType(float herbavore, float carnivore, float scavenger, float omnivore, out bool isHerbavore, out bool isCarnivore, out bool isScavenger, out bool isOmnivore)
    {
        if (herbavore >= carnivore && herbavore >= scavenger && herbavore >= omnivore)
        {
            isHerbavore = true;
            isCarnivore = false;
            isScavenger = false;
            isOmnivore = false;
        }
        else if (carnivore >= herbavore && carnivore >= scavenger && carnivore >= omnivore)
        {
            isHerbavore = false;
            isCarnivore = true;
            isScavenger = false;
            isOmnivore = false;
        }
        else if (scavenger >= herbavore && scavenger >= carnivore && scavenger >= omnivore)
        {
            isHerbavore = false;
            isCarnivore = false;
            isScavenger = true;
            isOmnivore = false;
        }
        else if (omnivore >= herbavore && omnivore >= carnivore && omnivore >= scavenger)
        {
            isHerbavore = true;
            isCarnivore = true;
            isScavenger = false;
            isOmnivore = true;
        }
        else
        {
            isHerbavore = true;
            isCarnivore = false;
            isScavenger = false;
            isOmnivore = false;
        }
    }
    public int GetCreatureTypeInt()
    {
        if (IsOmnivore)
            return 0;
        else if (IsHerbavore)
            return 1;
        else if (IsCarnivore)
            return 2;
        else
            return 3;
    }
    public string GetCreatureTypeText()
    {
        string creatureType = String.Empty;
        int creatureInt = GetCreatureTypeInt();

        switch (creatureInt)
        {
            case 0:
                creatureType = "Omnivore";
                break;
            case 1:
                creatureType = "Herbavore";
                break;
            case 2:
                creatureType = "Carnivore";
                break;
            default:
                creatureType = "Scavenger";
                break;
        }

        return creatureType;
    }
    private void SetCreatureColor()
    {
        if (IsHerbavore)
        {
            switch (FoodTypeInt)
            {
                case 0:
                    CreatureColor = Color.LightBlue;
                    break;
                case 1:
                    CreatureColor = Color.Salmon;
                    break;
                default:
                    CreatureColor = Color.LightGreen;
                    break;
            }
        }
        else
        {
            CreatureColor = Color.White;
        }
    }
}