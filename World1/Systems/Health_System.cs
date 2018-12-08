using System.Collections.Generic;
using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;

[GameSystem]
public class HealthSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        Program.slashGame.EventManager.RegisterListener(GameEventEnum.HealthChange, OnHealthChange);
    }

    public void OnHealthChange(GameEvent e)
    {
        var slashGame = Program.slashGame;

        var data = (HealthChangeEventData)e.EventData;

        var health = slashGame.EntityManager.GetComponent<HealthComponent>(data.EntityID);

        health.CurrentHP += data.ChangeHP;

        if (health.CurrentHP > health.MaxHP)
        {
            health.CurrentHP = health.MaxHP;
        }
        else if (health.CurrentHP <= 0)
        {
            health.CurrentHP = 0;
            slashGame.EventManager.QueueEvent(GameEventEnum.Death, new DeathEventData(data.EntityID, 0));
        }
        System.Diagnostics.Debugger.Log(1, "HealthSystem", health.CurrentHP.ToString() + "\n");
    }
}

public class HealthComponent : IEntityComponent
{
    public const string AttributeCurrentHP = "MovementComponent.CurrentHP";
    public const string AttributeMaxHP = "MovementComponent.MaxHP";

    public int CurrentHP { get; set; }
    public int MaxHP { get; set; }

    public bool IsAlive
    {
        get
        {
            return (CurrentHP > 0);
        }
    }

    public void InitComponent(IAttributeTable attributeTable)
    {
        CurrentHP = attributeTable.GetIntOrDefault(AttributeCurrentHP, 1);
        MaxHP = attributeTable.GetIntOrDefault(AttributeMaxHP, 1);
    }

    public HealthComponent(int currenthp, int maxhp)
    {
        CurrentHP = currenthp;
        MaxHP = maxhp;
    }
}

public class HealthChangeEventData
{
    public int EntityID;
    public int ChangeHP;

    public HealthChangeEventData(int entityid, int changehp)
    {
        EntityID = entityid;
        ChangeHP = changehp;
    }
}
