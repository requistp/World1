using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;

[GameSystem]
public class MemorySystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        Program.slashGame.EventManager.RegisterListener(GameEventEnum.Component_New_Memory, OnComponent_New_Memory);
        Program.slashGame.EventManager.RegisterListener(GameEventEnum.LocationChanged, OnMovementCompleted_RememberTerrain);
        Program.slashGame.EventManager.RegisterListener(GameEventEnum.VibrationSensed, OnVibrationSensed);
        Program.slashGame.EventManager.RegisterListener(GameEventEnum.VisuallySensed, OnVisuallySensed);
    }

    public void OnComponent_New_Memory(GameEvent e)
    {
        var slashGame = Program.slashGame;
        var data = (MemoryComponent)e.EventData;

        var c = new SadConsole.Console(Program.MapWidth, Program.MapHeight) { Position = new Point(Program.MapWidth, 0) };
        SadConsole.Global.CurrentScreen.Children.Add(c);
        data.Display = c;
    }
    public void OnMovementCompleted_RememberTerrain(GameEvent e)
    {
        // This step is purely to update my classification of the terrain I'm at (standing on)

        var enm = Program.slashGame.EntityManager;
        var data = (LocationChangedData)e.EventData;

        if (enm.TryGetComponent(data.EntityID, out MemoryComponent memory) && TerrainSystem.TryGet_Terrain(data.Location_New, out TerrainComponent terrain, out FormComponent form))
        {
            int beenHereCount = 1;
            if (memory.Exists(terrain.EntityID)) { beenHereCount += memory[terrain.EntityID].BeenHereCount; }
            memory.Add(terrain.EntityID, 0, MemoryTypeEnum.Presence, form.Symbol, data.Location_New, false, false, true, beenHereCount, true);
        }
    }
    public void OnVibrationSensed(GameEvent e)
    {
        var data = (VibrationSenseEventData)e.EventData;
        
        NewMemory(data.EntityID, data.EntityID_Sensed, data.Distance, MemoryTypeEnum.Vibration, true, false);
    }
    public void OnVisuallySensed(GameEvent e)
    {
        var data = (VisuallySensedEventData)e.EventData;
        
        NewMemory(data.EntityID, data.EntityID_Sensed, data.Distance, MemoryTypeEnum.Visual, true, false);
    }

    private void NewMemory(int entityid, int entityid_InMemory, float distance, MemoryTypeEnum memoryType, bool? moves, bool hereNow)
    {
        var enm = Program.slashGame.EntityManager;

        if (enm.TryGetComponent(entityid, out MemoryComponent memory) && enm.TryGetComponent(entityid_InMemory, out FormComponent formMemory))
        {
            int BeenHereCount = 0;
            bool? Edible = null;
            bool? IsPassableTerrain = false;
            bool IsTerrain = false;

            if (!ClassifyMemory_Self(memoryType, out BeenHereCount, out Edible, out IsPassableTerrain, out IsTerrain))
            {
                if (!ClassifyMemory_Terrain(memory, entityid_InMemory, hereNow, out BeenHereCount, out Edible, out IsPassableTerrain, out IsTerrain))
                {

                    //if (formMemory.Symbol == "g") { System.Diagnostics.Debugger.Log(1, "test", "g!\n"); }

                    //System.Diagnostics.Debugger.Log(1, "test", "Missing classify: " + formMemory.Symbol + "\n");

                }                
            }

            memory.Add(entityid_InMemory, distance, memoryType, formMemory.Symbol, formMemory.Location.GetAsInt, Edible, moves, IsPassableTerrain, BeenHereCount, IsTerrain);
        }        
    }

    private bool ClassifyMemory_Self(MemoryTypeEnum memoryType, out int BeenHereCount, out bool? Edible, out bool? IsPassableTerrain, out bool IsTerrain)
    {
        if (memoryType == MemoryTypeEnum.SelfReflection)
        {
            BeenHereCount = 0;
            Edible = false;
            IsPassableTerrain = false;
            IsTerrain = false;
            return true;
        }

        BeenHereCount = 0;
        Edible = null;
        IsPassableTerrain = false;
        IsTerrain = false;
        return false;
    }
    private bool ClassifyMemory_Terrain(MemoryComponent memory, int entityID_InMemory, bool hereNow, out int BeenHereCount, out bool? Edible, out bool? IsPassableTerrain, out bool IsTerrain)
    {
        var enm = Program.slashGame.EntityManager;

        if (enm.TryGetComponent(entityID_InMemory, out TerrainComponent terrain))
        {
            MemoryEvent oldMemoryEvent = null;
            if (memory.Exists(entityID_InMemory)) oldMemoryEvent = memory[entityID_InMemory];
            BeenHereCount = (oldMemoryEvent?.BeenHereCount ?? 0) + (hereNow ? 1 : 0);

            Edible = false;
            IsPassableTerrain = oldMemoryEvent?.IsPassableTerrain;
            IsTerrain = true;
            return true;
        }

        BeenHereCount = 0;
        Edible = null;
        IsPassableTerrain = false;
        IsTerrain = false;
        return false;
    }

    public override void LateUpdate(float dt)
    {
        base.LateUpdate(dt);

        var enm = Program.slashGame.EntityManager;

        foreach (var x in enm.EntitiesWithComponent(typeof(MemoryComponent)))
        {
            var memory = enm.GetComponent<MemoryComponent>(x);
            if (memory.DisplayActive && memory.Display != null)
            {
                UpdateMemoryDisplay(x, memory);
                break;
            }
        }
    }

    public void UpdateMemoryDisplay(int entityid, MemoryComponent memory)
    {
        var slashGame = Program.slashGame;

        memory.Display.Clear();

        foreach (var m in memory.EventDictionary.Memories.Values)
        {
            memory.Display.Print(m.Location.X, m.Location.Y, m.Symbol);
        }

        // Display Self Last
        if (slashGame.EntityManager.TryGetComponent<FormComponent>(entityid, out FormComponent form) && form != null)
        {
            memory.Display.Print((int)Math.Round(form.Location.X, 0), (int)Math.Round(form.Location.Y, 0), form.Symbol);
        }
    }
}

public class MemoryComponent : IEntityComponent
{
    public SadConsole.Console Display { get; set; }
    public bool DisplayActive { get; set; }
    public LocationDataInt Location_Previous { get; protected set; }
    public virtual MemoryEvents EventDictionary { get; }
    public virtual MemoryEvent this[int i] => EventDictionary.Memories[i];
    public virtual bool Exists(int i) => EventDictionary.Exists(i);
    
    public virtual bool TryGet_TerrainAtLocation(LocationDataInt location, out MemoryEvent me)
    {
        me = EventDictionary.TerrainAtLocation(location);
        return (me != null);
    }

    public virtual void InitComponent(IAttributeTable attributeTable) { }

    protected MemoryComponent() { }

    public MemoryComponent(int entityid, bool displayActive, string symbol, LocationDataInt location)
    {
        DisplayActive = displayActive;
        Location_Previous = location;
        EventDictionary = new MemoryEvents(entityid);
        EventDictionary.Add(entityid, 0, MemoryTypeEnum.SelfReflection, symbol, Location_Previous, false, true, false, 0, false);
        Program.slashGame.EventManager.QueueEvent(GameEventEnum.Component_New_Memory, this);
    }

    public virtual void Add(int entityid, float distance, MemoryTypeEnum memoryType, string symbol, LocationDataInt location, bool? edible, bool? moves, bool? passableTerrain, int beenHereCount, bool terrain) => EventDictionary.Add(entityid, distance, memoryType, symbol, location, edible, moves, passableTerrain, beenHereCount, terrain);
}

public class HiveMemoryComponent : MemoryComponent
{
    private static readonly Dictionary<int, MemoryEvents> HiveDictionaries = new Dictionary<int, MemoryEvents>();

    public int HiveID { get; }

    public override MemoryEvents EventDictionary => HiveDictionaries[HiveID];
    public override MemoryEvent this[int i] => EventDictionary.Memories[i];
    public override bool Exists(int i) => EventDictionary.Exists(i);

    public override bool TryGet_TerrainAtLocation(LocationDataInt location, out MemoryEvent me)
    {
        me = EventDictionary.TerrainAtLocation(location);
        return (me != null);
    }

    public override void InitComponent(IAttributeTable attributeTable) { }

    public HiveMemoryComponent(int entityid, int hiveID, bool displayActive, string symbol, LocationDataInt location)
    {
        HiveID = hiveID;
        base.DisplayActive = displayActive;
        base.Location_Previous = location;

        if (!HiveDictionaries.ContainsKey(HiveID))
        {
            HiveDictionaries.Add(HiveID, new MemoryEvents(entityid));
        }

        EventDictionary.Add(entityid, 0, MemoryTypeEnum.SelfReflection, symbol, Location_Previous, false, true, false, 0, false);

        Program.slashGame.EventManager.QueueEvent(GameEventEnum.Component_New_Memory, this);
    }

    public override void Add(int entityid, float distance, MemoryTypeEnum memoryType, string symbol, LocationDataInt location, bool? edible, bool? moves, bool? passableTerrain, int beenHereCount, bool terrain) => EventDictionary.Add(entityid, distance, memoryType, symbol, location, edible, moves, passableTerrain, beenHereCount, terrain);
}

public class MemoryEvents
{
    private int EntityID { get; }

    public Dictionary<int, MemoryEvent> Memories;

    public MemoryEvent TerrainAtLocation(LocationDataInt location) => Memories.Values.FirstOrDefault(x => x.Location == location && x.IsTerrain);

    public bool Exists(int i) => Memories.ContainsKey(i);

    public void Add(int entityid, float distance, MemoryTypeEnum memoryType, string symbol, LocationDataInt location, bool? edible, bool? moves, bool? passableTerrain, int beenHereCount, bool terrain)
    {
        var newMemoryEvent = new MemoryEvent(entityid, distance, location, memoryType, symbol, edible, moves, passableTerrain, beenHereCount, terrain, 1);

        if (!Memories.TryGetValue(entityid, out MemoryEvent memoryEvent))
        {
            Memories.Add(entityid, newMemoryEvent);

            if (!newMemoryEvent.IsTerrain)
            {
                Program.slashGame.EventManager.QueueEvent(GameEventEnum.Memory_NewEvent_NonTerrainFound, new NewMemoryEventData(EntityID, newMemoryEvent));
            }
        }
        else
        {
            newMemoryEvent.MemoryCount += memoryEvent.MemoryCount;
            Memories[entityid] = newMemoryEvent;
        }
    }

    public MemoryEvents(int entityID)
    {
        EntityID = entityID;
        Memories = new Dictionary<int, MemoryEvent>();
    }
}

public class MemoryEvent
{
    public int EntityID { get; }
    public float Distance { get; }
    public LocationDataInt Location { get; }
    public MemoryTypeEnum MemoryType { get; }
    public string Symbol { get; }
    public float Time { get; }

    public int BeenHereCount { get; }       // How many times have I been here?
    public bool? IsEdible { get; }          // Can I eat this?
    public bool? ThingMoves { get; }        // Does this thing move?
    public bool? IsPassableTerrain { get; } // Can I walk here?
    public bool IsTerrain { get; }          // Is this a terrain element?
    public int MemoryCount { get; set; }    // How many times have I seen here?

    public MemoryEvent(int entityid, float distance, LocationDataInt location, MemoryTypeEnum memoryType, string symbol, bool? edible, bool? moves, bool? passableTerrain, int beenHereCount, bool terrain, int memoryCount)
    {
        EntityID = entityid;
        Distance = distance;
        Location = location;
        Time = Program.slashGame.TimeElapsed;
        MemoryType = memoryType;
        Symbol = symbol;

        BeenHereCount = beenHereCount;
        IsEdible = edible;
        ThingMoves = moves;
        IsPassableTerrain = passableTerrain;
        IsTerrain = terrain;
        MemoryCount = memoryCount;
    }
}

public enum MemoryTypeEnum
{
    Presence,       // i.e., I'm physically standing there
    SelfReflection,
    Vibration,
    Visual
}

public class NewMemoryEventData
{
    public int EntityID { get; }
    public MemoryEvent MemoryEvent { get; }

    public NewMemoryEventData(int entityID, MemoryEvent memoryEvent)
    {
        EntityID = entityID;
        MemoryEvent = memoryEvent;
    }
}