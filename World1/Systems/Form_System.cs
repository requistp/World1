using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;

[GameSystem]
public class FormSystem : GameSystem
{
    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);

        Program.slashGame.EventManager.RegisterListener(GameEventEnum.Component_New_Form, OnNewForm);
        Program.slashGame.EventManager.RegisterListener(GameEventEnum.Death, OnDeath);
    }

    public void OnNewForm(GameEvent e)
    {
        var form = (FormComponent)e.EventData;

        var console1 = new SadConsole.Console(1, 1);
        console1.Print(0, 0, form.Symbol);
        console1.Position = new Point((int)form.Location.X, (int)form.Location.Y);
        SadConsole.Global.CurrentScreen.Children.Add(console1);
        
        form.Display = console1;
    }

    public void OnDeath(GameEvent e)
    {
        var data = (DeathEventData)e.EventData;

        var form = Program.slashGame.EntityManager.GetComponent<FormComponent>(data.EntityID);

        form.Display.Parent.Children.Remove(form.Display);
    }
}

public class FormComponent : IEntityComponent
{
    public const string AttributeSymbol = "FormComponent.Symbol";

    public bool IsPassable { get; }
    public string Name { get; }
    public string Symbol { get; }
    public LocationDataFloat Location { get; set; }
    public LocationDataInt Location_Born { get; }
    public SadConsole.Console Display { get; set; }

    public void InitComponent(IAttributeTable attributeTable) { }

    public FormComponent(string symbol, LocationDataFloat location, bool isPassable, string name = "")
    {
        IsPassable = isPassable;
        Symbol = symbol;
        Location = location;
        Location_Born = Location.GetAsInt;
        Name = name;
        Program.slashGame.EventManager.QueueEvent(GameEventEnum.Component_New_Form, this);
    }
}


