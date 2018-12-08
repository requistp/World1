using System;
using System.Collections.Generic;
using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;
using PhUtilityAI;

[GameSystem]
public class EatingSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        Program.slashGame.EventManager.RegisterListener(GameEventEnum.Activity_NewSet_Eat, OnNewActivity_Eat);
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        var em = Program.slashGame.EntityManager;

        // Update hunger level
        foreach (var x in em.EntitiesWithComponent(typeof(EatingComponent)))
        {
            var eat = em.GetComponent<EatingComponent>(x);
                
            eat.HungerLevel = (eat.HungerLevel + (eat.HungerRateOfIncrease * dt)).Clamp(0f, 100f);
        }
    }

    public void OnNewActivity_Eat(GameEvent e)
    {
        var enm = Program.slashGame.EntityManager;
        var data = (Event_NewActivity_Eat)e.EventData;

        System.Diagnostics.Debugger.Log(1, "test", "Handle Eat\n");

        var eat = enm.GetComponent<EatingComponent>(data.EntityID);

        //if (eat.FoodInRange)
        //{
        //    System.Diagnostics.Debugger.Log(1, "test", "   Begin eating\n");
        //}
        //else
        //{
        //    System.Diagnostics.Debugger.Log(1, "test", "   Shouldn't be getting here because I should revert to exploring\n");
        //}
    }
}

public class EatingComponent : IEntityComponent
{
    public int EntityID { get; }
    //public bool FoodInRange { get; set; } = false;
    public float HungerLevel { get; set; }
    public float Range { get; }
    public float HungerRateOfIncrease { get; set; }
    public List<FoodTypeEnum> EdibleFoods { get; set; }

    public bool Get_FoodInMemory()
    {
        if (Program.slashGame.EntityManager.TryGetComponent(EntityID, out MemoryComponent memory))
        {
            //return memory.Memories.
        }

        return false;
    }
    public float Get_HungerLevel() => HungerLevel;
    public void Event_Activity_NewSet_Eat() => Program.slashGame.EventManager.QueueEvent(GameEventEnum.Activity_NewSet_Eat, new Event_NewActivity_Eat(EntityID));

    public void InitComponent(IAttributeTable attributeTable) { }

    public EatingComponent(int entityid, float hungerLevel, float hungerRateOfIncrease, float range, List<FoodTypeEnum> edibleFoods)
    {
        EntityID = entityid;
        HungerLevel = hungerLevel;
        EdibleFoods = edibleFoods;
        Range = range;
        HungerRateOfIncrease = hungerRateOfIncrease;

        if (Program.slashGame.EntityManager.TryGetComponent(entityid, out PhUtilityAIComponent aic))
        {
            aic.AddActivityOption(ActivityOptionEnums.Eat, ActivityOptionEnums.Explore, true, false, Get_HungerLevel, Get_FoodInMemory, Event_Activity_NewSet_Eat, true, new PhUtilityStopCondition_Timer(10f, aic.EvaluateOptions));
        }
    }
}

public class Event_NewActivity_Eat
{
    public int EntityID { get; }

    public Event_NewActivity_Eat(int entityid)
    {
        EntityID = entityid;
    }
}



//Program.slashGame.EventManager.RegisterListener(GameEventEnum.Movement, OnMovement);
//public void OnMovement(GameEvent e)
//{
//    var enm = Program.slashGame.EntityManager;
//    var data = (MovementEventData)e.EventData;

//    // If I can eat, then update my eating options
//    if (enm.TryGetComponent(data.EntityID, out EatingComponent eat))
//    {
//        bool newFoodInRange = false;
//        if (enm.TryGetComponent(data.EntityID, out MemoryComponent memory))
//        {
//            foreach (var mem in memory.Memories.Filter(eat.Range))
//            {
//                if (enm.TryGetComponent(mem.EntityID, out FoodComponent food) && eat.EdibleFoods.Contains(food.FoodType))
//                {
//                    newFoodInRange = true;
//                    break;
//                }
//            }
//        }
//        eat.FoodInRange = newFoodInRange;
//    }

//    // Can something nearby eat me?
//    // Need to add logic
//}

