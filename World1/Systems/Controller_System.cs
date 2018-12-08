using System.Collections.Generic;
using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;

[GameSystem]
public class ControllerSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        Program.slashGame.EventManager.RegisterListener(GameEventEnum.ControllerInput, this.OnControllerInput);
    }

    public void OnControllerInput(GameEvent e)
    {
        var slashGame = Program.slashGame;

        var inputState = (ControllerEventData)e.EventData;

        foreach (int x in slashGame.EntityManager.EntitiesWithComponent(typeof(ControllerComponent)))
        {
            var cont = slashGame.EntityManager.GetComponent<ControllerComponent>(x);

            if (cont != null)
            {
                //if (inputState.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F1))
                //{
                //    System.Diagnostics.Debugger.Log(1, "test", "F1\n");
                //    slashGame.EventManager.QueueEvent(GameEventEnum.MemoryDisplayRequest);
                //}

                if (inputState.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
                {
                    slashGame.EventManager.QueueEvent(GameEventEnum.Movement, new MovementEventData(x, MovementDirectionEnum.North));
                }
                else if (inputState.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
                {
                    slashGame.EventManager.QueueEvent(GameEventEnum.Movement, new MovementEventData(x, MovementDirectionEnum.East));
                }
                else if (inputState.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
                {
                    slashGame.EventManager.QueueEvent(GameEventEnum.Movement, new MovementEventData(x, MovementDirectionEnum.South));
                }
                else if (inputState.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
                {
                    slashGame.EventManager.QueueEvent(GameEventEnum.Movement, new MovementEventData(x, MovementDirectionEnum.West));
                }

                inputState.KeyboardState.Clear();

                break;
            }
        }
    }
}

public class ControllerComponent : IEntityComponent
{
    public void InitComponent(IAttributeTable attributeTable)
    {
    }
}

public class ControllerEventData
{
    public SadConsole.Input.Keyboard KeyboardState;

    public ControllerEventData(SadConsole.Input.Keyboard newKeyboardState)
    {
        KeyboardState = newKeyboardState;
    }
}
  