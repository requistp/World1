//System.Diagnostics.Debugger.Log(1, "test", "looping\n");
using System;
using System.Diagnostics;
using SadConsole;
//using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using PhUtilityAI;
using System.Collections.Generic;

class Program
{
    public const int MapWidth = 60;
    public const int MapHeight = 30;

    public static Slash.Application.Games.Game slashGame = new Slash.Application.Games.Game();

    static void Main(string[] args)
    {
        // Setup the engine and creat the main window.
        SadConsole.Game.Create("IBM.font", MapWidth * 2, MapHeight * 2);

        // Hook the start event so we can add consoles to the system.
        SadConsole.Game.OnInitialize = Init;

        // Hook the update event that happens each frame so we can trap keys and respond.
        SadConsole.Game.OnUpdate = Update;
        
        LoadTerrain();

        LoadFood();

        LoadEntities();
            
        slashGame.StartGame();

        // Start the game.
        SadConsole.Game.Instance.Run();
        
        // Code here will not run until the game window closes.

        SadConsole.Game.Instance.Dispose();
    }

    private static void LoadEntities()
    {
        int entityid;

        //Player
        //entityid = slashGame.EntityManager.CreateEntity();
        //slashGame.EntityManager.AddComponent(entityid, new FormComponent("@", new LocationDataFloat(10, 7), false));
        //slashGame.EntityManager.AddComponent(entityid, new MovementComponent(4));
        //slashGame.EntityManager.AddComponent(entityid, new HealthComponent(10, 10));
        //slashGame.EntityManager.AddComponent(entityid, new AttackComponent(1));
        //slashGame.EntityManager.AddComponent(entityid, new ControllerComponent());
        //slashGame.EntityManager.AddComponent(entityid, new MemoryComponent(entityid));

        // Animal 1

        MakeDeer(20, 10, true);
        MakeDeer(40, 20, false);
    }

    private static void MakeDeer(int startX, int startY, bool displayActive)
    {
        var entityid = slashGame.EntityManager.CreateEntity();
        var form = new FormComponent("x", new LocationDataFloat(startX, startY), false, "Deer (" + entityid.ToString() + ")");
        slashGame.EntityManager.AddComponent(entityid, form);
        //slashGame.EntityManager.AddComponent(entityid, new MemoryComponent(entityid, displayActive, "x", form.Location.GetAsInt));
        slashGame.EntityManager.AddComponent(entityid, new HiveMemoryComponent(entityid, 1, displayActive, "x", form.Location.GetAsInt));
        slashGame.EntityManager.AddComponent(entityid, new PhUtilityAIComponent(entityid));
        slashGame.EntityManager.AddComponent(entityid, new MovementComponent(entityid, 8, form.Location.GetAsInt));

        slashGame.EntityManager.AddComponent(entityid, new HealthComponent(10, 10));
        //slashGame.EntityManager.AddComponent(entityid, new VibrationSenseComponent(2));
        slashGame.EntityManager.AddComponent(entityid, new VisualSenseComponent(2.0f));
        slashGame.EntityManager.AddComponent(entityid, new EatingComponent(entityid, 0f, 1f, 1.75f, new List<FoodTypeEnum>() { FoodTypeEnum.Grass }));
        //slashGame.EntityManager.AddComponent(entityid, new ControllerComponent());
        //slashGame.EntityManager.AddComponent(entityid, new AIControllerComponent(entityid, AIControllerSystem.Explore_Roam_Broad, 5.0f, 0f));
    }

    private static void LoadTerrain()
    {
        int entityid;

        for (int x = 0; x <= MapWidth - 1; x++) 
        {
            for (int y = 0; y <= MapHeight - 1; y++) 
            {
                entityid = slashGame.EntityManager.CreateEntity();
                var terrain = new TerrainComponent(entityid, (x == 0 || y == 0 || x == MapWidth - 1 || y == MapHeight - 1) ? TerrainTypeEnum.Rock : TerrainTypeEnum.Dirt);
                slashGame.EntityManager.AddComponent(entityid, terrain);
                slashGame.EntityManager.AddComponent(entityid, new FormComponent(TerrainSystem.Symbol(terrain.TerrainType), new LocationDataFloat(x, y), !TerrainSystem.IsSolid(terrain.TerrainType)));
            }
        }
    }

    private static void LoadFood()
    {
        int entityid = 0;

        for (int i = 0; i <= 10; i++)
        {
            int x = UtilityFunctions.Random(entityid, 1, MapWidth - 2);
            int y = UtilityFunctions.Random((int)Math.Tan(entityid), 1, MapHeight - 2);
            
            entityid = slashGame.EntityManager.CreateEntity();
            var food = new FoodComponent(FoodTypeEnum.Grass, 10);
            slashGame.EntityManager.AddComponent(entityid, food);
            slashGame.EntityManager.AddComponent(entityid, new FormComponent(food.Symbol, new LocationDataFloat(x, y), true));
        }
    }

    private static void Update(GameTime time)
    {
        GetUserInput();
        
        slashGame.Update((float)time.ElapsedGameTime.Milliseconds/1000f);
    }

    private static void GetUserInput()
    {
        slashGame.EventManager.QueueEvent(GameEventEnum.ControllerInput, new ControllerEventData(SadConsole.Global.KeyboardState));
    }

    private static void Init()
    {
    }
}

