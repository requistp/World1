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
public class VibrationSenseSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        Program.slashGame.EventManager.RegisterListener(GameEventEnum.LocationChanged, OnLocationChanged);
    }

    public void OnLocationChanged(GameEvent e)
    {
        var enm = Program.slashGame.EntityManager;
        var data = (LocationChangedData)e.EventData;

        if (!enm.TryGetComponent<FormComponent>(data.EntityID, out FormComponent formMover))
        {
            return; // Must have a form to have a location
        }

        foreach (int x in Program.slashGame.EntityManager.EntitiesWithComponent(typeof(VibrationSenseComponent)))
        {
            if (x != data.EntityID)
            {
                var listenerForm = enm.GetComponent<FormComponent>(x); // Form of the entity that can sense movement
                var listenerVibSense = enm.GetComponent<VibrationSenseComponent>(x); // VibrationSense of the entity that can sense movement

                var distance = UtilityFunctions.Distance2D(listenerForm.Location, formMover.Location);
                if (distance <= listenerVibSense.Range)
                {
                    Program.slashGame.EventManager.QueueEvent(GameEventEnum.VibrationSensed, new VibrationSenseEventData(x, data.EntityID, distance, formMover.Symbol, formMover.Location.GetAsInt));
                }
            }
        }
    }
}

public class VibrationSenseComponent : IEntityComponent
{
    public const string AttributeVibrationSenseRange = "VibrationSenseComponent.Range";

    public float Range { get; set; }

    public void InitComponent(IAttributeTable attributeTable)
    {
        Range = attributeTable.GetFloatOrDefault(AttributeVibrationSenseRange, 1);
    }

    public VibrationSenseComponent(float range)
    {
        Range = range;
    }
}

public class VibrationSenseEventData
{
    public int EntityID { get; }
    public int EntityID_Sensed { get; }
    public float Distance { get; }
    public LocationDataInt Location { get; }
    public string Symbol { get; }

    public VibrationSenseEventData(int entityID, int entityid_sensed, float distance, string symbol, LocationDataInt location)
    {
        EntityID = entityID;
        EntityID_Sensed = entityid_sensed;
        Distance = distance;
        Symbol = symbol;
        Location = location;
    }
}
