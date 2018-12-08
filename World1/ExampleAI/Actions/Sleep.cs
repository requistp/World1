namespace ExampleAI
{
    public class SleepAction : AbstractAction
    {
        public static ActionTypeEnums ActionType = ActionTypeEnums.Sleep;

        public override Crystal.IAction Clone() => new SleepAction(this);

        protected override void OnExecute(AIUtilityComponent aic)
        {
            System.Diagnostics.Debugger.Log(1, "test", ActionType.ToString() + " Execute\n");
            aic.LastAction = ActionType;

            Sleep(aic);
        }

        protected override void OnUpdate(AIUtilityComponent aic)
        {
            System.Diagnostics.Debugger.Log(1, "test", ActionType.ToString() + " Update\n");
            if (aic.Energy >= 98f)
            {
                EndInSuccess(aic);
            }

            Sleep(aic);
        }

        public SleepAction(SleepAction other) : base(other)
        {
        }

        public SleepAction(Crystal.IActionCollection collection) : base(ActionType, collection)
        {
        }

        private void Sleep(AIUtilityComponent aic)
        {
            aic.Energy += 3.5f;
            aic.Bladder += 1f;
            aic.Thirst += 0.5f;
            aic.Hunger += 0.9f;
        }
    }
}


//public class SleepAction : ActionBase<AIUtilityComponent>
//{
//    public static readonly string Name = "Sleep";

//    public override IAction Clone()
//    {
//        return new SleepAction(this);
//    }

//    protected override void OnExecute(AIUtilityComponent aic)
//    {
//        base.OnExecute(aic); // Just records LastAction

//        Sleep(aic);
//    }

//    protected override void OnUpdate(AIUtilityComponent context)
//    {
//        if (context.Energy >= 98f)
//            EndInSuccess(context);

//        Sleep(context);
//    }

//    public SleepAction()
//    {
//    }

//    SleepAction(SleepAction other) : base(other)
//    {
//    }

//    public SleepAction(IActionCollection collection) : base(Name, collection)
//    {
//    }

//    void Sleep(AIUtilityComponent context)
//    {
//        context.Energy += 3.5f;
//        context.Bladder += 1f;
//        context.Thirst += 0.5f;
//        context.Hunger += 0.9f;
//    }
//}
