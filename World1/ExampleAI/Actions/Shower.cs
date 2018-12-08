namespace ExampleAI
{
    public class ShowerAction : AbstractAction
    {
        public static ActionTypeEnums ActionType = ActionTypeEnums.Shower;

        public override Crystal.IAction Clone() => new ShowerAction(this);

        protected override void OnExecute(AIUtilityComponent aic)
        {
            aic.LastAction = ActionType;

            aic.Cleanliness += 90f;
            aic.Energy += 2.5f;

            EndInSuccess(aic);
        }

        //protected override void OnUpdate(AIUtilityComponent aic)
        //{
        //}

        public ShowerAction(ShowerAction other) : base(other)
        {
        }

        public ShowerAction(Crystal.IActionCollection collection) : base(ActionType, collection)
        {
        }
    }
}



//public class ShowerAction : ActionBase<AIUtilityComponent>
//{
//    public static readonly string Name = "Shower";

//    public override IAction Clone()
//    {
//        return new ShowerAction(this);
//    }

//    protected override void OnExecute(AIUtilityComponent aic)
//    {
//        base.OnExecute(aic); // Just records LastAction

//        aic.Cleanliness += 90f;
//        aic.Energy += 2.5f;

//        EndInSuccess(aic);
//    }

//    protected override void OnUpdate(AIUtilityComponent context)
//    {
//    }

//    public ShowerAction()
//    {
//    }

//    ShowerAction(ShowerAction other) : base(other)
//    {
//    }

//    public ShowerAction(IActionCollection collection) : base(Name, collection)
//    {
//    }
//}
