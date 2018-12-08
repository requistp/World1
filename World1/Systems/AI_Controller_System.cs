using System.Collections.Generic;
using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;
using System.Linq;
using PhUtilityAI;

[GameSystem]
public class AIControllerSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        Program.slashGame.EventManager.RegisterListener(GameEventEnum.Activity_NewSet_Explore, OnNewActivity_Explore);
        Program.slashGame.EventManager.RegisterListener(GameEventEnum.Memory_NewEvent_NonTerrainFound, OnMemory_NewEvent_NonTerrainFound);
    }

    public void OnMemory_NewEvent_NonTerrainFound(GameEvent e)
    {
        var enm = Program.slashGame.EntityManager;

        var data = (NewMemoryEventData)e.EventData;

        if (enm.TryGetComponent(data.EntityID, out AIControllerComponent aicc))
        {
            if (aicc.StopCondition.GetType() == typeof(PhUtilityStopCondition_GameEventEnum))
            {
                var stop = (PhUtilityStopCondition_GameEventEnum)aicc.StopCondition;

                if (!stop.Stopped && stop.GameEventTrigger == GameEventEnum.Memory_NewEvent_NonTerrainFound)
                {
                    stop.Finish();
                }
            }
        }
    }
    public void OnNewActivity_Explore(GameEvent e)
    {
        var enm = Program.slashGame.EntityManager;
        var data = (Event_NewActivity_Explore)e.EventData;
        
        if (enm.TryGetComponent(data.EntityID, out AIControllerComponent aicc) && !aicc.Exploring)
        {
            aicc.Get_ExploreMethod(data.EntityID);
            aicc.Exploring = true;
        }
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        var enm = Program.slashGame.EntityManager;

        foreach (var x in enm.EntitiesWithComponent(typeof(AIControllerComponent)))
        {
            var aicc = enm.GetComponent<AIControllerComponent>(x);

            if (aicc.Exploring && aicc.MoveOption != null && enm.TryGetComponent(x, out FormComponent form))
            {
                // If reached target location, reset target location
                if (form.Location.GetAsInt == aicc.MoveOption.LocationTarget)
                {
                    //System.Diagnostics.Debugger.Log(1, "test", "reached target, X,Y:" + aicc.MoveOption.LocationTarget.X.ToString() + "," + aicc.MoveOption.LocationTarget.Y.ToString() + "\n");
                    aicc.Get_ExploreMethod(x);
                    continue;
                }

                // Moved since last update?
                if (form.Location != aicc.MoveOption.LocationLastUpdate) // Yes, then continue moving in the same direction
                {
                    //System.Diagnostics.Debugger.Log(1, "test", ".");
                    aicc.MoveOption.LocationLastUpdate = form.Location.Clone();
                    Program.slashGame.EventManager.QueueEvent(GameEventEnum.Movement, new MovementEventData(x, aicc.MoveOption.Direction));
                    continue;
                }

                if (enm.TryGetComponent(x, out MemoryComponent memory)) // No, then try a new direction
                {
                    if (TerrainSystem.TryGet_Terrain(aicc.MoveOption.LocationTarget, out TerrainComponent terrainTarget, out FormComponent formTarget))
                    {
                        //System.Diagnostics.Debugger.Log(1, "test", "\nUpdate memory for location I can't get to\n");
                        memory.Add(terrainTarget.EntityID, 1f, MemoryTypeEnum.Presence, formTarget.Symbol, aicc.MoveOption.LocationTarget, false, false, false, 0, true);
                    }
                }
                //System.Diagnostics.Debugger.Log(1, "test", "\nTry another direction\n");
                aicc.Get_ExploreMethod(x);
            }
        }
    }

    public static void Explore_Roam_Random(int entityID) // Generates activity that reminds me of an ant or a rat -- moves randomly around a tight range (Ignores memory count)
    {
        var enm = Program.slashGame.EntityManager;

        if (enm.TryGetComponent(entityID, out AIControllerComponent aicc) && enm.TryGetComponent(entityID, out FormComponent form))
        {
            List<MovementOption> MoveOptions = GetMovementOptions(entityID, form, aicc.MaximumRangeFromBorn);

            SetMovementOption(aicc, form, MoveOptions);
        }
    }
    public static void Explore_Roam_Broad(int entityID)  // Generates activity that reminds me of a broad ranging animal, like a predator (Uses lowest memory count)
    {
        var enm = Program.slashGame.EntityManager;

        if (enm.TryGetComponent(entityID, out AIControllerComponent aicc) && enm.TryGetComponent(entityID, out FormComponent form))
        {
            List<MovementOption> MoveOptions = GetMovementOptions(entityID, form, aicc.MaximumRangeFromBorn);

            // Remove option not at the minimum MemoryHere count
            MoveOptions.RemoveAll(x => x.MemoryCount > MoveOptions.OrderBy(y => y.MemoryCount).First().MemoryCount);

            SetMovementOption(aicc, form, MoveOptions);
        }
    }
    public static void Explore_Roam_Narrow(int entityID) // Generates activity that reminds me of a cow or deer -- stays close to home (Uses highest memory count)
    {
        var enm = Program.slashGame.EntityManager;

        if (enm.TryGetComponent(entityID, out AIControllerComponent aicc) && enm.TryGetComponent(entityID, out FormComponent form))
        {
            List<MovementOption> MoveOptions = GetMovementOptions(entityID, form, aicc.MaximumRangeFromBorn);

            // Remove option not at the maximum MemoryHere count
            MoveOptions.RemoveAll(x => x.MemoryCount < MoveOptions.OrderByDescending(y => y.MemoryCount).First().MemoryCount);

            SetMovementOption(aicc, form, MoveOptions);
        }
    }

    private static List<MovementOption> GetMovementOptions(int entityID, FormComponent form, float maximumRangeFromBorn)
    {
        var enm = Program.slashGame.EntityManager;

        MovementOption moveOption = null;

        List<MovementOption> MoveOptions = new List<MovementOption>();

        var memory = enm.GetComponent<MemoryComponent>(entityID) ?? enm.GetComponent<HiveMemoryComponent>(entityID);
        if (memory != null)
        //if (enm.TryGetComponent(entityID, out MemoryComponent memory))
        {
            // Get memories of North, South, East, & West
            if (IsMoveOption(form, memory, MovementDirectionEnum.North, maximumRangeFromBorn, out moveOption)) MoveOptions.Add(moveOption);
            if (IsMoveOption(form, memory, MovementDirectionEnum.East, maximumRangeFromBorn, out moveOption)) MoveOptions.Add(moveOption);
            if (IsMoveOption(form, memory, MovementDirectionEnum.South, maximumRangeFromBorn, out moveOption)) MoveOptions.Add(moveOption);
            if (IsMoveOption(form, memory, MovementDirectionEnum.West, maximumRangeFromBorn, out moveOption)) MoveOptions.Add(moveOption);

            //If Options exist that aren't the last position, always take it
            if (MoveOptions.Exists(x => !x.IsLastPosition)) MoveOptions.RemoveAll(x => x.IsLastPosition);

            // Remove option not at the minimum BeenHere count
            MoveOptions.RemoveAll(x => x.BeenHereCount > MoveOptions.OrderBy(y => y.BeenHereCount).First().BeenHereCount);
        }

        return MoveOptions;
    }
    private static bool IsMoveOption(FormComponent form, MemoryComponent memory, MovementDirectionEnum direction, float maximumRangeFromBorn, out MovementOption moveOption)
    {
        moveOption = null;
        LocationDataInt locationTest = form.Location.GetAsInt;

        switch (direction)
        {
            case MovementDirectionEnum.North:
                locationTest.Y -= 1;
                break;
            case MovementDirectionEnum.East:
                locationTest.X += 1;
                break;
            case MovementDirectionEnum.South:
                locationTest.Y += 1;
                break;
            case MovementDirectionEnum.West:
                locationTest.X -= 1;
                break;
        }

        if (locationTest.X < 0 || locationTest.X > Program.MapWidth - 1 || locationTest.Y < 0 || locationTest.Y > Program.MapHeight - 1)
        {
            return false;
        }

        if (maximumRangeFromBorn != 0  && UtilityFunctions.Distance2D(locationTest, form.Location_Born) > maximumRangeFromBorn)
        {
            return false;
        }

        if (memory.TryGet_TerrainAtLocation(locationTest, out MemoryEvent me))
        {
            if (!(me.IsPassableTerrain ?? true))
            {
                return false;
            }
            else
            {
                moveOption = new MovementOption(me.BeenHereCount, me.MemoryCount, direction, (locationTest == memory.Location_Previous), locationTest, null);
                return true;
            }
        }
        else
        {
            moveOption = new MovementOption(0, 0, direction, false, locationTest, null);
            return true;
        }
    }
    private static void SetMovementOption(AIControllerComponent aicc, FormComponent form, List<MovementOption> MoveOptions)
    {
        MovementOption moveOption = null;

        if (MoveOptions != null && MoveOptions.Count() > 0) moveOption = MoveOptions[UtilityFunctions.Random(0, MoveOptions.Count() - 1)];

        aicc.MoveOption = moveOption;

        if (moveOption != null)
        {
            aicc.MoveOption.LocationLastUpdate = form.Location.Clone();
            Program.slashGame.EventManager.QueueEvent(GameEventEnum.Movement, new MovementEventData(aicc.EntityID, moveOption.Direction));
        }        
    }
}

public class AIControllerComponent : IEntityComponent
{
    public delegate void Explore_Method(int entityid);
    public Explore_Method Get_ExploreMethod { get; }

    public int EntityID { get; }

    public void Event_Activity_NewSet_Explore() => Program.slashGame.EventManager.QueueEvent(GameEventEnum.Activity_NewSet_Explore, new Event_NewActivity_Explore(EntityID));
    public bool Exploring { get; set; } = false;
    public bool Get_ExploreIsExecutable() => true;
    public float Get_ExploreXValue() => 1.0f;
    public float MaximumRangeFromBorn { get; set;  }
    public MovementOption MoveOption;
    public AbstractStopCondition StopCondition { get; set; }

    public void InitComponent(IAttributeTable attributeTable) { }

    public AIControllerComponent(int entityid, Explore_Method get_ExploreMethod, float duration, float maximumRangeFromBorn)
    {
        EntityID = entityid;
        Get_ExploreMethod = get_ExploreMethod;
        MaximumRangeFromBorn = maximumRangeFromBorn;

        if (Program.slashGame.EntityManager.TryGetComponent(entityid, out PhUtilityAIComponent aic))
        {
            //StopCondition = new PhUtilityStopCondition_Timer(duration, aic.EvaluateOptions);
            StopCondition = new PhUtilityStopCondition_GameEventEnum(GameEventEnum.Memory_NewEvent_NonTerrainFound, aic.EvaluateOptions);
            aic.AddActivityOption(ActivityOptionEnums.Explore, null, true, true, Get_ExploreXValue, Get_ExploreIsExecutable, Event_Activity_NewSet_Explore, true, StopCondition);
        }
    }
}

public class MovementOption
{
    public MovementDirectionEnum Direction { get; }
    public int BeenHereCount { get; }
    public int MemoryCount { get; }
    public bool IsLastPosition { get; }
    public LocationDataInt LocationTarget { get; }
    public LocationDataFloat LocationLastUpdate { get; set; }

    public MovementOption(int beenHereCount, int memoryCount, MovementDirectionEnum direction, bool isLastPosition, LocationDataInt locationTarget, LocationDataFloat locationLastUpdate)
    {
        BeenHereCount = beenHereCount;
        Direction = direction;
        IsLastPosition = isLastPosition;
        LocationTarget = locationTarget;
        LocationLastUpdate = locationLastUpdate;
        MemoryCount = memoryCount;
    }
}

public class Event_NewActivity_Explore
{
    public int EntityID { get; }

    public Event_NewActivity_Explore(int entityid)
    {
        EntityID = entityid;
    }
}

