namespace ExampleAI {

    public class EatAction : AbstractAction
    {
        public static ActionTypeEnums ActionType = ActionTypeEnums.Eat;

        public override Crystal.IAction Clone() => new EatAction(this);

        protected override void OnExecute(AIUtilityComponent aic)
        {
            aic.LastAction = ActionType;

            aic.Hunger -= 80f;
            aic.Bladder += 20f;
            aic.Wealth -= 50f;

            EndInSuccess(aic);
        }

        //protected override void OnUpdate(AIUtilityComponent context)
        //{
        //}

        public EatAction(EatAction other) : base(other)
        {
        }

        public EatAction(Crystal.IActionCollection collection) : base(ActionType, collection)
        {
        }
    }
}


//public class EatAction : AbstractAction
//{
//    public static readonly string Name = "Eat";

//    public override IAction Clone()
//    {
//        return new EatAction(this);
//    }

//    protected override void OnExecute(AIUtilityComponent aic)
//    {
//        base.OnExecute(aic); // Just records LastAction

//        aic.Hunger -= 80f;
//        aic.Bladder += 20f;
//        aic.Wealth -= 50f;
//        EndInSuccess(aic);
//    }

//    protected override void OnUpdate(AIUtilityComponent context)
//    {
//    }

//    public EatAction()
//    {
//    }

//    EatAction(EatAction other) : base(other)
//    {
//    }

//    public EatAction(IActionCollection collection) : base(Name, collection)
//    {
//    }
//}