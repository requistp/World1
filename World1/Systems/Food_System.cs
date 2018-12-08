using System.Collections.Generic;
using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;

[GameSystem]
public class FoodSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        //Program.slashGame.EventManager.RegisterListener(GameEventEnum.EntityDetected, OnEntityDetected);
    }

    //public void OnEntityDetected(GameEvent e)
    //{
    //    var slashGame = Program.slashGame;
    //    var data = (EntityDetectedData)e.EventData;

    //    var formDetected = slashGame.EntityManager.GetComponent<FormComponent>(data.EntityID_Detected); // Entity2Data that moved

    //    //System.Diagnostics.Debugger.Log(1, "test", "classify new entity\n");
    //}
}

public class FoodComponent : IEntityComponent
{
    public FoodTypeEnum FoodType;
    public int Quantity;

    public string Symbol
    {
        get
        {
            switch (FoodType)
            {
                case FoodTypeEnum.Carrot:
                    return "c";
                case FoodTypeEnum.Grass:
                    return "g";
                default:
                    return "f";
            }
        }
    }

    public void InitComponent(IAttributeTable attributeTable)
    {
    }

    public FoodComponent(FoodTypeEnum foodType, int quantity)
    {
        FoodType = foodType;
        Quantity = quantity;
    }
}

public enum FoodTypeEnum
{
    Carrot,
    Grass
}

//public class VibrationSenseEventData
//{
//    public int EntityID;
//    public int EntityID_Sensed;

//    public VibrationSenseEventData(int entityID, int entityid_sensed)
//    {
//        EntityID = entityID;
//        EntityID_Sensed = entityid_sensed;
//    }
//}
