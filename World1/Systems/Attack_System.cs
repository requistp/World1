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
public class AttackSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        Program.slashGame.EventManager.RegisterListener(GameEventEnum.Attack, this.OnAttack);
    }

    public void OnAttack(GameEvent e)
    {
        var slashGame = Program.slashGame;

        var data = (AttackEventData)e.EventData;

        var attack = slashGame.EntityManager.GetComponent<AttackComponent>(data.EntityID);
        if (attack == null)
        {
            return;
        }

        var formAttacker = slashGame.EntityManager.GetComponent<FormComponent>(data.EntityID);

        var attackLocation = new LocationDataFloat((int)formAttacker.Location.X, (int)formAttacker.Location.Y);

        if (data.Direction == MovementDirectionEnum.North)
        {
            attackLocation.Y -= 1;
        }
        else if (data.Direction == MovementDirectionEnum.East)
        {
            attackLocation.X += 1;
        }
        else if (data.Direction == MovementDirectionEnum.South)
        {
            attackLocation.Y += 1;
        }
        else if (data.Direction == MovementDirectionEnum.West)
        {
            attackLocation.X -= 1;
        }

        ExecuteAttack(attackLocation, -attack.Damage);
    }

    private void ExecuteAttack(LocationDataFloat location, int damage)
    {
        var slashGame = Program.slashGame;

        foreach (int x in slashGame.EntityManager.EntitiesWithComponent(typeof(FormComponent)))
        {
            var form = slashGame.EntityManager.GetComponent<FormComponent>(x);

            if (form.Location.X == location.X && form.Location.Y == location.Y)
            {
                var health = Program.slashGame.EntityManager.GetComponent<HealthComponent>(x);

                if (health != null)
                {
                    slashGame.EventManager.QueueEvent(GameEventEnum.HealthChange, new HealthChangeEventData(x, damage));
                    break;
                }
            }
        }
    }
}

public class AttackComponent : IEntityComponent
{
    public const string AttributeDamage = "AttackComponent.Damage";

    public int Damage { get; set; }

    public void InitComponent(IAttributeTable attributeTable)
    {
        Damage = attributeTable.GetIntOrDefault(AttributeDamage, 1);
    }

    public AttackComponent(int damage)
    {
        Damage = damage;
    }
}

public class AttackEventData
{
    public int EntityID;
    public MovementDirectionEnum Direction;

    public AttackEventData(int entityid, MovementDirectionEnum direction)
    {
        EntityID = entityid;
        Direction = direction;
    }
}

//public enum AttackDirectionEnum
//{
    
//    North,
//    East,
//    South,
//    West
//}
