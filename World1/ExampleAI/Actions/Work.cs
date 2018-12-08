namespace ExampleAI
{
    public class WorkAction : AbstractAction
    {
        public static ActionTypeEnums ActionType = ActionTypeEnums.Work;

        private float dWealth = 20f;
        private float accumulatedWealth;
        private float maxWealthPerSession = 100f;

        public override Crystal.IAction Clone() => new WorkAction(this);

        protected override void OnExecute(AIUtilityComponent aic)
        {
            aic.LastAction = ActionType;

            accumulatedWealth = 0;

            Work(aic);
        }

        protected override void OnUpdate(AIUtilityComponent aic)
        {
            if (accumulatedWealth >= maxWealthPerSession)
            {
                EndInSuccess(aic);
            }

            Work(aic);
        }

        public WorkAction(WorkAction other) : base(other)
        {
        }

        public WorkAction(Crystal.IActionCollection collection) : base(ActionType, collection)
        {
        }

        private void Work(AIUtilityComponent context)
        {
            accumulatedWealth += dWealth;
            context.Wealth += dWealth;
            context.Energy -= 2.5f;
            context.Thirst += 2.5f;
            context.Hunger += 4f;
            context.Cleanliness -= 2.5f;
        }
    }
}






//public class WorkAction : ActionBase<AIUtilityComponent>
//{
//    public static readonly string Name = "Work";
//    float dWealth = 20f;
//    float accumulatedWealth;
//    float maxWealthPerSession = 100f;
//    public override IAction Clone()
//    {
//        return new WorkAction(this);
//    }

//    protected override void OnExecute(AIUtilityComponent aic)
//    {
//        base.OnExecute(aic); // Just records LastAction

//        accumulatedWealth = 0;
//        Work(aic);
//    }

//    protected override void OnUpdate(AIUtilityComponent context)
//    {
//        if (accumulatedWealth >= maxWealthPerSession)
//            EndInSuccess(context);

//        Work(context);
//    }

//    public WorkAction()
//    {
//    }

//    WorkAction(WorkAction other) : base(other)
//    {
//    }

//    public WorkAction(IActionCollection collection) : base(Name, collection)
//    {
//    }

//    void Work(AIUtilityComponent context)
//    {
//        accumulatedWealth += dWealth;
//        context.Wealth += dWealth;
//        context.Energy -= 2.5f;
//        context.Thirst += 2.5f;
//        context.Hunger += 4f;
//        context.Cleanliness -= 2.5f;
//    }

//}
