using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EvolutionConquest
{
    public class Game1 : Game
    {
        //Framework variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private InputState _inputState;
        private Player _player;
        private SpriteFont _diagFont;
        private ConnectionManager _connectionManager;
        private int _diagTextHeight;
        private int _frames;
        private int _fps;
        private double _elapsedSeconds;
        private double _totalElapsedSeconds;
        private DebugTimer _debugTimer;
        private TimeSpan _resetTimeSpan;
        private Thread _statsThread;
        private Thread _plantSpreadThread;
        private Thread _collisionThread;
        private Thread _humanThread;
        private StatsThread _statsThreadClass;
        private PlantSpreadThread _plantSpreadThreadClass;
        private CollisionThread _collisionThreadClass;
        //Game variables
        private GameData _gameData;
        private SpriteFont _panelHeaderFont;
        private SpriteFont _foodFont;
        private SpriteFont _mapStatisticsFont;
        private Texture2D _whitePixel;
        private Texture2D _blankMarker;
        private Texture2D _eggMarker;
        private Texture2D _eggMarkerGreen;
        private Texture2D _eggMarkerRed;
        private Texture2D _eggMarkerPurple;
        private Texture2D _eggMarkerYellow;
        private Texture2D _herbavoreTexture;
        private Texture2D _herbavoreSightTexture;
        private Texture2D _carnivoreTexture;
        private Texture2D _carnivoreSightTexture;
        private Texture2D _scavengerTexture;
        private Texture2D _scavengerSightTexture;
        private Texture2D _omnivoreTexture;
        private Texture2D _omnivoreSightTexture;
        private Texture2D _foodTexture;
        private Texture2D _carcassTexture;
        private Texture2D _eggTexture;
        private Texture2D _populationTexture;
        private Texture2D _deadCreaturesTexture;
        private Texture2D _foodOnMapTexture;
        private Texture2D _eggsOnMapTexture;
        private Texture2D _plantsOnMapTexture;
        private Texture2D _saveButton;
        private Texture2D _closeButton;
        private Texture2D _defaultButton;
        private TabPanel _settingsTabPanel;
        private Random _rand;
        private int _gameRandSeed;
        private int _sessionID;
        private CreatureShapeGenerator _creatureGenerator;
        private FoodShapeGenerator _foodGenerator;
        private EggShapeGenerator _eggGenerator;
        private CarcassShapeGenerator _carcassGenerator;
        private Names _names;
        private Borders _borders;
        //private Chart _chart;
        private List<string> _controlsListText;
        private List<TextureContainer> _textureList;
        private double _elapsedSecondsSinceTick;
        private double _elapsedTimeSinceFoodGeneration;
        private double _elapsedTimeSinceDebugTimeData;
        private float _tickSeconds;
        private float _elapsedTicksSinceSecondProcessing;
        private int _creatureIdCtr;
        private int _elapsedTicksForInitialFoodUpgrade;
        private int _elapsedTicksSinceFoodUpgrade;
        private int _climateHeight;
        private bool _writeStats;
        private bool _chartPosSet;
        private float _foodGenerationIntervalSeconds;
        private float _foodLifeSpan;
        private TimeSpan _totalElapsedTime;
        //Constants
        private const int SESSION_NUMBER = 1;
        private const int MINUTES_TILL_GAMEOVER = 60;
        private const int MINUTES_TILL_DEBUG_TIME_DATA = 1;
        private const float SPRITE_FONT_SCALE = 0.5f;
        private const int TICKS_PER_SECOND = 10;
        private const int CREATURE_SPEED_BOOST = 3;
        private const int BORDER_WIDTH = 10;
        private const int GRID_CELL_SIZE = 50; //Seems to be the sweet spot for a 5,000 x 5,000 map based on the texture sizes we have so far
        private const float HUD_ICON_SCALE = 0.375f;
        private const int EVENT_LOG_DISPLAY_COUNT = 38;
        //GamePlay feature toggles
        private const bool ENABLE_GAME_RESETS = true;
        private const bool ENABLE_DEBUG_DATA = false;
        private const bool ENABLE_FOOD_UPGRADES = true;
        private const bool ENABLE_ENERGY_DEATH = true;
        private const bool ENABLE_SIGHT = true;
        private const bool ENABLE_CLIMATE = true;
        private const bool ENABLE_DATABASE_STATS = false;
        private const bool ENABLE_CSV_STATS = true;
        private const bool ENABLE_DEBUG_TIME_DATA = true;
        //Colors
        private Color MAP_COLOR = Color.SandyBrown;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            _resetTimeSpan = new TimeSpan(); //This must be initialized outside of the InitVariables function so that it doest not get reset
            _writeStats = true;
            _chartPosSet = false;

            IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            Global.Camera.ViewportWidth = _graphics.GraphicsDevice.Viewport.Width;
            Global.Camera.ViewportHeight = _graphics.GraphicsDevice.Viewport.Height;
            Global.Camera.CenterOn(new Vector2(Global.Camera.ViewportWidth / 2, Global.Camera.ViewportHeight / 2));

            base.Initialize();
        }
        protected override void LoadContent()
        {
            if (_statsThread != null && _statsThread.IsAlive)
            {
                _statsThread.Abort();
            }
            if (_humanThread != null && _humanThread.IsAlive)
            {
                _humanThread.Abort();
            }
            if (_collisionThread != null && _collisionThread.IsAlive)
            {
                _collisionThread.Abort();
            }
            if (_plantSpreadThread != null && _plantSpreadThread.IsAlive)
            {
                _plantSpreadThread.Abort();
                Thread.Sleep(500); //Give the threads some time to shutdown since the threads have Sleeps built in
            }

            //Load settings at the beginning
            _gameData = new GameData();
            _gameData.GraphicsDevice = _graphics.GraphicsDevice;
            _gameData.Settings = SettingsHelper.ReadSettings("Settings.json");
            _gameData.CreatureSettings = SettingsHelper.ReadCreatureSettings("CreatureSettings.json");
            _gameData.MutationSettings = SettingsHelper.ReadMutationSettings("MutationSettings.json");
            _gameData.MaxCreatureUndigestedFood = _gameData.MAX_UNDIGESTED_FOOD;
            _gameData.TicksPerSecond = (int)TICKS_PER_SECOND;
            _gameData.SetNewBestRun(); //Initialize the best run settings

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _textureList = new List<TextureContainer>();
            _populationTexture = Content.Load<Texture2D>("population");
            _deadCreaturesTexture = Content.Load<Texture2D>("dead");
            _foodOnMapTexture = Content.Load<Texture2D>("food");
            _eggsOnMapTexture = Content.Load<Texture2D>("eggs");
            _plantsOnMapTexture = Content.Load<Texture2D>("plant");
            _blankMarker = Content.Load<Texture2D>("BlankMarker");
            _eggMarker = Content.Load<Texture2D>("EggMarker");
            _eggMarkerGreen = Content.Load<Texture2D>("EggMarkerGreen");
            _eggMarkerRed = Content.Load<Texture2D>("EggMarkerRed");
            _eggMarkerPurple = Content.Load<Texture2D>("EggMarkerPurple");
            _eggMarkerYellow = Content.Load<Texture2D>("EggMarkerYellow");
            _saveButton = Content.Load<Texture2D>("Save");
            _closeButton = Content.Load<Texture2D>("Close");
            _defaultButton = Content.Load<Texture2D>("Default");

            Texture2D tree0 = Content.Load<Texture2D>("T0");
            Texture2D tree1 = Content.Load<Texture2D>("T1");
            Texture2D tree2 = Content.Load<Texture2D>("T2");
            Texture2D tree3 = Content.Load<Texture2D>("T3");
            Texture2D tree4 = Content.Load<Texture2D>("T4");
            Texture2D tree5 = Content.Load<Texture2D>("T5");

            _textureList.Add(new TextureContainer() { Name = "T0", Texture = tree0 });
            _textureList.Add(new TextureContainer() { Name = "T1", Texture = tree1 });
            _textureList.Add(new TextureContainer() { Name = "T2", Texture = tree2 });
            _textureList.Add(new TextureContainer() { Name = "T3", Texture = tree3 });
            _textureList.Add(new TextureContainer() { Name = "T4", Texture = tree4 });
            _textureList.Add(new TextureContainer() { Name = "T5", Texture = tree5 });

            _foodFont = Content.Load<SpriteFont>("FoodFont");
            _gameData.DebugFont = _foodFont;
            _panelHeaderFont = Content.Load<SpriteFont>("ArialBlack");
            _mapStatisticsFont = Content.Load<SpriteFont>("BEBAS___");
            _diagFont = Content.Load<SpriteFont>("DiagnosticsFont");
            _diagTextHeight = (int)Math.Ceiling(_diagFont.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZ[]").Y);
            _tickSeconds = TICKS_PER_SECOND;

            _whitePixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            Color[] color = new Color[1];
            color[0] = Color.White;
            _whitePixel.SetData(color);

            //Init variables after the settings are loaded
            InitVariables();

            //_sessionID = SESSION_NUMBER;
            _sessionID = _rand.Next(0, int.MaxValue); //We could have just used the Seed since it is random but I might implement the ability to choose seed at a later time
            //_rand = new Random(1);
            _gameRandSeed = _rand.Next(0, int.MaxValue); //Use the initial random class to set a seed
            _rand = new Random(_gameRandSeed); //Re-instantiate the _rand variable with our seed
            _names = new Names();
            _creatureGenerator = new CreatureShapeGenerator();
            _foodGenerator = new FoodShapeGenerator();
            _eggGenerator = new EggShapeGenerator();
            _carcassGenerator = new CarcassShapeGenerator();
            _herbavoreTexture = _creatureGenerator.CreateCreatureHerbavoreTexture(_graphics.GraphicsDevice, false);
            _herbavoreSightTexture = _creatureGenerator.CreateCreatureHerbavoreTexture(_graphics.GraphicsDevice, true);
            _carnivoreTexture = _creatureGenerator.CreateCreatureCarnivoreTexture(_graphics.GraphicsDevice, false);
            _carnivoreSightTexture = _creatureGenerator.CreateCreatureCarnivoreTexture(_graphics.GraphicsDevice, true);
            _scavengerTexture = _creatureGenerator.CreateCreatureScavengerTexture(_graphics.GraphicsDevice, false);
            _scavengerSightTexture = _creatureGenerator.CreateCreatureScavengerTexture(_graphics.GraphicsDevice, true);
            _omnivoreTexture = _creatureGenerator.CreateCreatureOmnivoreTexture(_graphics.GraphicsDevice, false);
            _omnivoreSightTexture = _creatureGenerator.CreateCreatureOmnivoreTexture(_graphics.GraphicsDevice, true);
            _foodTexture = _foodGenerator.CreateFoodTexture(_graphics.GraphicsDevice);
            _eggTexture = _eggGenerator.CreateEggTexture(_graphics.GraphicsDevice, Color.Black, Color.White);
            _carcassTexture = _carcassGenerator.CreateCarcassTexture(_graphics.GraphicsDevice);
            _gameData.GameSeed = _gameRandSeed;
            _gameData.SessionID = _sessionID;
            _gameData.TotalElapsedSeconds = _totalElapsedSeconds;

            //Generate the Map
            _borders = new Borders();
            _borders.Texture = _whitePixel;
            _borders.LeftWall = new Vector2(0, 0);
            _borders.RightWall = new Vector2(_gameData.Settings.WorldSize, 0);
            _borders.TopWall = new Vector2(0, 0);
            _borders.BottomWall = new Vector2(0, _gameData.Settings.WorldSize);

            //Initialize the Grid
            int gridWidth = (int)Math.Ceiling((double)_gameData.Settings.WorldSize / GRID_CELL_SIZE);

            _gameData.MapGridData = new GridData[gridWidth, gridWidth];

            //Loop through grid and set Rectangle on each cell, named iterators x,y to help avoid confusion
            for (int y = 0; y < _gameData.MapGridData.GetLength(0); y++)
            {
                for (int x = 0; x < _gameData.MapGridData.GetLength(1); x++)
                {
                    _gameData.MapGridData[x, y] = new GridData();
                    _gameData.MapGridData[x, y].Creatures = new List<Creature>();
                    _gameData.MapGridData[x, y].Eggs = new List<Egg>();
                    _gameData.MapGridData[x, y].Food = new List<Food>();
                    _gameData.MapGridData[x, y].Plants = new List<Plant>();

                    Rectangle rec = new Rectangle();
                    rec.X = x * GRID_CELL_SIZE;
                    rec.Y = y * GRID_CELL_SIZE;
                    rec.Width = GRID_CELL_SIZE;
                    rec.Height = GRID_CELL_SIZE;

                    _gameData.MapGridData[x, y].CellRectangle = rec;
                }
            }

            //Attempt to give starting creatures a better chance of finding food in the beginning to avoid large extinctions to open the game
            //Load in random food
            int amountOfFood = (int)(((_gameData.Settings.WorldSize * _gameData.Settings.WorldSize) / _foodTexture.Width) * (_gameData.Settings.StartingFoodRatio / 100000f));
            //Spawn in fast despawning food
            for (int i = 0; i < amountOfFood; i++)
            {
                SpawnFood(_rand.Next((_gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN / 10 - _gameData.INITIAL_SPAWN_FOOD_VARIANCE / 10), ((_gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN / 10) + _gameData.INITIAL_SPAWN_FOOD_VARIANCE / 10)));
            }
            //Spawn in medium despawning food
            for (int i = 0; i < amountOfFood; i++)
            {
                SpawnFood(_rand.Next((_gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN / 4 - _gameData.INITIAL_SPAWN_FOOD_VARIANCE / 4), ((_gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN / 4) + _gameData.INITIAL_SPAWN_FOOD_VARIANCE / 4)));
            }
            //Spawn in food that does not despawn
            for (int i = 0; i < amountOfFood / 2; i++)
            {
                //SpawnFood(_rand.Next(_gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN - _gameData.INITIAL_SPAWN_FOOD_VARIANCE, _gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN + _gameData.INITIAL_SPAWN_FOOD_VARIANCE));
                SpawnFood(-1f);
            }

            //Spawn in starting Plants
            int startingPlantAmount = (int)Math.Ceiling((((_gameData.Settings.WorldSize * _gameData.Settings.WorldSize) / ((_herbavoreTexture.Width + _herbavoreTexture.Height) / 2)) * (_gameData.Settings.StartingPlantRatio / 1000000f)));
            for (int i = 0; i < startingPlantAmount; i++)
            {
                SpawnStartingPlant();
            }

            //Game start, load in starting population of creatures
            int startingCreatureAmount = (int)Math.Ceiling((((_gameData.Settings.WorldSize * _gameData.Settings.WorldSize) / ((_herbavoreTexture.Width + _herbavoreTexture.Height) / 2)) * (_gameData.Settings.StartingCreatureRatio / 1000000f)));
            for (int i = 0; i < startingCreatureAmount; i++)
            {
                SpawnStartingCreature();
            }

            //Calculate the food spawn speed
            //_foodGenerationIntervalSeconds = (_gameData.Settings.FoodGenerationValue * 1000f) / (_gameData.Settings.WorldSize * _gameData.Settings.WorldSize);
            _foodGenerationIntervalSeconds = 34000000 / (_gameData.Settings.FoodGenerationValue * (_gameData.Settings.WorldSize * _gameData.Settings.WorldSize));
            //_foodLifeSpan = 9000f + (_gameData.Settings.FoodGenerationValue * 100f);
            _foodLifeSpan = 3125;
            //_foodLifeSpan = (_gameData.Settings.FoodGenerationValue * 1000f);

            //Generate initial Map stats so that the stats do not read all 0's at the beginning
            _gameData.CalculateMapStatistics();

            Global.Camera.AdjustZoom(500);

            //Game Controls list for HUD
            _controlsListText = new List<string>();
            //Controls: [W][A][S][D] Camera Pan, [PageUp][PageDown] Iterate Creatures, [Shift] + [PageUp][PageDown] Iterate Species, [F12] Toggle Chart";
            _controlsListText.Add("[W][A][S][D] Camera Pan");
            _controlsListText.Add("[Scroll Wheel] Zoom");
            _controlsListText.Add("[PageUp][PageDown] Cycle Creatures");
            _controlsListText.Add("[Shift] + [PageUp][PageDown] Cycle Species");
            _controlsListText.Add("[Ctrl] + [PageUp][PageDown] Cycle Creatures");
            _controlsListText.Add("[F1] Toggle Settings Panel");
            _controlsListText.Add("[F2] Toggle Event Log Panel");
            _controlsListText.Add("[F4] Toggle Herbavore Marker");
            _controlsListText.Add("[F5] Toggle Carnivore Marker");
            _controlsListText.Add("[F6] Toggle Scavenger Marker");
            _controlsListText.Add("[F7] Toggle Omnivore Marker");
            _controlsListText.Add("[F8] Toggle Debug Data");
            _controlsListText.Add("[F9] Toggle Food Strength");
            _controlsListText.Add("[F10] Toggle Creature Statistics");
            _controlsListText.Add("[F11] Toggle Chart");
            _controlsListText.Add("[Left Click] Follow/Unfollow Creature");
            _controlsListText.Add("[F] Highlight Species");
            _controlsListText.Add("[H] Focus Top Herbavore");
            _controlsListText.Add("[C] Focus Top Carnivore");
            _controlsListText.Add("[V] Focus Top Scavenger");
            _controlsListText.Add("[O] Focus Top Omnivore");
            _controlsListText.Add(" ");
            _controlsListText.Add("[F12] Toggle Control Menu");

            ////Create the chart
            //_chart = new Chart();
            //_chart.Width = 600;
            //_chart.Height = 300;
            //_chart.Location = new System.Drawing.Point(_graphics.PreferredBackBufferWidth - _chart.Width - 2, _graphics.PreferredBackBufferHeight - _chart.Height - 2);
            //_chart.Text = "Test";
            //_chart.Visible = false;
            //ChartArea chartArea1 = new ChartArea();
            //chartArea1.Name = "ChartArea1";
            //_chart.ChartAreas.Add(chartArea1);
            //Legend legend = new Legend();
            //_chart.Legends.Add(legend);

            //Control.FromHandle(Window.Handle).Controls.Add(_chart);

            if (ENABLE_DATABASE_STATS && _writeStats)
            {
                try
                {
                    //Establish a connection to the SQL Server database, we do not want to run a whole simulation only to find out the SQL Server database is not accessible
                    SqlConnection tmpConnection = _connectionManager.GetSqlConnection(_gameData.Settings);
                    tmpConnection.Close(); //Close the connection right away so that our connection does not timeout while the simulation/game runs
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to establish conncetion to database. Stats will not be written. Ex: " + ex.Message);
                    _writeStats = false;
                }
            }
            else if (ENABLE_CSV_STATS && _writeStats)
            {
                //Build the SessionID folder to hold the stats and this doubles as a check to make sure we can write files
                System.IO.Directory.CreateDirectory(_sessionID.ToString());
            }

            BuildSettingsPanel();

            _statsThreadClass = new StatsThread(_gameData, _rand);
            _statsThread = new Thread(new ThreadStart(_statsThreadClass.Start));
            _statsThread.Start();

            _plantSpreadThreadClass = new PlantSpreadThread(_gameData, _rand, GRID_CELL_SIZE);
            _plantSpreadThread = new Thread(new ThreadStart(_plantSpreadThreadClass.Start));
            _plantSpreadThread.Start();

            _collisionThreadClass = new CollisionThread(_gameData, _rand);
            _collisionThread = new Thread(new ThreadStart(_collisionThreadClass.Start));
            _collisionThread.Start();

            ////Test creatures
            //SpawnTwoTestCreaturesWithInterceptPaths();
            //SpawnOneTestCarnivore();
            //SpawnOneTestScavenger();
        }
        protected override void UnloadContent()
        {
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            if (_statsThread != null && _statsThread.IsAlive)
            {
                _statsThread.Abort();
            }
            if (_plantSpreadThread != null && _plantSpreadThread.IsAlive)
            {
                _plantSpreadThread.Abort();
            }
            if (_humanThread != null && _humanThread.IsAlive)
            {
                _humanThread.Abort();
            }
            if (_collisionThread != null && _collisionThread.IsAlive)
            {
                _collisionThread.Abort();
            }

            base.OnExiting(sender, args);
        }
        protected override void Update(GameTime gameTime)
        {
            try
            {
                bool tick = false;

                _elapsedTimeSinceDebugTimeData += gameTime.ElapsedGameTime.TotalSeconds;
                if (_elapsedTimeSinceDebugTimeData / 60 >= MINUTES_TILL_DEBUG_TIME_DATA)
                {
                    //Write Debug file
                    List<string> csvDebugTimerFile = BuildDebugTimerList();
                    System.IO.File.WriteAllLines(System.IO.Path.Combine(_sessionID.ToString(), "DebugTimerFileRolling.csv"), csvDebugTimerFile.ToArray());
                    csvDebugTimerFile = null; //Let GC cleanup the RAM before we build the next list

                    _elapsedTimeSinceDebugTimeData = 0;
                    _debugTimer = new DebugTimer();
                }

                _totalElapsedTime = (gameTime.TotalGameTime - _resetTimeSpan);
                _gameData.ElapsedTimeSinceFitnessCalculation += gameTime.ElapsedGameTime.TotalSeconds;

                DateTime debugDttm = DateTime.Now;
                UpdateHandleInputs(gameTime);
                _debugTimer.TimeUpdateHandleInputs += (DateTime.Now - debugDttm).TotalMilliseconds;

                if (_gameData.ResetGame)
                {
                    ResetGame(gameTime);
                    _gameData.ResetGame = false;
                }
                if (_gameData.BuildSettingsPanel)
                {
                    BuildSettingsPanel();
                    _gameData.BuildSettingsPanel = false;
                }

                if ((gameTime.TotalGameTime - _resetTimeSpan).TotalMinutes < MINUTES_TILL_GAMEOVER)
                {
                    _tickSeconds = 1f / TICKS_PER_SECOND;

                    _elapsedSecondsSinceTick += gameTime.ElapsedGameTime.TotalSeconds;
                    _elapsedTimeSinceFoodGeneration += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_elapsedSecondsSinceTick > _tickSeconds)
                    {
                        _elapsedSecondsSinceTick = _elapsedSecondsSinceTick - _tickSeconds; //Start the next tick with the overage
                        tick = true;
                    }

                    //During a tick do all creature processing
                    if (tick)
                    {
                        debugDttm = DateTime.Now;
                        UpdateTick(gameTime);
                        _debugTimer.TimeUpdateTick += (DateTime.Now - debugDttm).TotalMilliseconds;
                    }
                    else //Off tick processing
                    {
                        debugDttm = DateTime.Now;
                        UpdateOffTick(gameTime);
                        _debugTimer.TimeUpdateOffTick += (DateTime.Now - debugDttm).TotalMilliseconds;
                    }
                }
                else
                {
                    //Simulation has ended. Time to write stats to the database
                    UpdateHandleEndOfSimulation(gameTime);
                }

                //This must be after movement caluclations occur for the creatures otherwise the camera will glitch back and forth
                if (_gameData.Focus != null)
                {
                    Global.Camera.CenterOn(_gameData.Focus.Position);
                }

                UpdateHandleObjectsToBeDrawn(gameTime);

                base.Update(gameTime);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(_sessionID.ToString(), "ErrorLogUpdate.txt"), DateTime.Now.ToString() + " - UPDATE Uncaught error: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            try
            {
                GraphicsDevice.Clear(MAP_COLOR);

                //FPS Counter
                _totalElapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;
                _gameData.TotalElapsedSeconds = _totalElapsedSeconds; //Keep two variables in sync
                if (_elapsedSeconds >= 1)
                {
                    _fps = _frames;
                    _frames = 0;
                    _elapsedSeconds = 0;
                }
                _frames++;
                _elapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;

                if (_fps >= 30)
                    if (1 == 1)
                    { }

                // === DRAW WITHIN THE WORLD ===
                DateTime debugDttm = DateTime.Now;
                DrawWorldObjects();
                _debugTimer.TimeDrawWorldObjects += (DateTime.Now - debugDttm).TotalMilliseconds;
                //=== DRAW HUD INFORMATION, DOES NOT DRAW TO WORLD SCALE ===
                debugDttm = DateTime.Now;
                DrawHUD();
                _debugTimer.TimeDrawHUD += (DateTime.Now - debugDttm).TotalMilliseconds;

                base.Draw(gameTime);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(_sessionID.ToString(), "ErrorLogDraw.txt"), DateTime.Now.ToString() + " - DRAW Uncaught error: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
            }
        }

        //Update functions
        private void UpdateTick(GameTime gameTime)
        {
            _gameData.TickElapsedPlants = true;
            _elapsedTicksSinceSecondProcessing++;

            //Check eggs before creatures so that the baby can follow cretaure update logic when born such as eating food
            DateTime debugDttm = DateTime.Now;
            UpdateTickEggHatching(gameTime);
            _debugTimer.TimeUpdateTickEggHatching += (DateTime.Now - debugDttm).TotalMilliseconds;

            //Creature logic
            debugDttm = DateTime.Now;
            UpdateTickCreature(gameTime);
            _debugTimer.TimeUpdateTickCreature += (DateTime.Now - debugDttm).TotalMilliseconds;

            //Plant logic
            //debugDttm = DateTime.Now;
            //UpdateTickPlant(gameTime);
            //_debugTimer.TimeUpdateTickPlant += (DateTime.Now - debugDttm).TotalMilliseconds;

            //Food logic
            debugDttm = DateTime.Now;
            UpdateTickFood(gameTime);
            _debugTimer.TimeUpdateTickFood += (DateTime.Now - debugDttm).TotalMilliseconds;
        }
        private void UpdateTickEggHatching(GameTime gameTime)
        {
            for (int i = _gameData.Eggs.Count - 1; i >= 0; i--)
            {
                _gameData.Eggs[i].AdvanceTick();
                //Check to egg hatched
                if (_gameData.Eggs[i].ElapsedTicks >= _gameData.Eggs[i].TicksTillHatched)
                {
                    //TODO change texture based on creature properties dynamically
                    //Assign texture to the creature
                    _gameData.Eggs[i].Creature.Texture = DetermineCreatureTexture(_gameData.Eggs[i].Creature);
                    _gameData.Creatures.Add(_gameData.Eggs[i].Creature);
                    _gameData.RemoveEggFromGrid(_gameData.Eggs[i], _gameData.Eggs[i].GridPositions);
                    _gameData.Eggs.RemoveAt(i);
                }
            }
        }
        private void UpdateTickCreature(GameTime gameTime)
        {
            for (int i = _gameData.Creatures.Count - 1; i >= 0; i--)
            {
                if (_gameData.Creatures[i].IsAlive)
                {
                    _gameData.Creatures[i].AdvanceTick(_rand);

                    //Check if the creature has died
                    if (_gameData.Creatures[i].ElapsedTicks > _gameData.Creatures[i].LifespanActual || (ENABLE_ENERGY_DEATH && _gameData.Creatures[i].Energy < 0))
                    {
                        int speciesId = _gameData.Creatures[i].SpeciesId;
                        string speciesName = _gameData.Creatures[i].Species;
                        string speciesType = _gameData.Creatures[i].GetCreatureTypeText();

                        _gameData.Creatures[i].IsAlive = false;
                        if (ENABLE_ENERGY_DEATH && _gameData.Creatures[i].Energy < 0)
                        {
                            _gameData.Creatures[i].Energy = 0; //Not possible to have negative energy
                            _gameData.Creatures[i].DeathCause = "Energy";
                        }
                        else
                        {
                            _gameData.Creatures[i].DeathCause = "Age";
                        }
                        _gameData.AddDeadCreatureToList(_gameData.Creatures[i]);

                        //Drop all food on the ground randomly around the area
                        for (int k = 0; k < _gameData.Creatures[i].UndigestedFood; k++)
                        {
                            SpawnFood(new Vector2(_gameData.Creatures[i].Position.X + _rand.Next(-15, 15), _gameData.Creatures[i].Position.Y + _rand.Next(-15, 15)), _gameData.Creatures[i].Herbavore);
                        }
                        //Spawn in Carcass food for scavengers
                        if (!_gameData.Creatures[i].IsScavenger && _rand.Next(0, 100) > 50)
                        {
                            SpawnFood(new Vector2(_gameData.Creatures[i].Position.X, _gameData.Creatures[i].Position.Y), _gameData.Creatures[i].Herbavore, _gameData.CARCASS_LIFESPAN, true);
                        }

                        if (_gameData.Focus == _gameData.Creatures[i])
                        {
                            _gameData.Focus = null;
                            _gameData.FocusIndex = -1;
                        }
                        //_gameData.RemoveCreatureFromGrid(_gameData.Creatures[i], _gameData.Creatures[i].GridPositions);
                        //_gameData.Creatures.RemoveAt(i);

                        continue;
                    }
                    //Check if we can lay a new egg
                    if (_gameData.Creatures[i].DigestedFood > 0 && _gameData.Creatures[i].TicksSinceLastEgg >= _gameData.Creatures[i].EggIntervalActual && _gameData.Creatures[i].Energy >= _gameData.Creatures[i].GetEggCreateEnergyLoss(_gameData.Settings.EnergyConsumptionFromLayingEgg))
                    {
                        _gameData.Creatures[i].DigestedFood--; //Costs one digested food to lay an egg
                        Egg egg = _gameData.Creatures[i].LayEgg(_rand, ref _names, _gameData, ref _creatureIdCtr);
                        //TODO handle this maybe in the Creature class
                        egg.Texture = _eggTexture;
                        egg.GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);
                        _gameData.AddEggToGrid(egg);
                        _gameData.Eggs.Add(egg); //Add the new egg to gameData, the LayEgg function will calculate the Mutations
                        _gameData.Creatures[i].EggCreateEnergyLoss(_gameData.Settings.EnergyConsumptionFromLayingEgg); //Pass in the CONST energy loss so that we can do additional calculations
                    }
                    //Do vision checks
                    if (ENABLE_SIGHT)
                    {
                        //UpdateCreatureSight(gameTime, i);
                    }

                    UpdateMoveCreature(gameTime, i);
                }
            }
        }
        private void UpdateTickFood(GameTime gameTime)
        {
            if (ENABLE_FOOD_UPGRADES)
            {
                _elapsedTicksForInitialFoodUpgrade++;
                if (_elapsedTicksForInitialFoodUpgrade >= _gameData.Settings.TicksUntilFoodUpgradeStarts * TICKS_PER_SECOND)
                {
                    if (_gameData.CurrentMaxFoodLevel < _gameData.Settings.MaxFoodLevel)
                    {
                        _elapsedTicksSinceFoodUpgrade++;
                        if (_elapsedTicksSinceFoodUpgrade >= _gameData.Settings.TicksBetweenFoodUpgrades * TICKS_PER_SECOND)
                        {
                            //Chance to increase the food level
                            if (_rand.Next(0, 100) <= _gameData.Settings.FoodUpgradeChancePercent)
                            {
                                _elapsedTicksSinceFoodUpgrade = 0;
                                _gameData.CurrentMaxFoodLevel += _gameData.Settings.FoodUpgradeAmount;
                            }
                        }
                    }
                }
            }

            //Remove food that has expired
            for (int i = _gameData.Food.Count - 1; i >= 0; i--)
            {
                try
                {
                    _gameData.Food[i].ElapsedTicks++;

                    if (_gameData.Food[i].Lifespan > 0 && _gameData.Food[i].ElapsedTicks > _gameData.Food[i].LifespanActual)
                    {
                        _gameData.Food[i].MarkForDelete = true;
                    }
                }
                catch (Exception) //Ignore this error, for performance reasons we do not want to lock the variable thus we will ignore errors
                { }
            }
        }
        private void UpdateTickPlant(GameTime gameTime)
        {
            lock (_gameData.LockPlants)
            {
                for (int i = _gameData.Plants.Count - 1; i >= 0; i--)
                {
                    _gameData.Plants[i].AdvanceTick(_rand);

                    //Check if plant has died
                    if (_gameData.Plants[i].TicksSinceBirth >= _gameData.Plants[i].Lifespan)
                    {
                        _gameData.Plants[i].MarkForDelete = true;
                    }
                }
            }
        }
        private void UpdateOffTick(GameTime gameTime)
        {
            //Spawn new food
            DateTime debugDttm = DateTime.Now;
            UpdateOffTickSpawnFood(gameTime);
            _debugTimer.TimeUpdateOffTickSpawnFood += (DateTime.Now - debugDttm).TotalMilliseconds;

            //Collisions And Movement
            debugDttm = DateTime.Now;
            UpdateOffTickHandleCollisionsAndMovement(gameTime);
            _debugTimer.TimeUpdateOffTickHandleCollisionsAndMovement += (DateTime.Now - debugDttm).TotalMilliseconds;

            //Every second interval processing only when it is not a TICK. Some calcs only needs to be updated once every X seconds
            if (_elapsedTicksSinceSecondProcessing >= TICKS_PER_SECOND * 5)
            {
                debugDttm = DateTime.Now;
                UpdateOffTickInterval(gameTime);
                _debugTimer.TimeUpdateOffTickInterval += (DateTime.Now - debugDttm).TotalMilliseconds;
            }

            if (_gameData.ElapsedTimeSinceFitnessCalculation >= GameData.SECONDS_BETWEEN_FITNESS_CALC)
            {
                double fitnessScore = CalculateFitness();

                _gameData.ElapsedTimeSinceFitnessCalculation -= GameData.SECONDS_BETWEEN_FITNESS_CALC;
                _gameData.TotalFitnessPoints += fitnessScore;
                _gameData.NumberOfFitnessCalculations++;
            }
        }
        private void UpdateOffTickInterval(GameTime gameTime)
        {
            _elapsedTicksSinceSecondProcessing = 0;

            if (!_chartPosSet)
            {
                UpdateOffTickIntervalGraphs(gameTime);
            }
            UpdateOffTickIntervalMapStats(gameTime);
            UpdateOffTickIntervalObjectCleanup(gameTime);
        }
        private void UpdateOffTickIntervalGraphs(GameTime gameTime)
        {
            if (_gameData.ChartDataTop.Count > 0 && _gameData.ChartTexture != null && _graphics.PreferredBackBufferWidth > 0)
            {
                _gameData.ChartPosition = new Point(_graphics.PreferredBackBufferWidth - _gameData.ChartTexture.Width - 2, _graphics.PreferredBackBufferHeight - _gameData.ChartTexture.Height - 2);
                _chartPosSet = true;
            }

            //Generate Graph data
            //Moved to Stats thread
            //_gameData.CalculateChartData(_rand); //This will populat the Chart Data in _gameData. Even if we hide the chart we need to keep track of ChartData

            //if (_gameData.ShowChart)
            //{
            //    _gameData.LockChart.Locker = "Main";
            //    lock (_gameData.LockChart)
            //    {
            //        if (!_chart.Visible && _gameData.ChartDataTop.Count > 0 && !_gameData.ShowSettingsPanel)
            //        {
            //            _chart.Visible = true;
            //        }
            //        if (_chart.Series != null)
            //        {
            //            _chart.Series.Clear();
            //            for (int i = 0; i < _gameData.ChartDataTop.Count; i++)
            //            {
            //                int? count = _gameData.ChartDataTop[i].CountsOverTime[_gameData.ChartDataTop[i].CountsOverTime.Count - 1];
            //                string name = String.Empty;
            //                //System.Drawing.Color seriesColor = System.Drawing.Color.White;

            //                name = _gameData.ChartDataTop[i].Name;

            //                if (name.Length > 15)
            //                    name = name.Substring(0, 12) + "...";

            //                if (count != null)
            //                {
            //                    name += "(" + count + ")";
            //                }
            //                if (!String.IsNullOrEmpty(_gameData.ChartDataTop[i].CreatureType))
            //                {
            //                    name = "(" + _gameData.ChartDataTop[i].CreatureType.Substring(0, 1) + ")" + name;
            //                }

            //                _chart.Series.Add(name);
            //                _chart.Series[name].XValueType = ChartValueType.Int32;
            //                _chart.Series[name].ChartType = SeriesChartType.StackedArea100;
            //                _chart.Series[name].BorderWidth = 3;
            //                if (_gameData.ChartDataTop[i].ChartColor != System.Drawing.Color.White)
            //                {
            //                    _chart.Series[name].Color = _gameData.ChartDataTop[i].ChartColor;
            //                }

            //                for (int k = 0; k < _gameData.ChartDataTop[i].CountsOverTime.Count; k++)
            //                {
            //                    _chart.Series[name].Points.AddXY(k, _gameData.ChartDataTop[i].CountsOverTime[k]);
            //                }
            //            }
            //        }
            //    }
            //    _gameData.LockChart.Locker = "";
            //}
        }
        private void UpdateOffTickIntervalMapStats(GameTime gameTime)
        {
            _gameData.CalculateMapStatistics();
        }
        private void UpdateOffTickIntervalObjectCleanup(GameTime gameTime)
        {
            lock (_gameData.LockFood)
            {
                for (int i = _gameData.Food.Count - 1; i >= 0; i--)
                {
                    if (_gameData.Food[i].MarkForDelete)
                    {
                        _gameData.RemoveFoodFromGrid(_gameData.Food[i], _gameData.Food[i].GridPositions);
                        _gameData.Food.Remove(_gameData.Food[i]);
                    }
                }
            }
            lock (_gameData.LockPlants)
            {
                for (int i = _gameData.Plants.Count - 1; i >= 0; i--)
                {
                    //Check if plant has died
                    if (_gameData.Plants[i].MarkForDelete)
                    {
                        _gameData.RemovePlantFromGrid(_gameData.Plants[i], _gameData.Plants[i].GridPositions);
                        _gameData.Plants.RemoveAt(i);
                    }
                    else
                    {
                        for (int k = _gameData.Plants[i].Interactions.Count - 1; k >= 0; k--)
                        {
                            if (_gameData.Plants[i].Interactions[k].ElapsedTicks >= _gameData.Plants[i].EatCooldownTicksActual)
                            {
                                _gameData.Plants[i].Interactions.RemoveAt(k);
                            }
                        }
                    }
                }
            }
            if (String.IsNullOrEmpty(_gameData.LockCreatures.Locker))
            {
                lock (_gameData.LockCreatures)
                {
                    for (int i = _gameData.Creatures.Count - 1; i >= 0; i--)
                    {
                        if (!_gameData.Creatures[i].IsAlive)
                        {
                            //Check to see if this is the last of the species to show in EventLog
                            int speciesId = _gameData.Creatures[i].SpeciesId;
                            string speciesType = _gameData.Creatures[i].GetCreatureTypeText();
                            string speciesName = _gameData.Creatures[i].Species;
                            bool found = false;

                            foreach (Creature c in _gameData.Creatures)
                            {
                                if (c.SpeciesId == speciesId && c.IsAlive)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                foreach (Egg egg in _gameData.Eggs)
                                {
                                    if (egg.Creature.SpeciesId == speciesId)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (!found)
                            {
                                _gameData.EventLog.Add("$[255,000,000]" + speciesType + " species '" + speciesName + "' has gone extinct");
                            }

                            _gameData.RemoveCreatureFromGrid(_gameData.Creatures[i], _gameData.Creatures[i].GridPositions);
                            _gameData.Creatures.RemoveAt(i);
                        }
                    }
                }
            }
            UpdateOffTickEventCheckCleanup(gameTime);
        }
        private void UpdateOffTickHandleCollisionsAndMovement(GameTime gameTime)
        {
            List<Creature> deadCreaturesToRemove = new List<Creature>();

            //CollisionDetection
            //Border Collision Detection
            for (int i = 0; i < _gameData.Creatures.Count; i++)
            {
                if (_gameData.Creatures[i].IsAlive)
                {
                    if (_gameData.Creatures[i].Position.X - (_gameData.Creatures[i].Texture.Width / 2) <= 0 || _gameData.Creatures[i].Position.X + (_gameData.Creatures[i].Texture.Width / 2) >= _gameData.Settings.WorldSize)
                    {
                        if (_gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y >= 0 ||
                            _gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y < 0 ||
                            _gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y >= 0 ||
                            _gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y < 0)
                        {
                            _gameData.Creatures[i].Rotation = (((float)Math.PI * 2) - _gameData.Creatures[i].Rotation);
                        }
                    }
                    if (_gameData.Creatures[i].Position.Y - (_gameData.Creatures[i].Texture.Height / 2) <= 0 || _gameData.Creatures[i].Position.Y + (_gameData.Creatures[i].Texture.Height / 2) >= _gameData.Settings.WorldSize)
                    {
                        if (_gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y >= 0 ||
                            _gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y < 0 ||
                            _gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y >= 0 ||
                            _gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y < 0)
                        {
                            _gameData.Creatures[i].Rotation = (((float)Math.PI) - _gameData.Creatures[i].Rotation);
                        }
                    }

                    ////Food collision
                    //if ((_gameData.Creatures[i].IsHerbavore || _gameData.Creatures[i].IsScavenger) && _gameData.Creatures[i].UndigestedFood < _gameData.MaxCreatureUndigestedFood)
                    //{
                    //    foreach (Point p in _gameData.Creatures[i].GridPositions)
                    //    {
                    //        for (int k = (_gameData.MapGridData[p.X, p.Y].Food.Count - 1); k >= 0; k--)
                    //        {
                    //            if (_gameData.Creatures[i].FoodTypeInt == _gameData.MapGridData[p.X, p.Y].Food[k].FoodType && (_gameData.Creatures[i].IsScavenger || _gameData.Creatures[i].Herbavore >= _gameData.MapGridData[p.X, p.Y].Food[k].FoodStrength))
                    //            {
                    //                if (_gameData.Creatures[i].Bounds.Intersects(_gameData.MapGridData[p.X, p.Y].Food[k].Bounds))
                    //                {
                    //                    Food tmpFood = _gameData.MapGridData[p.X, p.Y].Food[k];
                    //                    _gameData.Creatures[i].UndigestedFood++;
                    //                    _gameData.Creatures[i].TotalFoodEaten++;
                    //                    _gameData.Creatures[i].Energy += _gameData.Settings.EnergyGivenFromFood + (_gameData.Creatures[i].FoodDigestion / 10); //Slower food digestion means you pull more energy from the food
                    //                    _gameData.RemoveFoodFromGrid(tmpFood, _gameData.MapGridData[p.X, p.Y].Food[k].GridPositions);
                    //                    _gameData.Food.Remove(tmpFood);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    ////Plant Collision
                    //if ((_gameData.Creatures[i].IsHerbavore && !_gameData.Creatures[i].IsOmnivore) && _gameData.Creatures[i].UndigestedFood < MAX_UNDIGESTED_FOOD)
                    //{
                    //    foreach (Point p in _gameData.Creatures[i].GridPositions)
                    //    {
                    //        for (int k = (_gameData.MapGridData[p.X, p.Y].Plants.Count - 1); k >= 0; k--)
                    //        {
                    //            if (_gameData.Creatures[i].Herbavore >= _gameData.MapGridData[p.X, p.Y].Plants[k].FoodStrength)
                    //            {
                    //                if (_gameData.Creatures[i].Bounds.Intersects(_gameData.MapGridData[p.X, p.Y].Plants[k].Bounds))
                    //                {
                    //                    //Make sure the creature is not on cooldown before attempting to eat from the food
                    //                    if (_gameData.MapGridData[p.X, p.Y].Plants[k].Interactions.Count(t => t.CreatureID == _gameData.Creatures[i].CreatureId) <= 0)
                    //                    {
                    //                        float foodAwarded = _gameData.MapGridData[p.X, p.Y].Plants[k].Eat(_gameData.Creatures[i].CreatureId);

                    //                        if (foodAwarded > 0)
                    //                        {
                    //                            _gameData.Creatures[i].UndigestedFood += foodAwarded;
                    //                            _gameData.Creatures[i].TotalFoodEaten++;
                    //                            _gameData.Creatures[i].Energy += ((_gameData.Settings.EnergyGivenFromFood + (_gameData.Creatures[i].FoodDigestion / 10)) * foodAwarded) * 1.1f; //Slower food digestion means you pull more energy from the food
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    if (_gameData.Creatures[i].IsScavenger && _gameData.Creatures[i].UndigestedFood < _gameData.MaxCreatureUndigestedFood) //Scavenger Egg collision
                    {
                        foreach (Point p in _gameData.Creatures[i].GridPositions)
                        {
                            for (int k = (_gameData.MapGridData[p.X, p.Y].Eggs.Count - 1); k >= 0; k--)
                            {
                                if (_gameData.Creatures[i].SpeciesId != _gameData.MapGridData[p.X, p.Y].Eggs[k].Creature.SpeciesId)
                                {
                                    if (_gameData.Creatures[i].Bounds.Intersects(_gameData.MapGridData[p.X, p.Y].Eggs[k].Bounds))
                                    {
                                        Egg tmpEgg = _gameData.MapGridData[p.X, p.Y].Eggs[k];
                                        _gameData.Creatures[i].UndigestedFood++;
                                        _gameData.Creatures[i].TotalFoodEaten++;
                                        _gameData.Creatures[i].Energy += _gameData.Settings.EnergyGivenFromFood + (_gameData.Creatures[i].FoodDigestion / 10); //Slower food digestion means you pull more energy from the food
                                        _gameData.RemoveEggFromGrid(tmpEgg, _gameData.MapGridData[p.X, p.Y].Eggs[k].GridPositions);
                                        _gameData.Eggs.Remove(tmpEgg);
                                    }
                                }
                            }
                        }
                    }
                    else if (_gameData.Creatures[i].IsCarnivore && _gameData.Creatures[i].UndigestedFood < _gameData.MaxCreatureUndigestedFood) //Carnivore creature collision
                    {
                        foreach (Point p in _gameData.Creatures[i].GridPositions)
                        {
                            for (int k = (_gameData.MapGridData[p.X, p.Y].Creatures.Count - 1); k >= 0; k--)
                            {
                                if (_gameData.Creatures[i].SpeciesId != _gameData.MapGridData[p.X, p.Y].Creatures[k].SpeciesId)
                                {
                                    if ((_gameData.MapGridData[p.X, p.Y].Creatures[k].IsHerbavore || (_gameData.MapGridData[p.X, p.Y].Creatures[k].IsScavenger && (_gameData.Creatures[i].Carnivore - _gameData.Settings.CarnivoreLevelBuffer) > _gameData.MapGridData[p.X, p.Y].Creatures[k].Scavenger) || (!_gameData.Creatures[i].IsOmnivore && _gameData.MapGridData[p.X, p.Y].Creatures[k].IsCarnivore && (_gameData.Creatures[i].Carnivore - _gameData.Settings.CarnivoreLevelBuffer) > _gameData.MapGridData[p.X, p.Y].Creatures[k].Carnivore)))
                                    {
                                        if (_gameData.Creatures[i].Bounds.Intersects(_gameData.MapGridData[p.X, p.Y].Creatures[k].Bounds))
                                        {
                                            Creature tmpCreature = _gameData.MapGridData[p.X, p.Y].Creatures[k];

                                            //Slower food digestion means you pull more energy from the food
                                            float energyAddition = _gameData.Settings.EnergyGivenFromFood + (_gameData.Creatures[i].FoodDigestion / 10);

                                            //Bonus food awarded for Carnivores. They are more specialized to digesting meat compared to Omnivores
                                            if (!_gameData.Creatures[i].IsOmnivore)
                                            {
                                                _gameData.Creatures[i].UndigestedFood += 2;
                                                energyAddition = energyAddition * 3;
                                            }
                                            //Get one food for eating the creature
                                            _gameData.Creatures[i].UndigestedFood++;
                                            _gameData.Creatures[i].TotalFoodEaten++;
                                            _gameData.Creatures[i].Energy += energyAddition;

                                            //Carnivores over performing so remove this advantage
                                            ////Get all of the undigested food from the creature along with the Energy that goes along with that
                                            //_gameData.Creatures[i].UndigestedFood += tmpCreature.UndigestedFood;
                                            //_gameData.Creatures[i].TotalFoodEaten += tmpCreature.UndigestedFood;
                                            //_gameData.Creatures[i].Energy += FOOD_ENERGY_AMOUNT * tmpCreature.UndigestedFood;

                                            tmpCreature.IsAlive = false;
                                            tmpCreature.DeathCause = "Eaten";

                                            if (_gameData.Focus == tmpCreature)
                                            {
                                                _gameData.Focus = null;
                                                _gameData.FocusIndex = -1;
                                            }

                                            deadCreaturesToRemove.Add(tmpCreature);
                                            //_gameData.RemoveCreatureFromGrid(tmpCreature, _gameData.MapGridData[p.X, p.Y].Creatures[k].GridPositions);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    UpdateMoveCreature(gameTime, i);
                }
            }

            foreach (Creature c in deadCreaturesToRemove)
            {
                _gameData.AddDeadCreatureToList(c);
                //_gameData.Creatures.Remove(c);
            }
        }
        private void UpdateOffTickSpawnFood(GameTime gameTime)
        {
            //Spawn new food
            if (_elapsedTimeSinceFoodGeneration > _foodGenerationIntervalSeconds)
            {
                _elapsedTimeSinceFoodGeneration -= _foodGenerationIntervalSeconds;
                SpawnFood();
            }
        }
        private void UpdateOffTickEventCheckCleanup(GameTime gameTim)
        {
            //_gameData.PruneEventLog(EVENT_LOG_DISPLAY_COUNT);
        }
        private void UpdateHandleInputs(GameTime gameTime)
        {
            if (_inputState.IsExitGame(PlayerIndex.One))
            {
                Exit();
            }
            else
            {
                _inputState.Update();
                _player.HandleInput(_inputState, PlayerIndex.One, ref _gameData, _settingsTabPanel);
                Global.Camera.HandleInput(_inputState, PlayerIndex.One, ref _gameData);

                //if (_gameData.ShowChart)
                //{
                //    if (_gameData.ChartDataTop.Count > 0)
                //    {
                //        if (_gameData.ShowSettingsPanel)
                //        {
                //            _chart.Visible = false;
                //        }
                //        else
                //        {
                //            _chart.Visible = true;
                //        }
                //    }
                //}
            }
        }
        private void UpdateHandleEndOfSimulation(GameTime gameTime)
        {
            #pragma warning disable CS0162 // Unreachable code detected
            if (ENABLE_DATABASE_STATS && _writeStats)
            {
                SqlConnection connection = _connectionManager.GetSqlConnection(_gameData.Settings);

                //Insert all of the Live creatures first
                for (int i = 0; i < _gameData.Creatures.Count; i++)
                {
                    List<string> sqlStatements = _gameData.Creatures[i].GetCreatureStatisticsSQL(_gameRandSeed, _sessionID, _gameData.Settings.StartingCreatureRatio, _gameData.Settings.StartingFoodRatio, (gameTime.TotalGameTime - _resetTimeSpan).TotalMinutes);
                    foreach (string s in sqlStatements)
                    {
                        SqlCommand cmd = _connectionManager.GetSqlCommand(connection, s);
                        cmd.ExecuteNonQuery();
                    }
                }

                //Insert all the dead creatures
                for (int i = 0; i < _gameData.DeadCreatures.Count; i++)
                {
                    List<string> sqlStatements = _gameData.DeadCreatures[i].GetCreatureStatisticsSQL(_gameRandSeed, _sessionID, _gameData.Settings.StartingCreatureRatio, _gameData.Settings.StartingFoodRatio, (gameTime.TotalGameTime - _resetTimeSpan).TotalMinutes);
                    foreach (string s in sqlStatements)
                    {
                        SqlCommand cmd = _connectionManager.GetSqlCommand(connection, s);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else if (ENABLE_CSV_STATS && _writeStats)
            {
                //Build Directory if it does not exist
                if(!System.IO.Directory.Exists(_sessionID.ToString()))
                    System.IO.Directory.CreateDirectory(_sessionID.ToString());

                //Write Settings
                List<string> csvSettings = BuildStringListFromClass(_gameData.Settings, _gameData.CreatureSettings, _gameData.MutationSettings);
                System.IO.File.WriteAllLines(System.IO.Path.Combine(_sessionID.ToString(), "Settings.csv"), csvSettings.ToArray());
                csvSettings = null; //Let GC cleanup the RAM before we build the next list

                //Alive creatures
                List<string> csvListAliveOnly = BuildStringList(_gameData.Creatures);
                System.IO.File.WriteAllLines(System.IO.Path.Combine(_sessionID.ToString(),"ActiveOnly.csv"), csvListAliveOnly.ToArray());
                csvListAliveOnly = null; //Let GC cleanup the RAM before we build the next list

                //Both Dead and Alive creatures
                List<string> csvListBothDeadAndAlive = BuildStringList(_gameData.Creatures, _gameData.DeadCreatures);
                System.IO.File.WriteAllLines(System.IO.Path.Combine(_sessionID.ToString(), "InactiveAndActive.csv"), csvListBothDeadAndAlive.ToArray());
                csvListBothDeadAndAlive = null; //Let GC cleanup the RAM before we build the next list

                //Write Magic Number file
                List<string> csvMagicNumberFile = BuildMagicNumberList();
                System.IO.File.WriteAllLines(System.IO.Path.Combine(_sessionID.ToString(), "MagicNumberFile.csv"), csvMagicNumberFile.ToArray());
                csvMagicNumberFile = null; //Let GC cleanup the RAM before we build the next list

                //Write Fitness file
                List<string> csvFitnessFile = BuildFitnessFile();
                System.IO.File.WriteAllLines(System.IO.Path.Combine(_sessionID.ToString(), "FitnessFile.csv"), csvFitnessFile.ToArray());
                csvFitnessFile = null; //Let GC cleanup the RAM before we build the next list

                //Write Debug file
                List<string> csvDebugTimerFile = BuildDebugTimerList();
                System.IO.File.WriteAllLines(System.IO.Path.Combine(_sessionID.ToString(), "DebugTimerFile.csv"), csvDebugTimerFile.ToArray());
                csvDebugTimerFile = null; //Let GC cleanup the RAM before we build the next list
            }
            else
            {
                //Reset the timespan so that we continue to run. Disable the warning for now, we will probably move a bunch of settings to the config later
                _resetTimeSpan = gameTime.TotalGameTime;
            }

            if (ENABLE_GAME_RESETS)
            {
                ResetGame(gameTime);
            }
            else
            {
                _writeStats = false;
            }
            #pragma warning restore CS0162 // Unreachable code detected
        }
        private void UpdateHandleObjectsToBeDrawn(GameTime gameTime)
        {
            if (Global.Camera.CameraChange && _gameData.Focus == null)
            {
                Global.Camera.CameraChange = false;

                for (int i = 0; i < _gameData.Food.Count(); i++)
                {
                    if (!_gameData.Food[i].MarkForDelete && Global.Camera.VisibleArea.Contains(_gameData.Food[i].Position))
                    {
                        _gameData.Food[i].DrawObject = true;
                    }
                    else
                    {
                        _gameData.Food[i].DrawObject = false;
                    }
                }

                for (int i = 0; i < _gameData.Plants.Count(); i++)
                {
                    if (!_gameData.Plants[i].MarkForDelete && Global.Camera.VisibleArea.Contains(_gameData.Plants[i].Position))
                    {
                        _gameData.Plants[i].DrawObject = true;
                    }
                    else
                    {
                        _gameData.Plants[i].DrawObject = false;
                    }
                }

                for (int i = 0; i < _gameData.Creatures.Count(); i++)
                {
                    if (_gameData.Creatures[i].IsAlive && Global.Camera.VisibleArea.Contains(_gameData.Creatures[i].Position))
                    {
                        _gameData.Creatures[i].DrawObject = true;
                    }
                    else
                    {
                        _gameData.Creatures[i].DrawObject = false;
                    }
                }
            }
        }

        //Update Creature functions
        private void UpdateMoveCreature(GameTime gameTime, int creatureIndex)
        {
            if (_gameData.Creatures[creatureIndex].IsAlive)
            {
                //Move the creature
                //_gameData.Creatures[creatureIndex].Position += _gameData.Creatures[creatureIndex].Direction * (_gameData.Creatures[creatureIndex].Speed / 10f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _gameData.Creatures[creatureIndex].Position += _gameData.Creatures[creatureIndex].Direction * (_gameData.Creatures[creatureIndex].Speed) * CREATURE_SPEED_BOOST * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _gameData.Creatures[creatureIndex].Energy -= _gameData.Creatures[creatureIndex].CalculateCreatureEnergyDepletion(_gameData);
                _gameData.Creatures[creatureIndex].GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);

                if (_gameData.Creatures[creatureIndex].CurrentGridPositionsForCompare != _gameData.Creatures[creatureIndex].OldGridPositionsForCompare)
                {
                    //Remove delta
                    List<Point> delta = _gameData.Creatures[creatureIndex].GetGridDelta();
                    if (delta.Count > 0)
                    {
                        _gameData.RemoveCreatureFromGrid(_gameData.Creatures[creatureIndex], delta);
                    }

                    //Add delta
                    delta = _gameData.Creatures[creatureIndex].GetGridDeltaAdd();
                    if (delta.Count > 0)
                    {
                        _gameData.AddCreatureDeltaToGrid(_gameData.Creatures[creatureIndex], delta);
                    }
                }
            }
        }

        //Draw World Functions
        private void DrawWorldObjects()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);

            try
            {
                DateTime debugDttm = DateTime.Now;
                DrawClimates();
                _debugTimer.TimeDrawClimates += (DateTime.Now - debugDttm).TotalMilliseconds;
                debugDttm = DateTime.Now;
                DrawFood();
                _debugTimer.TimeDrawFood += (DateTime.Now - debugDttm).TotalMilliseconds;
                debugDttm = DateTime.Now;
                DrawEggs();
                _debugTimer.TimeDrawEggs += (DateTime.Now - debugDttm).TotalMilliseconds;
                debugDttm = DateTime.Now;
                DrawCreatures();
                _debugTimer.TimeDrawCreatures += (DateTime.Now - debugDttm).TotalMilliseconds;
                debugDttm = DateTime.Now;
                DrawBorders();
                _debugTimer.TimeDrawBorders += (DateTime.Now - debugDttm).TotalMilliseconds;
                debugDttm = DateTime.Now;
                DrawPlants();
                _debugTimer.TimeDrawPlants += (DateTime.Now - debugDttm).TotalMilliseconds;
                debugDttm = DateTime.Now;
                DrawHighlightCreatures();
                _debugTimer.TimeDrawHighlightCreatures += (DateTime.Now - debugDttm).TotalMilliseconds;
                debugDttm = DateTime.Now;
                DrawDebugData();
                _debugTimer.TimeDrawDebugData += (DateTime.Now - debugDttm).TotalMilliseconds;

                //DateTime debugDttm = DateTime.Now;
                //_spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
                //DrawClimates();
                //_spriteBatch.End();
                //_debugTimer.TimeDrawClimates += (DateTime.Now - debugDttm).TotalMilliseconds;
                //debugDttm = DateTime.Now;
                //_spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
                //DrawFood();
                //_spriteBatch.End();
                //_debugTimer.TimeDrawFood += (DateTime.Now - debugDttm).TotalMilliseconds;
                //debugDttm = DateTime.Now;
                //_spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
                //DrawEggs();
                //_spriteBatch.End();
                //_debugTimer.TimeDrawEggs += (DateTime.Now - debugDttm).TotalMilliseconds;
                //debugDttm = DateTime.Now;
                //_spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
                //DrawCreatures();
                //_spriteBatch.End();
                //_debugTimer.TimeDrawCreatures += (DateTime.Now - debugDttm).TotalMilliseconds;
                //debugDttm = DateTime.Now;
                //_spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
                //DrawBorders();
                //_spriteBatch.End();
                //_debugTimer.TimeDrawBorders += (DateTime.Now - debugDttm).TotalMilliseconds;
                //debugDttm = DateTime.Now;
                //_spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
                //DrawPlants();
                //_spriteBatch.End();
                //_debugTimer.TimeDrawPlants += (DateTime.Now - debugDttm).TotalMilliseconds;
                //debugDttm = DateTime.Now;
                //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
                //DrawHighlightCreatures();
                //_debugTimer.TimeDrawHighlightCreatures += (DateTime.Now - debugDttm).TotalMilliseconds;
                //debugDttm = DateTime.Now;
                //DrawDebugData();
                //_spriteBatch.End();
                //_debugTimer.TimeDrawDebugData += (DateTime.Now - debugDttm).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(_sessionID.ToString(), "ErrorLogDrawWorld.txt"), DateTime.Now.ToString() + " - DRAW Uncaught error: " + ex.Message + Environment.NewLine + "Stacktrace: " + ex.StackTrace);
            }

            _spriteBatch.End();
        }
        private void DrawClimates()
        {
            //_spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.LeftWall.X - BORDER_WIDTH, (int)_borders.LeftWall.Y, BORDER_WIDTH, Global.WORLD_SIZE + BORDER_WIDTH), Color.SaddleBrown);
            _spriteBatch.Draw(_whitePixel, new Rectangle(0, 0, _gameData.Settings.WorldSize, _climateHeight), Color.LightSkyBlue);
            _spriteBatch.Draw(_whitePixel, new Rectangle(0, _gameData.Settings.WorldSize - _climateHeight, _gameData.Settings.WorldSize, _climateHeight), Color.DarkOrange);
        }
        private void DrawCreatures()
        {
            for (int i = 0; i < _gameData.Creatures.Count; i++)
            {
                if (_gameData.Creatures[i].DrawObject || (_gameData.Focus != null && _gameData.Creatures[i].IsAlive))
                {
                    _spriteBatch.Draw(_gameData.Creatures[i].Texture, _gameData.Creatures[i].Position, null, _gameData.Creatures[i].CreatureColor, _gameData.Creatures[i].Rotation, _gameData.Creatures[i].Origin, 1f, SpriteEffects.None, 1f);
                }
            }
        }
        private void DrawEggs()
        {
            for (int i = 0; i < _gameData.Eggs.Count; i++)
            {
                _spriteBatch.Draw(_gameData.Eggs[i].Texture, _gameData.Eggs[i].Position, null, Color.White, 0f, _gameData.Eggs[i].Origin, 1f, SpriteEffects.None, 1f);
            }
        }
        private void DrawFood()
        {
            for (int i = 0; i < _gameData.Food.Count; i++)
            {
                if (_gameData.Food[i].DrawObject || (_gameData.Focus != null && !_gameData.Food[i].MarkForDelete))
                {
                    _spriteBatch.Draw(_gameData.Food[i].Texture, _gameData.Food[i].Position, null, _gameData.Food[i].FoodColor, 0f, _gameData.Food[i].Origin, 1f, SpriteEffects.None, 1f);
                    if (_gameData.ShowFoodStrength)
                    {
                        _spriteBatch.DrawString(_foodFont, _gameData.Food[i].DisplayText, new Vector2(_gameData.Food[i].Position.X, _gameData.Food[i].Position.Y + (_gameData.Food[i].Texture.Height / 2) + (_gameData.Food[i].TextSize.Y / 2)), Color.Black, 0f, new Vector2(_gameData.Food[i].TextSize.X / 2, _gameData.Food[i].TextSize.Y / 2), 1f, SpriteEffects.None, 1f);
                    }
                }
            }
        }
        private void DrawPlants()
        {
            for (int i = 0; i < _gameData.Plants.Count; i++)
            {
                if (_gameData.Plants[i].DrawObject || (_gameData.Focus != null && !_gameData.Plants[i].MarkForDelete))
                {
                    _spriteBatch.Draw(_gameData.Plants[i].Texture, _gameData.Plants[i].Position, null, Color.White, _gameData.Plants[i].Rotation, _gameData.Plants[i].Origin, 1f, SpriteEffects.None, 1f);
                    if (_gameData.ShowDebugData)
                    {
                        _spriteBatch.DrawString(_foodFont, _gameData.Plants[i].DisplayText, new Vector2(_gameData.Plants[i].Position.X, _gameData.Plants[i].Position.Y + (_gameData.Plants[i].Texture.Height / 2) + (_gameData.Plants[i].TextSize.Y / 2)), Color.Black, 0f, new Vector2(_gameData.Plants[i].TextSize.X / 2, _gameData.Plants[i].TextSize.Y / 2), 1f, SpriteEffects.None, 1f);
                    }
                }
            }
        }
        private void DrawBorders()
        {
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.LeftWall.X - BORDER_WIDTH, (int)_borders.LeftWall.Y, BORDER_WIDTH, _gameData.Settings.WorldSize + BORDER_WIDTH), Color.SaddleBrown);
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.RightWall.X, (int)_borders.RightWall.Y - BORDER_WIDTH, BORDER_WIDTH, _gameData.Settings.WorldSize + BORDER_WIDTH), Color.SaddleBrown);
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.TopWall.X - BORDER_WIDTH, (int)_borders.TopWall.Y - BORDER_WIDTH, _gameData.Settings.WorldSize + BORDER_WIDTH, BORDER_WIDTH), Color.SaddleBrown);
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.BottomWall.X - BORDER_WIDTH, (int)_borders.BottomWall.Y, _gameData.Settings.WorldSize + (BORDER_WIDTH * 2), BORDER_WIDTH), Color.SaddleBrown);
        }
        private void DrawHighlightCreatures()
        {
            if (_gameData.HighlightSpecies && _gameData.Focus != null)
            {
                int followedSpecies = _gameData.Focus.SpeciesId;
                int borderWidth = 3;

                for (int i = 0; i < _gameData.Creatures.Count; i++)
                {
                    if (_gameData.Creatures[i].SpeciesId == followedSpecies)
                    {
                        int diagnolLength = (int)Math.Ceiling(Math.Sqrt((_gameData.Creatures[i].Texture.Width * _gameData.Creatures[i].Texture.Width) + (_gameData.Creatures[i].Texture.Height * _gameData.Creatures[i].Texture.Height)));
                        float upperLeftX = _gameData.Creatures[i].Position.X - (diagnolLength / 2), upperLeftY = _gameData.Creatures[i].Position.Y - (diagnolLength / 2);

                        _spriteBatch.Draw(_whitePixel, new Rectangle((int)upperLeftX - borderWidth, (int)upperLeftY - borderWidth, diagnolLength + borderWidth * 2, borderWidth), Color.Red);
                        _spriteBatch.Draw(_whitePixel, new Rectangle((int)upperLeftX - borderWidth, (int)upperLeftY + diagnolLength, diagnolLength + borderWidth * 2, borderWidth), Color.Red);
                        _spriteBatch.Draw(_whitePixel, new Rectangle((int)upperLeftX + diagnolLength, (int)upperLeftY - borderWidth, borderWidth, diagnolLength + borderWidth * 2), Color.Red);
                        _spriteBatch.Draw(_whitePixel, new Rectangle((int)upperLeftX - borderWidth, (int)upperLeftY - borderWidth, borderWidth, diagnolLength + borderWidth * 2), Color.Red);
                    }
                }
            }
        }
        private void DrawDebugData()
        {
            if (ENABLE_DEBUG_DATA || _gameData.ShowDebugData)
            {
                //Draw Map Grid
                for (int i = 0; i < _gameData.MapGridData.GetLength(0); i++)
                {
                    _spriteBatch.Draw(_whitePixel, new Rectangle(i * GRID_CELL_SIZE, 0, 1, _gameData.Settings.WorldSize), Color.Red);
                }
                for (int i = 0; i < _gameData.MapGridData.GetLength(1); i++)
                {
                    _spriteBatch.Draw(_whitePixel, new Rectangle(0, i * GRID_CELL_SIZE, _gameData.Settings.WorldSize, 1), Color.Red);
                }
                
                //Draw Creature Bounding boxes
                if(_gameData.FocusIndex >= 0)
                    DrawDebugDataForCreature(_gameData.Focus, false);

                if (ENABLE_DEBUG_DATA)
                {
                    //This is for Debugging interactions
                    int index = 0;
                    DrawDebugDataForCreature(_gameData.Creatures[index], false);

                    index = 1;
                    DrawDebugDataForCreature(_gameData.Creatures[index], true);
                }
            }
        }
        private void DrawDebugDataForCreature(Creature creature, bool left)
        {
            if (creature != null)
            {
                _spriteBatch.Draw(_whitePixel, new Rectangle((int)(creature.Position.X - creature.Origin.X), (int)(creature.Position.Y - creature.Origin.Y) - 1, creature.Texture.Width, 1), Color.Black);
                _spriteBatch.Draw(_whitePixel, new Rectangle((int)(creature.Position.X - creature.Origin.X), (int)(creature.Position.Y + creature.Origin.Y), creature.Texture.Width, 1), Color.Black);
                _spriteBatch.Draw(_whitePixel, new Rectangle((int)(creature.Position.X - creature.Origin.X) - 1, (int)(creature.Position.Y - creature.Origin.Y), 1, creature.Texture.Height), Color.Black);
                _spriteBatch.Draw(_whitePixel, new Rectangle((int)(creature.Position.X + creature.Origin.X), (int)(creature.Position.Y - creature.Origin.Y), 1, creature.Texture.Height), Color.Black);

                if (creature.Sight > 0)
                {
                    //Top line
                    _spriteBatch.Draw(_whitePixel, new Rectangle((int)(creature.Position.X - creature.TextureCollideDistance - (int)Math.Round(creature.Sight, 0)), (int)(creature.Position.Y - creature.TextureCollideDistance - (int)Math.Round(creature.Sight, 0)) - 1, (creature.TextureCollideDistance + (int)Math.Round(creature.Sight, 0)) * 2, 1), Color.Gray);
                    //Bottom line
                    _spriteBatch.Draw(_whitePixel, new Rectangle((int)(creature.Position.X - creature.TextureCollideDistance - (int)Math.Round(creature.Sight, 0)), (int)(creature.Position.Y + creature.TextureCollideDistance + (int)Math.Round(creature.Sight, 0)), (creature.TextureCollideDistance + (int)Math.Round(creature.Sight, 0)) * 2, 1), Color.Gray);
                    //Left line
                    _spriteBatch.Draw(_whitePixel, new Rectangle((int)(creature.Position.X - creature.TextureCollideDistance - (int)Math.Round(creature.Sight, 0)) - 1, (int)(creature.Position.Y - creature.TextureCollideDistance - (int)Math.Round(creature.Sight, 0)), 1, (creature.TextureCollideDistance + (int)Math.Round(creature.Sight, 0)) * 2), Color.Gray);
                    //Right line
                    _spriteBatch.Draw(_whitePixel, new Rectangle((int)(creature.Position.X + creature.TextureCollideDistance + (int)Math.Round(creature.Sight, 0)), (int)(creature.Position.Y - creature.TextureCollideDistance - (int)Math.Round(creature.Sight, 0)), 1, (creature.TextureCollideDistance + (int)Math.Round(creature.Sight, 0) * 2)), Color.Gray);
                }

                if (creature.CalculatedIntercept != Vector2.Zero)
                {
                    _spriteBatch.Draw(_whitePixel, new Rectangle((int)Math.Round(creature.CalculatedIntercept.X, 0) - 2, (int)Math.Round(creature.CalculatedIntercept.Y, 0) - 2, 4, 4), Color.Red);
                }

                List<string> debugInfo = new List<string>();
                debugInfo.Add("Position: " + (int)creature.Position.X + "," + (int)creature.Position.Y);
                debugInfo.Add("Direction: " + (int)creature.Direction.X + "," + (int)creature.Direction.Y);
                debugInfo.Add("Rotation R: " + Math.Round(creature.Rotation, 2));
                debugInfo.Add("Rotation D: " + Math.Round(MathHelper.ToDegrees(creature.Rotation), 2));
                if (creature.CalculatedIntercept != Vector2.Zero)
                {
                    debugInfo.Add("Intercept: " + (int)creature.CalculatedIntercept.X + "," + (int)creature.CalculatedIntercept.Y);
                }

                for (int x = 0; x < _gameData.MapGridData.GetLength(0); x++)
                {
                    for (int y = 0; y < _gameData.MapGridData.GetLength(1); y++)
                    {
                        if (_gameData.MapGridData[x, y].Creatures.Contains(creature))
                        {
                            debugInfo.Add("MapCell: " + x + "," + y);
                        }
                    }
                }

                int lockWidth = 125;
                if (left)
                    DrawDebugPanel(_diagFont, debugInfo, lockWidth, new Vector2(creature.Position.X - (creature.Texture.Width / 2) - 5 - lockWidth, creature.Position.Y - (creature.Texture.Height / 2)));
                else
                    DrawDebugPanel(_diagFont, debugInfo, lockWidth, new Vector2(creature.Position.X + (creature.Texture.Width / 2) + 5, creature.Position.Y - (creature.Texture.Height / 2)));
            }
        }
        private void DrawDebugPanel(SpriteFont textFont, List<string> text, int lockedWidth, Vector2 position)
        {
            int width = lockedWidth;
            int height = 0;
            int textHeight = 0;
            int textSpacing = 5;
            int borderDepth = 2;
            int startingX = (int)Math.Ceiling(position.X);
            int startingY = (int)Math.Ceiling(position.Y);
            int currentX, currentY;

            if (lockedWidth == 0) //Calculate the Width if lock width not specified
            {
                int maxWidth = 0;

                for (int i = 0; i < text.Count; i++)
                {
                    Vector2 size = textFont.MeasureString(text[i]);
                    textHeight = (int)Math.Ceiling(size.Y);
                    int tmpWidth = (int)Math.Ceiling(size.X);
                    if (tmpWidth > maxWidth)
                        maxWidth = tmpWidth;
                }

                width = maxWidth + (textSpacing * 2) + (borderDepth * 2);
            }
            else
            {
                Vector2 size = textFont.MeasureString("AGHIQZXVY[]qyp");
                textHeight = (int)Math.Ceiling(size.Y);
            }

            height = text.Count * (textHeight + textSpacing) + textSpacing;

            //Draw the Background border
            _spriteBatch.Draw(_whitePixel, new Rectangle(startingX, startingY, width, height), Color.Black);
            _spriteBatch.Draw(_whitePixel, new Rectangle(startingX + borderDepth, startingY + borderDepth, width - borderDepth * 2, height - borderDepth * 2), Color.White);

            currentX = startingX + borderDepth + textSpacing;
            currentY = startingY + borderDepth + textSpacing;

            for (int i = 0; i < text.Count; i++)
            {
                _spriteBatch.DrawString(textFont, text[i], new Vector2(currentX, currentY), Color.Black);
                currentY += textHeight + textSpacing;
            }
        }

        //Draw HUD Functions
        private void DrawHUD()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            DrawMarkers();
            DrawCreatureStatsOrEventLogPanel();
            DrawMapStatistics();
            DrawControlsPanel();
            DrawChartBorder();
            DrawSettingsPanel();
            DrawFPS();
            DrawDebugDataHUD();

            _spriteBatch.End();
        }
        private void DrawCreatureStatsOrEventLogPanel()
        {
            if (_gameData.ShowCreatureStats && _gameData.Focus != null)
            {
                DrawPanelWithText(_panelHeaderFont, "Creature Statistics", _diagFont, _gameData.Focus.GetCreatureInformation(), Global.Anchor.LeftCenter, (int)Math.Ceiling(_diagFont.MeasureString("Position: {X:-100.000000, Y:-100.000000}").X), 0, 20);
            }
            //else if(_gameData.ShowEventLogPanel && _gameData.EventLog.Count > 0)
            //{
            //    DrawPanelWithText(_panelHeaderFont, "Event Log", _diagFont, _gameData.GetEventsForDisplay(EVENT_LOG_DISPLAY_COUNT), Global.Anchor.TopLeft, (int)Math.Ceiling(_diagFont.MeasureString("Species 'Ferniandos' has mutated into 'Frainsiscos'").X), 0, 20);
            //}
        }
        private void DrawMapStatistics()
        {
            int topBuffer = 10;
            int topBufferNumers = 13;
            int spacing = 5;
            //int startingPos = (int)(_graphics.PreferredBackBufferWidth * 0.35);
            int startingPos = (int)(_graphics.PreferredBackBufferWidth * 0.25);
            int currentPos = startingPos;
            int menuSpacing = 63;

            //Population/Alive creatures
            _spriteBatch.Draw(_populationTexture, new Vector2(currentPos, topBuffer), null, Color.White, 0f, Vector2.Zero, HUD_ICON_SCALE, SpriteEffects.None, 1);
            currentPos += (int)Math.Ceiling(_populationTexture.Width * HUD_ICON_SCALE) + spacing;
            _spriteBatch.DrawString(_panelHeaderFont, _gameData.MapStatistics.AliveCreatures.ToString("#,##0") + "(" + _gameData.MapStatistics.UniqueSpecies.ToString("#,##0") + ")", new Vector2(currentPos, topBufferNumers), Color.Black);
            currentPos += menuSpacing + (int)(menuSpacing * .5);

            ////Dead creatures
            //_spriteBatch.Draw(_deadCreaturesTexture, new Vector2(currentPos, topBuffer), null, Color.White, 0f, Vector2.Zero, HUD_ICON_SCALE, SpriteEffects.None, 1);
            //currentPos += (int)Math.Ceiling(_deadCreaturesTexture.Width * HUD_ICON_SCALE) + spacing;
            //_spriteBatch.DrawString(_panelHeaderFont, _gameData.MapStatistics.DeadCreatures.ToString("#,##0"), new Vector2(currentPos, topBufferNumers), Color.Black);
            //currentPos += menuSpacing;

            //Eggs on map
            _spriteBatch.Draw(_eggsOnMapTexture, new Vector2(currentPos, topBuffer), null, Color.White, 0f, Vector2.Zero, HUD_ICON_SCALE, SpriteEffects.None, 1);
            currentPos += (int)Math.Ceiling(_eggsOnMapTexture.Width * HUD_ICON_SCALE) + spacing;
            _spriteBatch.DrawString(_panelHeaderFont, _gameData.MapStatistics.EggsOnMap.ToString("#,##0"), new Vector2(currentPos, topBufferNumers), Color.Black);
            currentPos += menuSpacing;

            //Food on map
            _spriteBatch.Draw(_foodOnMapTexture, new Vector2(currentPos, topBuffer), null, Color.White, 0f, Vector2.Zero, HUD_ICON_SCALE, SpriteEffects.None, 1);
            currentPos += (int)Math.Ceiling(_foodOnMapTexture.Width * HUD_ICON_SCALE) + spacing;
            _spriteBatch.DrawString(_panelHeaderFont, _gameData.MapStatistics.FoodOnMap.ToString("#,##0"), new Vector2(currentPos, topBufferNumers), Color.Black);
            currentPos += menuSpacing;

            //Plants on map
            _spriteBatch.Draw(_plantsOnMapTexture, new Vector2(currentPos, topBuffer), null, Color.White, 0f, Vector2.Zero, HUD_ICON_SCALE, SpriteEffects.None, 1);
            currentPos += (int)Math.Ceiling(_eggsOnMapTexture.Width * HUD_ICON_SCALE) + spacing;
            _spriteBatch.DrawString(_panelHeaderFont, _gameData.MapStatistics.PlantsOnMap.ToString("#,##0"), new Vector2(currentPos, topBufferNumers), Color.Black);
            currentPos += menuSpacing;

            //Percent Herbavore
            _spriteBatch.Draw(_herbavoreSightTexture, new Vector2(currentPos, topBuffer), null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 1);
            currentPos += (int)Math.Ceiling(_herbavoreTexture.Width * 2f) + spacing;
            _spriteBatch.DrawString(_panelHeaderFont, _gameData.MapStatistics.PercentHerbavore.ToString("#0%"), new Vector2(currentPos, topBufferNumers), Color.Black);
            currentPos += menuSpacing;

            //Percent Carnivore
            _spriteBatch.Draw(_carnivoreSightTexture, new Vector2(currentPos, topBuffer), null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 1);
            currentPos += (int)Math.Ceiling(_carnivoreTexture.Width * 2f) + spacing;
            _spriteBatch.DrawString(_panelHeaderFont, _gameData.MapStatistics.PercentCarnivore.ToString("#0%"), new Vector2(currentPos, topBufferNumers), Color.Black);
            currentPos += menuSpacing;

            //Percent Scavenger
            _spriteBatch.Draw(_scavengerSightTexture, new Vector2(currentPos, topBuffer), null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 1);
            currentPos += (int)Math.Ceiling(_scavengerTexture.Width * 2f) + spacing;
            _spriteBatch.DrawString(_panelHeaderFont, _gameData.MapStatistics.PercentScavenger.ToString("#0%"), new Vector2(currentPos, topBufferNumers), Color.Black);
            currentPos += menuSpacing;

            //Percent Omnivore
            _spriteBatch.Draw(_omnivoreSightTexture, new Vector2(currentPos, topBuffer), null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 1);
            currentPos += (int)Math.Ceiling(_omnivoreTexture.Width * 2f) + spacing;
            _spriteBatch.DrawString(_panelHeaderFont, _gameData.MapStatistics.PercentOmnivore.ToString("#0%"), new Vector2(currentPos, topBufferNumers), Color.Black);
            currentPos += menuSpacing;
        }
        private void DrawControlsPanel()
        {
            if (_gameData.ShowControls)
            {
                DrawPanelWithText(_panelHeaderFont, "Controls", _diagFont, _controlsListText, Global.Anchor.TopRight, 0, 0, 20);
            }
            else
            {
                DrawPanelWithText(_panelHeaderFont, String.Empty, _diagFont, new List<string> { "[F12] Show Controls" }, Global.Anchor.TopRight, 0, 0, 20);
            }
        }
        private void DrawChartBorder()
        {
            if (_chartPosSet && !_gameData.ShowSettingsPanel && _gameData.ShowChart && _gameData.ChartDataTop.Count > 0 && _gameData.ChartTexture != null && _gameData.ChartPosition != null)
            {
                int borderDepth = 2;

                _spriteBatch.Draw(_whitePixel, new Rectangle(_gameData.ChartPosition.X - borderDepth, _gameData.ChartPosition.Y - borderDepth, _gameData.ChartTexture.Width + (borderDepth * 2), _gameData.ChartTexture.Height + (borderDepth * 2)), Color.Black);
                _spriteBatch.Draw(_gameData.ChartTexture, new Vector2(_gameData.ChartPosition.X, _gameData.ChartPosition.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
            }
        }
        private void DrawSettingsPanel()
        {
            if (_gameData.ShowSettingsPanel)
            {
                int borderWidth = 2;
                int tabSpacing = 5;
                int sliderSpacing = 5;
                int tabTextHeight = 0;
                int workingXPos = 0;
                int workingYPos = 0;

                //Draw the panel frame first
                _spriteBatch.Draw(_whitePixel, new Rectangle(_settingsTabPanel.Position.X, _settingsTabPanel.Position.Y, _settingsTabPanel.PanelWidth, _settingsTabPanel.PanelHeight), Color.Black);
                _spriteBatch.Draw(_whitePixel, new Rectangle(_settingsTabPanel.Position.X + borderWidth, _settingsTabPanel.Position.Y + borderWidth, _settingsTabPanel.PanelWidth - (borderWidth * 2), _settingsTabPanel.PanelHeight - (borderWidth * 2)), Color.White);
     
                workingXPos = _settingsTabPanel.Position.X + borderWidth;
                workingYPos = _settingsTabPanel.Position.Y + borderWidth;

                //Draw the tabs
                for (int i = 0; i < _settingsTabPanel.Tabs.Count; i++)
                {
                    Color tabBackgroundColor = new Color(54, 78, 111);
                    Color textColor = Color.White;
                    if (i == _settingsTabPanel.ActiveTab)
                    {
                        tabBackgroundColor = new Color(255,242,157);
                        textColor = Color.Black;
                    }

                    Vector2 tabSize = _panelHeaderFont.MeasureString(_settingsTabPanel.Tabs[i].TabText);
                    tabTextHeight = (int)Math.Ceiling(tabSize.Y);

                    //Draw the Tab background
                    int tabWidth = (int)Math.Ceiling(tabSize.X) + (tabSpacing * 2);
                    Rectangle rec = new Rectangle(workingXPos, workingYPos, (int)Math.Ceiling(tabSize.X) + (tabSpacing * 2), (int)Math.Ceiling(tabSize.Y) + (tabSpacing * 2));
                    _spriteBatch.Draw(_whitePixel, rec, tabBackgroundColor);
                    _settingsTabPanel.Tabs[i].ButtonRectangle = rec;
                    //Draw the text
                    _spriteBatch.DrawString(_panelHeaderFont, _settingsTabPanel.Tabs[i].TabText, new Vector2(workingXPos + tabSpacing, workingYPos + tabSpacing), textColor);
                    //Increment the working variable
                    workingXPos += tabWidth;

                    //Draw seperator between tabs
                    _spriteBatch.Draw(_whitePixel, new Rectangle(workingXPos, workingYPos, borderWidth, (int)Math.Ceiling(tabSize.Y) + (tabSpacing * 2)), Color.Black);

                    workingXPos += borderWidth;
                }

                //Draw the Save/Close Buttons
                float buttonScale = 0.4f;
                int yCenterSpacing = ((tabTextHeight + (tabSpacing * 2)) - (int)(_settingsTabPanel.CloseButton.Texture.Height * buttonScale)) / 2;
                Rectangle closeButtonRec = new Rectangle((_settingsTabPanel.Position.X + _settingsTabPanel.PanelWidth) - borderWidth - tabSpacing - (int)(_settingsTabPanel.CloseButton.Texture.Width * buttonScale), workingYPos + yCenterSpacing, (int)(_settingsTabPanel.CloseButton.Texture.Width * buttonScale), (int)(_settingsTabPanel.CloseButton.Texture.Height * buttonScale));
                _settingsTabPanel.CloseButton.ButtonRectangle = closeButtonRec;
                _spriteBatch.Draw(_settingsTabPanel.CloseButton.Texture, new Vector2(closeButtonRec.X, closeButtonRec.Y), null, Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 1f);

                Rectangle saveButtonRec = new Rectangle((_settingsTabPanel.Position.X + _settingsTabPanel.PanelWidth) - borderWidth - tabSpacing - (int)(_settingsTabPanel.SaveButton.Texture.Width * buttonScale) - (int)(_settingsTabPanel.CloseButton.Texture.Width * buttonScale) - (tabSpacing * 2), workingYPos + yCenterSpacing, (int)(_settingsTabPanel.SaveButton.Texture.Width * buttonScale), (int)(_settingsTabPanel.SaveButton.Texture.Height * buttonScale));
                _settingsTabPanel.SaveButton.ButtonRectangle = saveButtonRec;
                _spriteBatch.Draw(_settingsTabPanel.SaveButton.Texture, new Vector2(saveButtonRec.X, saveButtonRec.Y), null, Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 1f);

                Rectangle defaultButtonRec = new Rectangle((_settingsTabPanel.Position.X + _settingsTabPanel.PanelWidth) - borderWidth - tabSpacing - (int)(_settingsTabPanel.SaveButton.Texture.Width * buttonScale) - (int)(_settingsTabPanel.CloseButton.Texture.Width * buttonScale) - (int)(_settingsTabPanel.DefaultButton.Texture.Width * buttonScale) - (tabSpacing * 3), workingYPos + yCenterSpacing, (int)(_settingsTabPanel.DefaultButton.Texture.Width * buttonScale), (int)(_settingsTabPanel.DefaultButton.Texture.Height * buttonScale));
                _settingsTabPanel.DefaultButton.ButtonRectangle = defaultButtonRec;
                _spriteBatch.Draw(_settingsTabPanel.DefaultButton.Texture, new Vector2(defaultButtonRec.X, defaultButtonRec.Y), null, Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 1f);

                workingXPos = _settingsTabPanel.Position.X + borderWidth;
                workingYPos += tabTextHeight + (tabSpacing * 2);

                //Draw post tab line
                _spriteBatch.Draw(_whitePixel, new Rectangle(workingXPos, workingYPos, _settingsTabPanel.PanelWidth - borderWidth, borderWidth), Color.Black);

                workingXPos += sliderSpacing;
                workingYPos += (borderWidth * 2) + sliderSpacing;

                for (int i = 0; i < _settingsTabPanel.Tabs[_settingsTabPanel.ActiveTab].Controls.Sliders.Count; i++)
                {
                    _spriteBatch.DrawString(_panelHeaderFont, _settingsTabPanel.Tabs[_settingsTabPanel.ActiveTab].Controls.Sliders[i].SliderText, new Vector2(workingXPos, workingYPos), Color.Black);

                    _settingsTabPanel.Tabs[_settingsTabPanel.ActiveTab].Controls.Sliders[i].SliderPosition = new Vector2(workingXPos + 300, workingYPos);
                    _settingsTabPanel.Tabs[_settingsTabPanel.ActiveTab].Controls.Sliders[i].CurrentValue = _settingsTabPanel.Tabs[_settingsTabPanel.ActiveTab].Controls.Sliders[i].CurrentValue;
                    _settingsTabPanel.Tabs[_settingsTabPanel.ActiveTab].Controls.Sliders[i].Draw(_spriteBatch);

                    workingYPos += _settingsTabPanel.Tabs[_settingsTabPanel.ActiveTab].Controls.Sliders[i].MarkerHeight + sliderSpacing;
                }
            }
        }
        private void DrawFPS()
        {
            //_spriteBatch.DrawString(_diagFont, "FPS: " + _fps + "   " + (int)_totalElapsedSeconds, new Vector2(5, 5), Color.Black);
            _spriteBatch.DrawString(_diagFont, "FPS: " + _fps, new Vector2(2, 2), Color.Black);
            _spriteBatch.DrawString(_diagFont, "Runtime: " + _totalElapsedTime.ToString(), new Vector2(2, _graphics.PreferredBackBufferHeight - _diagTextHeight - 2), Color.Black);
        }
        private void DrawMarkers()
        {
            //Need to draw this with World coordinates but HUD Size
            float scale = 0.5f;
            if (_gameData.EggMarkers)
            {
                for (int i = 0; i < _gameData.Eggs.Count; i++)
                {
                    Texture2D texture;
                    if (_gameData.Eggs[i].Creature.IsOmnivore)
                    {
                        texture = _eggMarkerYellow;
                    }
                    else if (_gameData.Eggs[i].Creature.IsScavenger)
                    {
                        texture = _eggMarkerPurple;
                    }
                    else if (_gameData.Eggs[i].Creature.IsCarnivore)
                    {
                        texture = _eggMarkerRed;
                    }
                    else if (_gameData.Eggs[i].Creature.IsHerbavore)
                    {
                        texture = _eggMarkerGreen;
                    }
                    else
                    {
                        texture = _eggMarker;
                    }

                    Vector2 position = Vector2.Transform(new Vector2(_gameData.Eggs[i].Position.X, _gameData.Eggs[i].Position.Y - (_gameData.Eggs[i].Texture.Height / 2)), (Global.Camera.TranslationMatrix));

                    position.X = position.X - ((texture.Width * scale) / 2);
                    position.Y = position.Y - (texture.Height * scale);

                    _spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1);
                }
            }
            if (_gameData.HerbavoreMarkers || _gameData.CarnivoreMarkers || _gameData.ScavengerMarkers || _gameData.OmnivoreMarkers)
            {
                for (int i = 0; i < _gameData.Creatures.Count; i++)
                {
                    Color? markerColor = null;
                    if (_gameData.OmnivoreMarkers && _gameData.Creatures[i].IsOmnivore)
                    {
                        markerColor = Color.Yellow;
                    }
                    else if (_gameData.HerbavoreMarkers && _gameData.Creatures[i].IsHerbavore)
                    {
                        markerColor = Color.ForestGreen;
                    }
                    else if (_gameData.CarnivoreMarkers && _gameData.Creatures[i].IsCarnivore)
                    {
                        markerColor = Color.Red;
                    }
                    else if (_gameData.ScavengerMarkers && _gameData.Creatures[i].IsScavenger)
                    {
                        markerColor = Color.Purple;
                    }

                    if (markerColor != null)
                    {
                        Vector2 position = Vector2.Transform(new Vector2(_gameData.Creatures[i].Position.X, _gameData.Creatures[i].Position.Y - (_gameData.Creatures[i].Texture.Height / 2)), (Global.Camera.TranslationMatrix));

                        position.X = position.X - ((_blankMarker.Width * scale) / 2);
                        position.Y = position.Y - (_blankMarker.Height * scale);

                        _spriteBatch.Draw(_blankMarker, position, null, (Color)markerColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 1);
                    }
                }
            }
        }
        private void DrawDebugDataHUD()
        {

        }
        private void DrawPanelWithText(SpriteFont headerFont, string header, SpriteFont textFont, List<string> text, Global.Anchor anchor, int lockedWidthValue, int lockedHeightValue, int screenBuffer)
        {
            Color borderColor = Color.Black;
            Color headerBackgroundColor = Color.Purple;
            Color headerTextColor = Color.White;
            Color textColor = Color.DarkGreen;
            Color textBackgroundColor = Color.LightBlue;

            int startingX = 0, startingY = 0, width = 0, height = 0, headerHeight = 0, textHeight = 0, textWidth = 0, headerTextHeight = 0;
            int borderDepth = 2, textSpacing = 5;
            Vector2 headerSize;
            bool drawHeader = true;

            if (String.IsNullOrEmpty(header))
                drawHeader = false;

            if (drawHeader)
            {
                headerSize = headerFont.MeasureString(header);
                headerTextHeight = (int)Math.Ceiling(headerSize.Y);
                textWidth = (int)Math.Ceiling(headerSize.Y);
            }

            if (lockedWidthValue == 0)
            {
                foreach (string s in text)
                {
                    int tmpSize = (int)Math.Ceiling(textFont.MeasureString(s).X);
                    if (tmpSize > textWidth)
                    {
                        textWidth = tmpSize;
                    }
                }
            }
            else
            {
                textWidth = lockedWidthValue;
            }

            if (lockedHeightValue == 0)
            {
                textHeight = _diagTextHeight;
            }
            else
            {
                textHeight = lockedHeightValue;
            }

            if (drawHeader)
                headerHeight = (textSpacing * 4) + headerTextHeight;
            height = headerHeight + (borderDepth * 2) + (text.Count * (textHeight + textSpacing)) + textSpacing;
            width = (borderDepth * 2) + textWidth + (textSpacing * 2);

            switch (anchor)
            {
                case Global.Anchor.TopCenter:
                    startingX = (_graphics.PreferredBackBufferWidth / 2) - (width / 2);
                    startingY = screenBuffer;
                    break;
                case Global.Anchor.BottomCenter:
                    startingX = (_graphics.PreferredBackBufferWidth / 2) - (width / 2);
                    startingY = _graphics.PreferredBackBufferHeight - height - screenBuffer;
                    break;
                case Global.Anchor.RightCenter:
                    startingX = _graphics.PreferredBackBufferWidth - width - screenBuffer;
                    startingY = (_graphics.PreferredBackBufferHeight / 2) - (height / 2);
                    break;
                case Global.Anchor.LeftCenter:
                    startingX = screenBuffer;
                    startingY = (_graphics.PreferredBackBufferHeight / 2) - (height / 2);
                    break;
                case Global.Anchor.TopLeft:
                    startingX = screenBuffer;
                    startingY = screenBuffer;
                    break;
                case Global.Anchor.TopRight:
                    startingX = _graphics.PreferredBackBufferWidth - width - screenBuffer;
                    startingY = screenBuffer;
                    break;
                case Global.Anchor.BottomLeft:
                    startingX = screenBuffer;
                    startingY = _graphics.PreferredBackBufferHeight - height - screenBuffer;
                    break;
                case Global.Anchor.BottomRight:
                    startingX = _graphics.PreferredBackBufferWidth - width - screenBuffer;
                    startingY = _graphics.PreferredBackBufferHeight - height - screenBuffer;
                    break;
            }

            int currentX = startingX + borderDepth + textSpacing;
            int currentY = startingY + borderDepth + textSpacing;

            _spriteBatch.Draw(_whitePixel, new Rectangle(startingX, startingY, width, height), borderColor);
            _spriteBatch.Draw(_whitePixel, new Rectangle(startingX + borderDepth, startingY + borderDepth, width - borderDepth * 2, height - borderDepth * 2), textBackgroundColor);
            if (drawHeader)
                _spriteBatch.Draw(_whitePixel, new Rectangle(startingX + borderDepth, startingY + borderDepth, width - borderDepth * 2, headerHeight), headerBackgroundColor);

            if (drawHeader)
            {
                currentY += textSpacing;
                _spriteBatch.DrawString(_panelHeaderFont, header, new Vector2(currentX, currentY), headerTextColor);
                currentY += headerTextHeight + (textSpacing * 3);
            }

            for (int i = 0; i < text.Count; i++)
            {
                Color tmpTextColor = textColor;
                string tempText = text[i];

                if (text[i].StartsWith("$["))
                {
                    string textColorCode = text[i].Substring(2, 11); //Example: [255,255,255] or [005,255,050] 
                    string[] split = textColorCode.Split(new char[] { ',' });
                    tmpTextColor = new Color(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));

                    tempText = tempText.Substring(text[i].IndexOf("]") + 1);
                }
                _spriteBatch.DrawString(textFont, tempText, new Vector2(currentX, currentY), tmpTextColor);
                currentY += textHeight + textSpacing;
            }
        }

        //Helper functions
        private void ResetGame(GameTime gameTime)
        {
            _resetTimeSpan = gameTime.TotalGameTime;
            LoadContent();

            if (ENABLE_GAME_RESETS)
            {
                //Check to see if we calculated a better fitness score compared to the current best
                if (_gameData.CurrentFitnessScore > _gameData.BestRunSettings.FitnessScore)
                {
                    _gameData.SetNewBestRun();
                }

                //Reset all the values back to the current best run values before randomizing
                _gameData.Settings = _gameData.BestRunSettings.Settings;
                _gameData.CreatureSettings = _gameData.BestRunSettings.CreatureSettings;
                _gameData.MutationSettings = _gameData.BestRunSettings.MutationSettings;
                _gameData.CARCASS_LIFESPAN = _gameData.BestRunSettings.CARCASS_LIFESPAN;
                _gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN = _gameData.BestRunSettings.INITIAL_SPAWN_FOOD_AVG_LIFESPAN;
                _gameData.INITIAL_SPAWN_FOOD_VARIANCE = _gameData.BestRunSettings.INITIAL_SPAWN_FOOD_VARIANCE;
                _gameData.MAX_UNDIGESTED_FOOD = _gameData.BestRunSettings.MAX_UNDIGESTED_FOOD;

                float lowEndVal = 0.1f;
                float highEndVal = 2f;

                //Randomize the settings file
                //_gameData.Settings.FoodGenerationValue = _rand.Next((int)(_gameData.Settings.FoodGenerationValue * lowEndVal), (int)(_gameData.Settings.FoodGenerationValue * highEndVal));
                _gameData.Settings.TicksUntilFoodUpgradeStarts = _rand.Next((int)(_gameData.Settings.TicksUntilFoodUpgradeStarts * lowEndVal), (int)(_gameData.Settings.TicksUntilFoodUpgradeStarts * highEndVal));
                _gameData.Settings.TicksBetweenFoodUpgrades = _rand.Next((int)(_gameData.Settings.TicksBetweenFoodUpgrades * lowEndVal), (int)(_gameData.Settings.TicksBetweenFoodUpgrades * highEndVal));
                _gameData.Settings.StartingPlantRatio = _rand.Next((int)(_gameData.Settings.StartingPlantRatio * lowEndVal), (int)(_gameData.Settings.StartingPlantRatio * highEndVal));
                _gameData.Settings.EnergyGivenFromFood = _rand.Next((int)(_gameData.Settings.EnergyGivenFromFood * lowEndVal), (int)(_gameData.Settings.EnergyGivenFromFood * highEndVal));
                _gameData.Settings.EnergyConsumptionFromLayingEgg = _rand.Next((int)(_gameData.Settings.EnergyConsumptionFromLayingEgg * lowEndVal), (int)(_gameData.Settings.EnergyConsumptionFromLayingEgg * highEndVal));
                _gameData.Settings.EnergyDepletionFromMovement = _rand.Next((int)(_gameData.Settings.EnergyDepletionFromMovement * lowEndVal), (int)(_gameData.Settings.EnergyDepletionFromMovement * highEndVal));
                _gameData.Settings.EnergyDepletionPercentFromComplexity = _rand.Next((int)(_gameData.Settings.EnergyDepletionPercentFromComplexity * lowEndVal), (int)(_gameData.Settings.EnergyDepletionPercentFromComplexity * highEndVal));

                _gameData.CreatureSettings.StartingEggIncubationMin = _rand.Next((int)(_gameData.CreatureSettings.StartingEggIncubationMin * lowEndVal), (int)(_gameData.CreatureSettings.StartingEggIncubationMin * highEndVal));
                _gameData.CreatureSettings.StartingEggIncubationMax = _rand.Next((int)(_gameData.CreatureSettings.StartingEggIncubationMax * lowEndVal), (int)(_gameData.CreatureSettings.StartingEggIncubationMax * highEndVal));
                _gameData.CreatureSettings.StartingEggIntervalMin = _rand.Next((int)(_gameData.CreatureSettings.StartingEggIntervalMin * lowEndVal), (int)(_gameData.CreatureSettings.StartingEggIntervalMin * highEndVal));
                _gameData.CreatureSettings.StartingEggIntervalMax = _rand.Next((int)(_gameData.CreatureSettings.StartingEggIntervalMax * lowEndVal), (int)(_gameData.CreatureSettings.StartingEggIntervalMax * highEndVal));
                _gameData.CreatureSettings.StartingFoodDigestionMin = _rand.Next((int)(_gameData.CreatureSettings.StartingFoodDigestionMin * lowEndVal), (int)(_gameData.CreatureSettings.StartingFoodDigestionMin * highEndVal));
                _gameData.CreatureSettings.StartingFoodDigestionMax = _rand.Next((int)(_gameData.CreatureSettings.StartingFoodDigestionMax * lowEndVal), (int)(_gameData.CreatureSettings.StartingFoodDigestionMax * highEndVal));
                _gameData.CreatureSettings.StartingSpeedMin = _rand.Next((int)(_gameData.CreatureSettings.StartingSpeedMin * lowEndVal), (int)(_gameData.CreatureSettings.StartingSpeedMin * highEndVal));
                _gameData.CreatureSettings.StartingSpeedMax = _rand.Next((int)(_gameData.CreatureSettings.StartingSpeedMax * lowEndVal), (int)(_gameData.CreatureSettings.StartingSpeedMax * highEndVal));
                _gameData.CreatureSettings.StartingLifespanMin = _rand.Next((int)(_gameData.CreatureSettings.StartingLifespanMin * lowEndVal), (int)(_gameData.CreatureSettings.StartingLifespanMin * highEndVal));
                _gameData.CreatureSettings.StartingLifespanMax = _rand.Next((int)(_gameData.CreatureSettings.StartingLifespanMax * lowEndVal), (int)(_gameData.CreatureSettings.StartingLifespanMax * highEndVal));
                _gameData.CreatureSettings.StartingEnergy = _rand.Next((int)(_gameData.CreatureSettings.StartingEnergy * lowEndVal), (int)(_gameData.CreatureSettings.StartingEnergy * highEndVal));
                
                _gameData.MutationSettings.MutationBonusPercent = _rand.Next((int)(_gameData.MutationSettings.MutationBonusPercent * lowEndVal), (int)(_gameData.MutationSettings.MutationBonusPercent * highEndVal));

                _gameData.CARCASS_LIFESPAN = _rand.Next((int)(_gameData.CARCASS_LIFESPAN * lowEndVal), (int)(_gameData.CARCASS_LIFESPAN * highEndVal));
                _gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN = _rand.Next((int)(_gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN * lowEndVal), (int)(_gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN * highEndVal));
                _gameData.INITIAL_SPAWN_FOOD_VARIANCE = _rand.Next((int)(_gameData.INITIAL_SPAWN_FOOD_VARIANCE * lowEndVal), (int)(_gameData.INITIAL_SPAWN_FOOD_VARIANCE * highEndVal));
                _gameData.MAX_UNDIGESTED_FOOD = _rand.Next((int)(_gameData.MAX_UNDIGESTED_FOOD * lowEndVal), (int)(_gameData.MAX_UNDIGESTED_FOOD * highEndVal));

                //Handle Mins greater than the max by swapping the min for the max
                if (_gameData.CreatureSettings.StartingEggIncubationMin > _gameData.CreatureSettings.StartingEggIncubationMax)
                {
                    float tmp = _gameData.CreatureSettings.StartingEggIncubationMin;
                    _gameData.CreatureSettings.StartingEggIncubationMin = _gameData.CreatureSettings.StartingEggIncubationMax;
                    _gameData.CreatureSettings.StartingEggIncubationMax = tmp;
                }
                if (_gameData.CreatureSettings.StartingEggIntervalMin > _gameData.CreatureSettings.StartingEggIntervalMax)
                {
                    float tmp = _gameData.CreatureSettings.StartingEggIntervalMin;
                    _gameData.CreatureSettings.StartingEggIntervalMin = _gameData.CreatureSettings.StartingEggIntervalMax;
                    _gameData.CreatureSettings.StartingEggIntervalMax = tmp;
                }
                if (_gameData.CreatureSettings.StartingFoodDigestionMin > _gameData.CreatureSettings.StartingFoodDigestionMax)
                {
                    float tmp = _gameData.CreatureSettings.StartingFoodDigestionMin;
                    _gameData.CreatureSettings.StartingFoodDigestionMin = _gameData.CreatureSettings.StartingFoodDigestionMax;
                    _gameData.CreatureSettings.StartingFoodDigestionMax = tmp;
                }
                if (_gameData.CreatureSettings.StartingSpeedMin > _gameData.CreatureSettings.StartingSpeedMax)
                {
                    float tmp = _gameData.CreatureSettings.StartingSpeedMin;
                    _gameData.CreatureSettings.StartingSpeedMin = _gameData.CreatureSettings.StartingSpeedMax;
                    _gameData.CreatureSettings.StartingSpeedMax = tmp;
                }
                if (_gameData.CreatureSettings.StartingLifespanMin > _gameData.CreatureSettings.StartingLifespanMax)
                {
                    float tmp = _gameData.CreatureSettings.StartingLifespanMin;
                    _gameData.CreatureSettings.StartingLifespanMin = _gameData.CreatureSettings.StartingLifespanMax;
                    _gameData.CreatureSettings.StartingLifespanMax = tmp;
                }
            }
        }
        private void InitVariables()
        {
            _rand = new Random();

            _inputState = new InputState();
            _player = new Player();
            _connectionManager = new ConnectionManager();
            _debugTimer = new DebugTimer();
            _elapsedSecondsSinceTick = 0;
            _elapsedTimeSinceFoodGeneration = 0;
            _elapsedTimeSinceDebugTimeData = 0;
            _elapsedTicksSinceSecondProcessing = 0;
            _fps = 0;
            _frames = 0;
            _elapsedSeconds = 0.0;
            _totalElapsedSeconds = 0.0;
            _creatureIdCtr = 0;
            _elapsedTicksForInitialFoodUpgrade = 0;
            _elapsedTicksSinceFoodUpgrade = 0;
            _gameData.CurrentMaxFoodLevel = 2;
            _climateHeight = (int)(_gameData.Settings.WorldSize * (_gameData.Settings.ClimateHeightPercent * 0.01));
            _totalElapsedTime = new TimeSpan();
        }
        private void BuildSettingsPanel()
        {
            //Build the Panel
            _settingsTabPanel = new TabPanel();
            _settingsTabPanel.Tabs = new List<Tab>();
            _settingsTabPanel.PanelWidth = _graphics.GraphicsDevice.Viewport.Width / 2;
            _settingsTabPanel.PanelHeight = (int)(_graphics.GraphicsDevice.Viewport.Height * 0.80);
            _settingsTabPanel.Position = new Point(((_graphics.GraphicsDevice.Viewport.Width / 2) - (_settingsTabPanel.PanelWidth / 2)), ((_graphics.GraphicsDevice.Viewport.Height / 2) - (_settingsTabPanel.PanelHeight / 2)));
            _settingsTabPanel.SaveButton = new Button() { Texture = _saveButton };
            _settingsTabPanel.CloseButton = new Button() { Texture = _closeButton };
            _settingsTabPanel.DefaultButton = new Button() { Texture = _defaultButton };
            //Build Tab section

            _settingsTabPanel.Tabs.Add(BuildWorldSettingsTab());
            _settingsTabPanel.Tabs.Add(BuildCreatureSettingsTab());
            _settingsTabPanel.Tabs.Add(BuildMutationSettingsTab());

            _settingsTabPanel.ActiveTab = 0;
        }
        private Tab BuildWorldSettingsTab()
        {
            int sliderSpacing = 5;
            int sliderBarWidth = 300;
            int workingX = sliderSpacing;
            int workingY = sliderSpacing;
            Tab worldTab = new Tab();
            worldTab.TabNumber = 1;
            worldTab.TabText = "Map Settings";

            UIControls _uiControls = new UIControls();

            //World Size
            Slider tmpSlider = new Slider();
            tmpSlider.SliderText = "World Size";
            tmpSlider.SliderCode = SettingEnum.WorldSize;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 50;
            tmpSlider.MaxValue = 50000;
            tmpSlider.CurrentValue = _gameData.Settings.WorldSize;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Climate Percent
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Climate Percent";
            tmpSlider.SliderCode = SettingEnum.ClimatePercent;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 49;
            tmpSlider.CurrentValue = _gameData.Settings.ClimateHeightPercent;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Starting Food Ratio
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Starting Food Ratio";
            tmpSlider.SliderCode = SettingEnum.StartingFoodRatio;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 500;
            tmpSlider.CurrentValue = _gameData.Settings.StartingFoodRatio;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Food Generation value
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Food Generation Value";
            tmpSlider.SliderCode = SettingEnum.FoodGenerationValue;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.Settings.FoodGenerationValue;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            //Time to start Food Upgrade
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Food Upgrade Start Time";
            tmpSlider.SliderCode = SettingEnum.SecondsUntilFoodUpgradesStart;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 5000;
            tmpSlider.CurrentValue = _gameData.Settings.TicksUntilFoodUpgradeStarts;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            //Time between food upgrades
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Time Between Food Upgrades";
            tmpSlider.SliderCode = SettingEnum.SecondsBetweenFoodUpgrades;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 500;
            tmpSlider.CurrentValue = _gameData.Settings.TicksBetweenFoodUpgrades;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            //Food upgrade amount
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Food Upgrade Amount";
            tmpSlider.SliderCode = SettingEnum.FoodUpgradeAmount;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 10;
            tmpSlider.CurrentValue = _gameData.Settings.FoodUpgradeAmount;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            //Food max level
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Food Max Level";
            tmpSlider.SliderCode = SettingEnum.MaxFoodlevel;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 250;
            tmpSlider.CurrentValue = _gameData.Settings.MaxFoodLevel;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            //Starting Plant ratio
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Starting Plants";
            tmpSlider.SliderCode = SettingEnum.StartingPlantRatio;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 500;
            tmpSlider.CurrentValue = _gameData.Settings.StartingPlantRatio;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Starting Creature ratio
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Starting Creatures";
            tmpSlider.SliderCode = SettingEnum.StartingCreatureRatio;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 250;
            tmpSlider.CurrentValue = _gameData.Settings.StartingCreatureRatio;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Energy given from Food
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Energy From Food";
            tmpSlider.SliderCode = SettingEnum.EnergyGivenFromFood;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 500;
            tmpSlider.CurrentValue = _gameData.Settings.EnergyGivenFromFood;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            //Energy Loss from Laying egg
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Lay egg energy loss";
            tmpSlider.SliderCode = SettingEnum.EnergyLossFromLayingEgg;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 500;
            tmpSlider.CurrentValue = _gameData.Settings.EnergyConsumptionFromLayingEgg;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            //Energy depletion from movement
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Movement Energy Loss";
            tmpSlider.SliderCode = SettingEnum.EnergyDepletionFromMovementRate;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 250;
            tmpSlider.CurrentValue = _gameData.Settings.EnergyDepletionFromMovement;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Energy depletion from complexity
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Complexity Energy Loss";
            tmpSlider.SliderCode = SettingEnum.EnergyDepletionPercentFromComplexity;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 200;
            tmpSlider.CurrentValue = _gameData.Settings.EnergyDepletionPercentFromComplexity;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            worldTab.Controls = _uiControls;

            return worldTab;
        }
        private Tab BuildCreatureSettingsTab()
        {
            int sliderSpacing = 5;
            int sliderBarWidth = 300;
            int workingX = sliderSpacing;
            int workingY = sliderSpacing;
            Tab worldTab = new Tab();
            worldTab.TabNumber = 2;
            worldTab.TabText = "Creature Settings";

            UIControls _uiControls = new UIControls();

            //Egg Interval Min
            Slider tmpSlider = new Slider();
            tmpSlider.SliderText = "Egg Interval Min";
            tmpSlider.SliderCode = SettingEnum.StartingEggIntervalMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingEggIntervalMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Egg Interval Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Egg Interval Max";
            tmpSlider.SliderCode = SettingEnum.StartingEggIntervalMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingEggIntervalMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            //Egg Incubation Min
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Egg Incubation Min";
            tmpSlider.SliderCode = SettingEnum.StartingEggIncubationMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 500;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingEggIncubationMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Egg Incubation Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Egg Incubation Max";
            tmpSlider.SliderCode = SettingEnum.StartingEggIncubationMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 500;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingEggIncubationMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Food Digestion Min
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Food Digestion Min";
            tmpSlider.SliderCode = SettingEnum.StartingFoodDigestionMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingFoodDigestionMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Food Digestion Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Food Digestion Max";
            tmpSlider.SliderCode = SettingEnum.StartingFoodDigestionMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingFoodDigestionMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Speed Min
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Speed Min";
            tmpSlider.SliderCode = SettingEnum.StartingSpeedMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 50;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingSpeedMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Speed Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Speed Max";
            tmpSlider.SliderCode = SettingEnum.StartingSpeedMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 50;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingSpeedMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Lifespan Min
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Lifespan Min";
            tmpSlider.SliderCode = SettingEnum.StartingLifespanMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 1000;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingLifespanMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Lifespan Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Lifespan Max";
            tmpSlider.SliderCode = SettingEnum.StartingLifespanMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 1000;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingLifespanMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Herbavore Min
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Herbavore Min";
            tmpSlider.SliderCode = SettingEnum.StartingHerbavoreLevelMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 50;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingHerbavoreLevelMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Herbavore Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Herbavore Max";
            tmpSlider.SliderCode = SettingEnum.StartingHerbavoreLevelMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 50;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingHerbavoreLevelMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Carnivore Min
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Carnivore Min";
            tmpSlider.SliderCode = SettingEnum.StartingCarnivoreLevelMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 50;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingCarnivoreLevelMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Carnivore Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Carnivore Max";
            tmpSlider.SliderCode = SettingEnum.StartingCarnivoreLevelMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 50;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingCarnivoreLevelMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Scavenger Min
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Scavenger Min";
            tmpSlider.SliderCode = SettingEnum.StartingScavengerLevelMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 50;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingScavengerLevelMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Scavenger Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Scavenger Max";
            tmpSlider.SliderCode = SettingEnum.StartingScavengerLevelMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 50;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingScavengerLevelMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Omnivore Min
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Omnivore Min";
            tmpSlider.SliderCode = SettingEnum.StartingOmnivoreLevelMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 50;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingOmnivoreLevelMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Omnivore Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Omnivore Max";
            tmpSlider.SliderCode = SettingEnum.StartingOmnivoreLevelMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 50;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingOmnivoreLevelMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Cold Tolerance Min
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Cold Tolerance Min";
            tmpSlider.SliderCode = SettingEnum.StartingColdToleranceMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingColdToleranceMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Cold Tolerance Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Cold Tolerance Max";
            tmpSlider.SliderCode = SettingEnum.StartingColdToleranceMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingColdToleranceMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Hot Tolerance Min
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Hot Tolerance Min";
            tmpSlider.SliderCode = SettingEnum.StartingHotToleranceMin;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingHotToleranceMin;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Hot Tolerance Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Hot Tolerance Max";
            tmpSlider.SliderCode = SettingEnum.StartingHotToleranceMax;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingHotToleranceMax;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            //Energy Max
            tmpSlider = new Slider();
            tmpSlider.SliderText = "Energy";
            tmpSlider.SliderCode = SettingEnum.StartingEnergy;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 5000;
            tmpSlider.CurrentValue = _gameData.CreatureSettings.StartingEnergy;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            worldTab.Controls = _uiControls;

            return worldTab;
        }
        private Tab BuildMutationSettingsTab()
        {
            int sliderSpacing = 5;
            int sliderBarWidth = 300;
            int workingX = sliderSpacing;
            int workingY = sliderSpacing;
            Tab mutationTab = new Tab();
            mutationTab.TabNumber = 1;
            mutationTab.TabText = "Mutation Settings";

            UIControls _uiControls = new UIControls();

            Slider tmpSlider = new Slider();
            tmpSlider.SliderText = "Chance for Positive Mutation";
            tmpSlider.SliderCode = SettingEnum.ChanceToIncreaseValue;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.ChanceToIncreaseValue;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Mutation Amount Percent";
            tmpSlider.SliderCode = SettingEnum.ChangeAmount;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 10;
            tmpSlider.CurrentValue = _gameData.MutationSettings.ChangeAmount;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Mutation Bonus Percent";
            tmpSlider.SliderCode = SettingEnum.MutationBonusPercent;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.MutationBonusPercent;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Egg Camo";
            tmpSlider.SliderCode = SettingEnum.EggCamo;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.EggCamo;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Egg Incubation";
            tmpSlider.SliderCode = SettingEnum.EggIncubation;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.EggIncubation;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Egg Interval";
            tmpSlider.SliderCode = SettingEnum.EggInterval;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.EggInterval;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Egg Toxicity";
            tmpSlider.SliderCode = SettingEnum.EggToxicity;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.EggToxicity;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Food Type";
            tmpSlider.SliderCode = SettingEnum.FoodType;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.FoodType;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Food Digestion";
            tmpSlider.SliderCode = SettingEnum.FoodDigestion;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.FoodDigestion;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Speed";
            tmpSlider.SliderCode = SettingEnum.Speed;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.Speed;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Lifespan";
            tmpSlider.SliderCode = SettingEnum.Lifespan;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.Lifespan;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Sight";
            tmpSlider.SliderCode = SettingEnum.Sight;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.Sight;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Attraction";
            tmpSlider.SliderCode = SettingEnum.Attraction;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.Attraction;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Camo";
            tmpSlider.SliderCode = SettingEnum.Camo;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.Camo;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Cloning";
            tmpSlider.SliderCode = SettingEnum.Cloning;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.Cloning;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Cold Climate Tolerance";
            tmpSlider.SliderCode = SettingEnum.ColdClimateTolerance;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.ColdClimateTolerance;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Hot Climate Tolerance";
            tmpSlider.SliderCode = SettingEnum.HotClimateTolerance;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.HotClimateTolerance;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Herbavore";
            tmpSlider.SliderCode = SettingEnum.Herbavore;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.Herbavore;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Carnivore";
            tmpSlider.SliderCode = SettingEnum.Carnivore;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.Carnivore;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Omnivore";
            tmpSlider.SliderCode = SettingEnum.Omnivore;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.Omnivore;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            tmpSlider = new Slider();
            tmpSlider.SliderText = "Scavenger";
            tmpSlider.SliderCode = SettingEnum.Scavenger;
            tmpSlider.SliderPosition = new Vector2(workingX, workingY);
            tmpSlider.BarWidth = sliderBarWidth;
            tmpSlider.BarHeight = 10;
            tmpSlider.MarkerHeight = 20;
            tmpSlider.MarkerWidth = 10;
            tmpSlider.MinValue = 0;
            tmpSlider.MaxValue = 100;
            tmpSlider.CurrentValue = _gameData.MutationSettings.Scavenger;
            tmpSlider.ShowPercent = true;
            tmpSlider.FillSlider = true;
            tmpSlider.Font = _panelHeaderFont;
            tmpSlider.Initialize(_graphics.GraphicsDevice);
            _uiControls.Sliders.Add(tmpSlider);

            workingY += tmpSlider.MarkerHeight + sliderSpacing;

            mutationTab.Controls = _uiControls;

            return mutationTab;
        }
        private List<string> BuildStringList(List<Creature> creatureList)
        {
            List<Creature> blankList = new List<Creature>();

            return BuildStringList(creatureList, blankList);
        }
        private List<string> BuildStringList(List<Creature> creatureList1, List<Creature> creatureList2)
        {
            List<string> builtList = new List<string>();

            if (creatureList1.Count > 0)
            {
                //Build Stats List array for each creature
                List<CreatureStats> statsList = new List<CreatureStats>();
                foreach (Creature c in creatureList1)
                {
                    statsList.Add(c.GetCreatureStatistics(_gameRandSeed, _sessionID, _totalElapsedSeconds));
                }

                //Build the column headers row if it does not exist
                if (builtList.Count <= 0 && statsList.Count > 0)
                {
                    string headers = String.Empty;

                    foreach (string s in statsList[0].FieldHeaders)
                    {
                        headers += s + ",";
                    }

                    if (!String.IsNullOrEmpty(headers))
                    {
                        headers = headers.Substring(0, headers.Length - 1);
                        builtList.Add(headers);
                    }
                }

                foreach (CreatureStats cs in statsList)
                {
                    string csvRow = String.Empty;

                    foreach (string s in cs.StringStats)
                    {
                        csvRow += s + ",";
                    }
                    foreach (int i in cs.IntStats)
                    {
                        csvRow += i.ToString() + ",";
                    }
                    foreach (int f in cs.FloatStats)
                    {
                        csvRow += f.ToString() + ",";
                    }

                    if (!String.IsNullOrEmpty(csvRow))
                    {
                        csvRow = csvRow.Substring(0, csvRow.Length - 1);
                        builtList.Add(csvRow);
                    }
                }
            }
            if (creatureList2.Count > 0)
            {
                //Build Stats List array for each creature
                List<CreatureStats> statsList = new List<CreatureStats>();
                foreach (Creature c in creatureList2)
                {
                    statsList.Add(c.GetCreatureStatistics(_gameRandSeed, _sessionID, _totalElapsedSeconds));
                }

                //Build the column headers row if it does not exist
                if (builtList.Count <= 0 && statsList.Count > 0)
                {
                    string headers = String.Empty;

                    foreach (string s in statsList[0].FieldHeaders)
                    {
                        headers += s + ",";
                    }

                    if (!String.IsNullOrEmpty(headers))
                    {
                        headers = headers.Substring(0, headers.Length - 1);
                        builtList.Add(headers);
                    }
                }

                foreach (CreatureStats cs in statsList)
                {
                    string csvRow = String.Empty;

                    foreach (string s in cs.StringStats)
                    {
                        csvRow += s + ",";
                    }
                    foreach (int i in cs.IntStats)
                    {
                        csvRow += i.ToString() + ",";
                    }
                    foreach (int f in cs.FloatStats)
                    {
                        csvRow += f.ToString() + ",";
                    }

                    if (!String.IsNullOrEmpty(csvRow))
                    {
                        csvRow = csvRow.Substring(0, csvRow.Length - 1);
                        builtList.Add(csvRow);
                    }
                }
            }

            return builtList;
        }
        private List<string> BuildStringListFromClass(object obj)
        {
            return BuildStringListFromClass(obj, null, null);
        }
        private List<string> BuildStringListFromClass(object obj1, object obj2, object obj3)
        {
            List<string> builtList = new List<string>();
            string headers = String.Empty;
            string csvRow = String.Empty;
            string className = String.Empty;

            if (obj1 != null)
            {
                className = obj1.GetType().Name;
                PropertyInfo[] pi1 = obj1.GetType().GetProperties();

                foreach (PropertyInfo p in pi1)
                {
                    headers += className + "_" + p.Name + ",";
                }
                foreach (PropertyInfo p in pi1)
                {
                    csvRow += p.GetValue(obj1, null).ToString() + ",";
                }
            }
            if (obj2 != null)
            {
                className = obj2.GetType().Name;
                PropertyInfo[] pi2 = obj2.GetType().GetProperties();

                foreach (PropertyInfo p in pi2)
                {
                    headers += className + "_" + p.Name + ",";
                }
                foreach (PropertyInfo p in pi2)
                {
                    csvRow += p.GetValue(obj2, null).ToString() + ",";
                }
            }
            if (obj3 != null)
            {
                className = obj3.GetType().Name;
                PropertyInfo[] pi3 = obj3.GetType().GetProperties();

                foreach (PropertyInfo p in pi3)
                {
                    headers += className + "_" + p.Name + ",";
                }
                foreach (PropertyInfo p in pi3)
                {
                    csvRow += p.GetValue(obj3, null).ToString() + ",";
                }
            }

            if (!String.IsNullOrEmpty(headers))
            {
                headers = headers.Substring(0, headers.Length - 1);
                builtList.Add(headers);
            }
            if (!String.IsNullOrEmpty(csvRow))
            {
                csvRow = csvRow.Substring(0, csvRow.Length - 1);
                builtList.Add(csvRow);
            }

            return builtList;
        }
        private List<string> BuildFitnessFile()
        {
            List<string> builtList = new List<string>();

            builtList.Add("Statistic,Value");
            builtList.Add("Fitness Score," + _gameData.CurrentFitnessScore);
            builtList.Add("Alive Creatures," + _gameData.MapStatistics.AliveCreatures.ToString());
            builtList.Add("Dead Creatures," + _gameData.MapStatistics.DeadCreatures.ToString());
            builtList.Add("Eggs On Map," + _gameData.MapStatistics.EggsOnMap.ToString());
            builtList.Add("Food On Map," + _gameData.MapStatistics.FoodOnMap.ToString());
            builtList.Add("Plants On Map," + _gameData.MapStatistics.PlantsOnMap.ToString());
            builtList.Add("Percent Herbavore," + _gameData.MapStatistics.PercentHerbavore.ToString());
            builtList.Add("Percent Carnivore," + _gameData.MapStatistics.PercentCarnivore.ToString());
            builtList.Add("Percent Scavenger," + _gameData.MapStatistics.PercentScavenger.ToString());
            builtList.Add("Percent Omnivore," + _gameData.MapStatistics.PercentOmnivore.ToString());
            builtList.Add("Unique Species," + _gameData.MapStatistics.UniqueSpecies.ToString());

            return builtList;
        }
        private List<string> BuildMagicNumberList()
        {
            List<string> builtList = new List<string>();

            builtList.Add("Variable,Value");
            builtList.Add("CARCASS_LIFESPAN," + _gameData.CARCASS_LIFESPAN);
            builtList.Add("INITIAL_SPAWN_FOOD_AVG_LIFESPAN," + _gameData.INITIAL_SPAWN_FOOD_AVG_LIFESPAN);
            builtList.Add("INITIAL_SPAWN_FOOD_VARIANCE," + _gameData.INITIAL_SPAWN_FOOD_VARIANCE);
            builtList.Add("MAX_UNDIGESTED_FOOD," + _gameData.MAX_UNDIGESTED_FOOD);

            return builtList;
        }
        private List<string> BuildDebugTimerList()
        {
            List<string> builtList = new List<string>();
            Type type = _debugTimer.GetType();
            PropertyInfo[] properties = type.GetProperties();

            double totalTime = 0;
            foreach (PropertyInfo property in properties)
            {
                totalTime += (double)property.GetValue(_debugTimer, null);
            }

            builtList.Add("Name,Value,Percent");
            foreach (PropertyInfo property in properties)
            {
                double percent = (double)property.GetValue(_debugTimer, null);
                percent = percent / totalTime;
                percent = Math.Round(percent, 2);

                builtList.Add(property.Name + "," + property.GetValue(_debugTimer, null) + "," + percent.ToString());
            }

            return builtList;
        }
        private void SpawnFood()
        {
            //SpawnFood(-1f);
            SpawnFood(_foodLifeSpan);
        }
        private void SpawnFood(float lifeSpan)
        {
            Vector2 position = new Vector2(_rand.Next(_foodTexture.Width, _gameData.Settings.WorldSize - _foodTexture.Width), _rand.Next(_foodTexture.Height, _gameData.Settings.WorldSize - _foodTexture.Height));
            SpawnFood(position, _gameData.CurrentMaxFoodLevel, lifeSpan);
        }
        private void SpawnFood(Vector2 position, float maxHerbavoreLevel)
        {
            //Pass in -1 lifespan to disable food life making the food last forever
            //SpawnFood(position, maxHerbavoreLevel, -1f);

            SpawnFood(position, maxHerbavoreLevel, _foodLifeSpan);
        }
        private void SpawnFood(Vector2 position, float maxHerbavoreLevel, float lifeSpan)
        {
            SpawnFood(position, maxHerbavoreLevel, lifeSpan, false);
        }
        private void SpawnFood(Vector2 position, float maxHerbavoreLevel, float lifeSpan, bool fromDeath)
        {
            Food food = new Food();

            food.WorldSize = _gameData.Settings.WorldSize;
            food.ClimateHeightPercent = _gameData.Settings.ClimateHeightPercent;
            food.TicksPerSecond = _gameData.TicksPerSecond;
            if (fromDeath)
            {
                food.Texture = _carcassTexture;
            }
            else
            {
                food.Texture = _foodTexture;
            }
            food.Position = position;
            food.Lifespan = lifeSpan * 10f;

            //When the creature dies it throws the food randomly around it, make sure that we do not have food out of the world spawning
            if (food.Bounds.Left < 0)
            {
                food.Position = new Vector2((float)Math.Ceiling(food.Texture.Width / 2.0), food.Position.Y);
            }
            else if (food.Bounds.Right > _gameData.Settings.WorldSize)
            {
                food.Position = new Vector2(_gameData.Settings.WorldSize - (float)Math.Ceiling(food.Texture.Width / 2.0), food.Position.Y);
            }

            if (food.Bounds.Top < 0)
            {
                food.Position = new Vector2(food.Position.X, (float)Math.Ceiling(food.Texture.Height / 2.0));
            }
            else if (food.Bounds.Bottom > _gameData.Settings.WorldSize)
            {
                food.Position = new Vector2(food.Position.X, _gameData.Settings.WorldSize - (float)Math.Ceiling(food.Texture.Height / 2.0));
            }

            food.GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);

            //50% chance to apply a food level to the food
            if (!fromDeath && maxHerbavoreLevel > 0 && _rand.Next(0, 100) > 50)
                food.FoodStrength = _rand.Next(1, (int)maxHerbavoreLevel);
            else
                food.FoodStrength = 0;

            if (!fromDeath)
            {
                food.FoodType = _rand.Next(0, 3); //Assign a random type to food

                if (food.FoodType == 0)
                    food.FoodColor = Color.Blue;
                else if (food.FoodType == 1)
                    food.FoodColor = Color.Red;
                else
                    food.FoodColor = Color.Green;
            }
            else
            {
                food.FoodType = -1;
                food.FoodColor = Color.White;
            }

            food.DisplayText = "(" + food.FoodStrength + ")";
            food.TextSize = _foodFont.MeasureString(food.DisplayText);

            _gameData.Food.Add(food);
            _gameData.AddFoodToGrid(food);
        }
        private void SpawnStartingPlant()
        {
            Plant plant = new Plant();
            plant.WorldSize = _gameData.Settings.WorldSize;
            plant.ClimateHeightPercent = _gameData.Settings.ClimateHeightPercent;
            plant.InitializeNewPlant(_rand, _textureList, _gameData);
            plant.Position = new Vector2(_rand.Next(plant.Texture.Width, _gameData.Settings.WorldSize - plant.Texture.Width), _rand.Next(plant.Texture.Height, _gameData.Settings.WorldSize - plant.Texture.Height));
            plant.GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);
            plant.GetExpandedGridPositions(_gameData); //Calculate expanded grid positions after the position has been set
            plant.DisplayText = plant.FoodAmount.ToString() + "/" + plant.FoodAmountCap;
            plant.TextSize = _foodFont.MeasureString(plant.DisplayText);

            _gameData.Plants.Add(plant);
            _gameData.AddPlantToGrid(plant);
        }
        private void SpawnStartingCreature()
        {
            Creature creature = new Creature();
            creature.WorldSize = _gameData.Settings.WorldSize;
            creature.ClimateHeightPercent = _gameData.Settings.ClimateHeightPercent;
            creature.InitNewCreature(_rand, ref _names, _gameData.NextSpeciesId, ref _creatureIdCtr, _gameData);
            creature.Texture = DetermineCreatureTexture(creature);
            creature.Position = new Vector2(_rand.Next(creature.Texture.Width, _gameData.Settings.WorldSize - creature.Texture.Width), _rand.Next(creature.Texture.Height, _gameData.Settings.WorldSize - creature.Texture.Height));
            creature.GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);

            _gameData.Creatures.Add(creature);
            _gameData.AddCreatureToGrid(creature);
        }
        private Texture2D DetermineCreatureTexture(Creature creature)
        {
            //Omnivore must be first
            if (creature.IsOmnivore && creature.Sight > 0)
            {
                return _omnivoreSightTexture;
            }
            else if (creature.IsOmnivore)
            {
                return _omnivoreTexture;
            }
            else if (creature.IsCarnivore && creature.Sight > 0)
            {
                return _carnivoreSightTexture;
            }
            else if (creature.IsCarnivore)
            {
                return _carnivoreTexture;
            }
            else if (creature.IsHerbavore && creature.Sight > 0)
            {
                return _herbavoreSightTexture;
            }
            else if (creature.IsHerbavore)
            {
                return _herbavoreTexture;
            }
            else if (creature.IsScavenger && creature.Sight > 0)
            {
                return _scavengerSightTexture;
            }
            else if (creature.IsScavenger)
            {
                return _scavengerTexture;
            }
            else
            {
                return _herbavoreTexture;
            }
        }
        private double CalculateFitness()
        {
            double score = 0d;

            double carnivorePointsFromPerfect = Math.Abs(10d - _gameData.MapStatistics.PercentCarnivore);
            double herbavorePointsFromPerfect = Math.Abs(70d - _gameData.MapStatistics.PercentHerbavore);
            double scavengerPointsFromPerfect = Math.Abs(15d - _gameData.MapStatistics.PercentScavenger);
            double omnivorePointsFromPerfect = Math.Abs(5d - _gameData.MapStatistics.PercentOmnivore);
            double creatureCountFromPerfect = Math.Abs(1000d - _gameData.MapStatistics.AliveCreatures);

            if (creatureCountFromPerfect > 100d)
                creatureCountFromPerfect = 100d;

            if (carnivorePointsFromPerfect > 0)
                score += 100d - carnivorePointsFromPerfect;
            if(herbavorePointsFromPerfect > 0)
                score += 100d - herbavorePointsFromPerfect;
            if(scavengerPointsFromPerfect > 0)
                score += 100d - scavengerPointsFromPerfect;
            if(omnivorePointsFromPerfect > 0)
                score += 100d - omnivorePointsFromPerfect;

            score += 100d - creatureCountFromPerfect;

            if (_fps < 30)
            {
                score -= 100;
            }

            return score;
        }

        //Debug Creature spawns
        private void SpawnTwoTestCreaturesWithInterceptPaths()
        {
            Creature creature = new Creature();
            creature.WorldSize = _gameData.Settings.WorldSize;
            creature.ClimateHeightPercent = _gameData.Settings.ClimateHeightPercent;
            creature.InitNewCreature(_rand, ref _names, _gameData.NextSpeciesId, ref _creatureIdCtr, _gameData);
            creature.Species = "Cheater";
            creature.OriginalSpecies = "Cheater";
            //creature.Position = new Vector2(210, 100);
            creature.Position = new Vector2(200, 100); //Position 1, does NOT work
            creature.Rotation = MathHelper.ToRadians(220);
            creature.ColdClimateTolerance = 10;
            creature.HotClimateTolerance = 0;
            creature.Sight = 30;
            creature.Carnivore = 10;
            creature.Speed = 18;
            creature.IsHerbavore = false;
            creature.IsCarnivore = true;
            creature.GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);
            creature.Texture = DetermineCreatureTexture(creature);
            _gameData.AddCreatureToGrid(creature);
            _gameData.Creatures.Add(creature);

            Creature creature2 = new Creature();
            creature2.WorldSize = _gameData.Settings.WorldSize;
            creature2.ClimateHeightPercent = _gameData.Settings.ClimateHeightPercent;
            creature2.InitNewCreature(_rand, ref _names, _gameData.NextSpeciesId, ref _creatureIdCtr, _gameData);
            creature2.Species = "Cheater2";
            creature2.OriginalSpecies = "Cheater2";
            //creature2.Position = new Vector2(10, 100);
            creature2.Position = new Vector2(10, 100); //Position 1, does NOT work
            creature2.Rotation = MathHelper.ToRadians(135);
            creature2.ColdClimateTolerance = 10;
            creature2.HotClimateTolerance = 0;
            creature2.Sight = 0;
            creature2.Speed = 23;
            creature2.Herbavore = 1;
            creature2.Scavenger = 10;
            creature2.IsHerbavore = false;
            creature2.IsCarnivore = false;
            creature2.IsScavenger = true;
            creature2.GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);
            creature2.Texture = DetermineCreatureTexture(creature2);
            _gameData.AddCreatureToGrid(creature2);
            _gameData.Creatures.Add(creature2);

            Vector2 foodPos = new Vector2(creature.Position.X + 15, creature.Position.Y - 100);
            SpawnFood(foodPos, 1);
            _gameData.Focus = creature;
            _gameData.FocusIndex = _gameData.Creatures.Count - 1;
        }
        private void SpawnOneTestCarnivore()
        {
            Creature creature = new Creature();
            creature.WorldSize = _gameData.Settings.WorldSize;
            creature.ClimateHeightPercent = _gameData.Settings.ClimateHeightPercent;
            creature.InitNewCreature(_rand, ref _names, _gameData.NextSpeciesId, ref _creatureIdCtr, _gameData);
            creature.Species = "Cheater";
            creature.OriginalSpecies = "Cheater";
            creature.Position = new Vector2(200, 100);
            creature.Rotation = MathHelper.ToRadians(220);
            creature.ColdClimateTolerance = 10;
            creature.HotClimateTolerance = 0;
            creature.Sight = 15;
            creature.Carnivore = 10;
            creature.Speed = 30;
            creature.Lifespan = 100000;
            creature.Energy = 100000;
            creature.EggInterval = 100000; //So it cannot lay any eggs
            creature.IsHerbavore = false;
            creature.IsCarnivore = true;
            creature.GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);
            creature.Texture = DetermineCreatureTexture(creature);
            _gameData.AddCreatureToGrid(creature);
            _gameData.Creatures.Add(creature);

            Vector2 foodPos = new Vector2(creature.Position.X + 15, creature.Position.Y - 100);
            SpawnFood(foodPos, 1);
            _gameData.Focus = creature;
            _gameData.FocusIndex = _gameData.Creatures.Count - 1;
        }
        private void SpawnOneTestScavenger()
        {
            Creature creature = new Creature();
            creature.WorldSize = _gameData.Settings.WorldSize;
            creature.ClimateHeightPercent = _gameData.Settings.ClimateHeightPercent;
            creature.InitNewCreature(_rand, ref _names, _gameData.NextSpeciesId, ref _creatureIdCtr, _gameData);
            creature.Species = "Cheater";
            creature.OriginalSpecies = "Cheater";
            creature.Position = new Vector2(200, 100);
            creature.Rotation = MathHelper.ToRadians(220);
            creature.ColdClimateTolerance = 0;
            creature.HotClimateTolerance = 0;
            creature.Sight = 15;
            creature.Scavenger = 10;
            creature.Speed = 30;
            creature.Lifespan = 100000;
            creature.Energy = 100000;
            creature.EggInterval = 500; //So it cannot lay any eggs
            creature.IsHerbavore = false;
            creature.IsScavenger = true;
            creature.GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);
            creature.Texture = DetermineCreatureTexture(creature);
            creature.FoodTypeBlue = 0;
            creature.FoodTypeRed = 0;
            creature.FoodTypeGreen = 0;
            _gameData.AddCreatureToGrid(creature);
            _gameData.Creatures.Add(creature);

            Vector2 foodPos = new Vector2(creature.Position.X + 15, creature.Position.Y - 100);
            SpawnFood(foodPos, 1);
            _gameData.Focus = creature;
            _gameData.FocusIndex = _gameData.Creatures.Count - 1;
        }
        private void SpawnOneTestPlant()
        {
            Plant plant = new Plant();
            plant.WorldSize = _gameData.Settings.WorldSize;
            plant.ClimateHeightPercent = _gameData.Settings.ClimateHeightPercent;
            plant.InitializeNewPlant(_rand, _textureList, _gameData);
            plant.Position = new Vector2(_gameData.Settings.WorldSize - plant.Texture.Width / 2, _gameData.Settings.WorldSize - plant.Texture.Height / 2);
            plant.GetGridPositionsForSpriteBase(GRID_CELL_SIZE, _gameData);
            plant.GetExpandedGridPositions(_gameData); //Calculate expanded grid positions after the position has been set
            plant.DisplayText = plant.FoodAmount.ToString() + "/" + plant.FoodAmountCap;
            plant.TextSize = _foodFont.MeasureString(plant.DisplayText);

            _gameData.Plants.Add(plant);
            _gameData.AddPlantToGrid(plant);
        }
    }

    public enum SettingEnum
    {
        WorldSize = 1,
        ClimatePercent = 2,
        StartingFoodRatio = 3,
        FoodGenerationValue = 4,
        SecondsUntilFoodUpgradesStart = 5,
        SecondsBetweenFoodUpgrades = 6,
        FoodUpgradeAmount = 7,
        FoodUpgradePercentChange = 8,
        MaxFoodlevel = 9,
        StartingCreatureRatio = 10,
        EnergyGivenFromFood = 11,
        EnergyLossFromLayingEgg = 12,
        EnergyDepletionFromMovementRate = 13,
        FoodType = 14,
        EnergyDepletionPercentFromComplexity = 15,
        StartingPlantRatio = 16,
        StartingEggIntervalMin = 101,
        StartingEggIntervalMax = 102,
        StartingEggIncubationMin = 103,
        StartingEggIncubationMax = 104,
        StartingFoodDigestionMin = 105,
        StartingFoodDigestionMax = 106,
        StartingSpeedMin = 107,
        StartingSpeedMax = 108,
        StartingLifespanMin = 109,
        StartingLifespanMax = 110,
        StartingHerbavoreLevelMin = 111,
        StartingHerbavoreLevelMax = 112,
        StartingCarnivoreLevelMin = 113,
        StartingCarnivoreLevelMax = 114,
        StartingScavengerLevelMin = 115,
        StartingScavengerLevelMax = 116,
        StartingOmnivoreLevelMin = 117,
        StartingOmnivoreLevelMax = 118,
        StartingColdToleranceMin = 119,
        StartingColdToleranceMax = 120,
        StartingHotToleranceMin = 121,
        StartingHotToleranceMax = 122,
        StartingEnergy = 123,
        ChanceToIncreaseValue = 201,
        ChangeAmount = 202,
        EggCamo = 203,
        EggIncubation = 204,
        EggInterval = 205,
        EggToxicity = 206,
        FoodDigestion = 207,
        Speed = 208,
        Lifespan = 209,
        Sight = 210,
        Attraction = 211,
        Camo = 212,
        Cloning = 213,
        ColdClimateTolerance = 214,
        HotClimateTolerance = 215,
        Herbavore = 216,
        Carnivore = 217,
        Omnivore = 218,
        Scavenger = 219,
        MutationBonusPercent = 220,
    }
}