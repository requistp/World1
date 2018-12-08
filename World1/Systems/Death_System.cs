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
public class DeathSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        Program.slashGame.EventManager.RegisterListener(GameEventEnum.Death, this.OnDeath);
    }

    public void OnDeath(GameEvent e)
    {
        var data = (DeathEventData)e.EventData;

        Program.slashGame.EntityManager.RemoveEntity(data.EntityID);
    }
}

public class DeathEventData
{
    public int EntityID;
    public int AttackerID;

    public DeathEventData(int entityid, int attackerid)
    {
        EntityID = entityid;
        AttackerID = attackerid;
    }
}
