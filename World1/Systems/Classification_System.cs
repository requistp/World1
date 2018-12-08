//using System.Collections.Generic;
//using Slash.Application.Systems;
//using Slash.Collections.AttributeTables;
//using Slash.ECS.Components;
//using Slash.ECS.Events;

//[GameSystem]
//public class ClassificationSystem : GameSystem
//{
//    public override void Init(IAttributeTable configuration)
//    {
//        base.Init(configuration);

//        Program.slashGame.EventManager.RegisterListener(GameEventEnum.EntityDetected, OnEntityDetected);
//    }

//    public void OnEntityDetected(GameEvent e)
//    {
//        //var slashGame = Program.slashGame;
//        //var data = (EntityDetectedData)e.EventData;

//        //var formDetected = slashGame.EntityManager.GetComponent<FormComponent>(data.EntityID_Detected); // Entity2Data that moved
        
//        //System.Diagnostics.Debugger.Log(1, "test", "classify new entity\n");
//    }
//}

//public class ClassificationComponent : IEntityComponent
//{
//    public List<ClassificationStatus> Classifications;

//    public void InitComponent(IAttributeTable attributeTable)
//    {
//    }

//    public ClassificationComponent()
//    {
//        Classifications = new List<ClassificationStatus>();
//    }
//}

//public class ClassificationStatus
//{
//    public int EntityID;
//    public ClassificationEnum Classification;
//}

