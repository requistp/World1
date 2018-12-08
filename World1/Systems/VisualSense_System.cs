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
public class VisualSenseSystem : GameSystem
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

        UpdateVisualAwareness(data.EntityID);
    }

    private void UpdateVisualAwareness(int entityid)
    {
        var enm = Program.slashGame.EntityManager;

        if (!enm.TryGetComponent<FormComponent>(entityid, out FormComponent formMover))
        {
            return; // Must have a form to have a location
        }

        // Did the mover see anything?
        if (enm.TryGetComponent<VisualSenseComponent>(entityid, out VisualSenseComponent vis))
        {
            foreach (var x in enm.EntitiesWithComponent(typeof(FormComponent)))
            {
                if (x != entityid)
                {
                    var formOther = enm.GetComponent<FormComponent>(x); // Form of the entity that can be seen

                    var distance = UtilityFunctions.Distance2D(formMover.Location, formOther.Location);
                    if (distance <= vis.Range)
                    {
                        Program.slashGame.EventManager.QueueEvent(GameEventEnum.VisuallySensed, new VisuallySensedEventData(entityid, x, distance, formOther.Symbol, formOther.Location.GetAsInt));
                    }
                }
            }
        }

        // Did anything see the mover?
        foreach (int x in enm.EntitiesWithComponent(typeof(VisualSenseComponent)))
        {
            if (x != entityid)
            {
                if (enm.TryGetComponent<FormComponent>(x, out FormComponent formOther))
                {
                    var visOther = enm.GetComponent<VisualSenseComponent>(x);

                    var distance = UtilityFunctions.Distance2D(formMover.Location, formOther.Location);
                    if (distance <= visOther.Range)
                    {
                        Program.slashGame.EventManager.QueueEvent(GameEventEnum.VisuallySensed, new VisuallySensedEventData(x, entityid, distance, formMover.Symbol, formMover.Location.GetAsInt));
                    }
                }
            }
        }
    }
}

public class VisualSenseComponent : IEntityComponent
{
    public const string AttributeVisualSenseRange = "VisualSenseComponent.Range";
    public const float MaximumVisualRange = 30f;

    public float Range { get; set; }

    public void InitComponent(IAttributeTable attributeTable)
    {
        Range = attributeTable.GetFloatOrDefault(AttributeVisualSenseRange, 1);
    }

    public VisualSenseComponent(float range)
    {
        Range = range;
    }
}

public class VisuallySensedEventData
{
    public int EntityID { get; }
    public int EntityID_Sensed { get; }
    public float Distance { get; }
    public string Symbol { get; }
    public LocationDataInt Location { get; }

    public VisuallySensedEventData(int entityID, int entityid_sensed, float distance, string symbol, LocationDataInt location)
    {
        EntityID = entityID;
        EntityID_Sensed = entityid_sensed;
        Distance = distance;
        Symbol = symbol;
        Location = location;
    }
}
