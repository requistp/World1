using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhUtilityAI;
using Slash.Application.Systems;
using Slash.Collections.AttributeTables;
using Slash.ECS.Components;
using Slash.ECS.Events;

[GameSystem]
public class PhUtilityAISystem : GameSystem
{
    public const float DefaultTimerForIdleCondition = 3.0f;

    public static SadConsole.Console Display;

    public override void Init(IAttributeTable configuration)
    {
        base.Init(configuration);
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        var slashGame = Program.slashGame;

        foreach (int x in slashGame.EntityManager.EntitiesWithComponent(typeof(PhUtilityAIComponent)))
        {
            var uai = slashGame.EntityManager.GetComponent<PhUtilityAIComponent>(x);

            uai.ActivityOptions.CurrentActivity?.StopCondition.Update(dt);

            uai.ActivityOptions.SetActivityIfNeeded();
        }
    }
}

public class PhUtilityAIComponent : IEntityComponent
{
    public int EntityID { get; }

    public ThingWithActivityOptions ActivityOptions { get; }

    public void EvaluateOptions() => ActivityOptions.EvaluateOptions();

    public void AddActivityOption(ActivityOptionEnums activityOption, ActivityOptionEnums? backupActivityOption, bool enabled, bool skipIfNotExecutable, ActivityOption.Get_X_Method getXMethod, ActivityOption.Get_IsExecutable_Method getIsExecutableMethod, ActivityOption.NewEventSet_Method newEventCall, bool ReEvaluateOptions, AbstractStopCondition stopCondition)
        => ActivityOptions.Add((int)activityOption, (int?)backupActivityOption, activityOption.ToString(), enabled, skipIfNotExecutable, getXMethod, getIsExecutableMethod, newEventCall, ReEvaluateOptions, stopCondition);

    public PhUtilityAIComponent(int entityid)
    {
        EntityID = entityid;

        ActivityOptions = new ThingWithActivityOptions((int)ActivityOptionEnums.Idle, ActivityOptionEnums.Idle.ToString(), null);
        ActivityOptions[(int)ActivityOptionEnums.Idle].StopCondition = new PhUtilityStopCondition_Timer(PhUtilityAISystem.DefaultTimerForIdleCondition, ActivityOptions.EvaluateOptions);
    }

    public void InitComponent(IAttributeTable attributeTable) { }
}

public class PhUtilityStopCondition_GameEventEnum : AbstractStopCondition
{
    public GameEventEnum GameEventTrigger { get; }

    public override Call_OnStop OnStopCall { get; }
    
    public override void Restart()
    {
        Stopped = false;
    }

    public override void Update(float dt) { }

    public PhUtilityStopCondition_GameEventEnum(GameEventEnum gameEventTrigger, Call_OnStop onStopCall)
    {
        GameEventTrigger = gameEventTrigger;
        OnStopCall = onStopCall;
    }
}
public class PhUtilityStopCondition_Timer : AbstractStopCondition
{
    public float Duration { get; set; }
    public float TimeRemaining { get; set; }
    
    public override Call_OnStop OnStopCall { get; }

    public override void Restart()
    {
        TimeRemaining = Duration;
        Stopped = false;
    }

    public override void Update(float dt)
    {
        if (!Stopped)
        {
            TimeRemaining -= dt;

            if (TimeRemaining <= 0)
            {
                Finish();
            }
        }
    }

    public PhUtilityStopCondition_Timer(float duration, Call_OnStop onStopCall)
    {
        Duration = duration;
        OnStopCall = onStopCall;
        TimeRemaining = duration;
    }
}

public enum ActivityOptionEnums
{
    Idle = 0, // Default, required, don't change ID

    Eat = 1,
    Explore = 2
}


