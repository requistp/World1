
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
public class MovementSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        Program.slashGame.EventManager.RegisterListener(GameEventEnum.Movement, OnMovement);

        InitializeLocationsForAllEntities();
    }

    private void InitializeLocationsForAllEntities()
    {
        var enm = Program.slashGame.EntityManager;

        foreach (var x in enm.EntitiesWithComponent(typeof(MovementComponent)))
        {
            if (enm.TryGetComponent(x, out FormComponent form))
            {
                Program.slashGame.EventManager.QueueEvent(GameEventEnum.LocationChanged, new LocationChangedData(x, form.Location.GetAsInt, form.Location.GetAsInt));
            }
        }        
    }
    public override void Update(float dt)
    {
        base.Update(dt);

        //Additional...
    }

    public override void LateUpdate(float dt)
    {
        base.LateUpdate(dt);

        var enm = Program.slashGame.EntityManager;

        foreach (int x in enm.EntitiesWithComponent(typeof(MovementComponent)))
        {
            var move = enm.GetComponent<MovementComponent>(x);

            if (move.MovementDirection != MovementDirectionEnum.Stopped)
            {
                var form = enm.GetComponent<FormComponent>(x);

                var startLocationInt = form.Location.GetAsInt; // Used to see if the entity integer-moved

                var effectiveSpeed = move.Speed_Base * dt;

                var newLocation = new LocationDataFloat(form.Location.X, form.Location.Y);

                if (move.MovementDirection == MovementDirectionEnum.North) newLocation.Y -= effectiveSpeed;
                else if (move.MovementDirection == MovementDirectionEnum.East) newLocation.X += effectiveSpeed;
                else if (move.MovementDirection == MovementDirectionEnum.South) newLocation.Y += effectiveSpeed;
                else if (move.MovementDirection == MovementDirectionEnum.West) newLocation.X -= effectiveSpeed;

                newLocation.X = newLocation.X.Clamp(0f, Program.MapWidth - 1);
                newLocation.Y = newLocation.Y.Clamp(0f, Program.MapHeight - 1);

                if (TerrainSystem.IsPassableTerrain(newLocation.GetAsInt)) //Need to add check for other entities being at lcoation
                {
                    form.Location = newLocation;
                    form.Display.Position = form.Location.GetAsPoint;

                    if (form.Location.GetAsInt != startLocationInt)
                    {
                        Program.slashGame.EventManager.QueueEvent(GameEventEnum.LocationChanged, new LocationChangedData(x, form.Location.GetAsInt, startLocationInt));
                    }
                }
                //else
                //{
                //    slashGame.EventManager.QueueEvent(GameEventEnum.Attack, new AttackEventData(x, move.MovementDirection));
                //}

                move.MovementDirection = MovementDirectionEnum.Stopped;
            }
        }
    }

    public void OnMovement(GameEvent e)
    {
        var enm = Program.slashGame.EntityManager;
        var data = (MovementEventData)e.EventData;

        if (enm.TryGetComponent(data.EntityID, out MovementComponent move))
        {
            move.MovementDirection = data.Direction;
        }
    }
}

public class MovementComponent : IEntityComponent
{
    public const string AttributeSpeed = "MovementComponent.Speed";

    public float Speed_Base { get; set; }
    public MovementDirectionEnum MovementDirection { get; set; }

    public void InitComponent(IAttributeTable attributeTable)
    {
        Speed_Base = attributeTable.GetIntOrDefault(AttributeSpeed, 1);
        MovementDirection = MovementDirectionEnum.Stopped;
    }

    public MovementComponent(int entityid, float speed, LocationDataInt location)
    {
        Speed_Base = speed;
        MovementDirection = MovementDirectionEnum.Stopped;
    }
}

public class MovementEventData
{
    public int EntityID { get; }
    public MovementDirectionEnum Direction { get; }

    public MovementEventData(int entityid, MovementDirectionEnum direction)
    {
        EntityID = entityid;
        Direction = direction;
    }
}

public class LocationChangedData
{
    public int EntityID { get; }
    public LocationDataInt Location_New { get; }
    public LocationDataInt Location_Old { get; }

    public LocationChangedData(int entityID, LocationDataInt location_New, LocationDataInt location_Old)
    {
        EntityID = entityID;
        Location_New = location_New;
        Location_Old = location_Old;
    }
}

public enum MovementDirectionEnum
{
    Stopped,
    North,
    East,
    South,
    West
}





    