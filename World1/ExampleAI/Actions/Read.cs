namespace ExampleAI
{
    public class ReadAction : AbstractAction
    {
        public static ActionTypeEnums ActionType = ActionTypeEnums.Read;

        public override Crystal.IAction Clone() => new ReadAction(this);

        protected override void OnExecute(AIUtilityComponent aic)
        {
            aic.LastAction = ActionType;

            aic.Energy -= 1f;
            aic.Wealth -= 10f;
            aic.Fitness -= 1f;
            
            EndInSuccess(aic);
        }

        //protected override void OnUpdate(AIUtilityComponent aic)
        //{
        //}

        public ReadAction(ReadAction other) : base(other)
        {
        }

        public ReadAction(Crystal.IActionCollection collection) : base(ActionType, collection)
        {
        }
    }

    //public class ReadAction : AbstractAction
    //{

    //    protected override void OnExecute(AIUtilityComponent aic)
    //    {
    //        System.Diagnostics.Debugger.Log(1, "test", "Read_OnExecute running...\n");

    //        base.OnExecute(aic); // Just records LastAction

    //        aic.Energy -= 1f;
    //        aic.Wealth -= 10f;
    //        aic.Fitness -= 1f;

    //        EndInSuccess(aic);
    //    }

    //    public ReadAction(Crystal.IActionCollection collection) : base(ActionTypeEnums.Read, collection)
    //    {
    //    }
    //}
}


//public class ReadAction : ActionBase<AIUtilityComponent>
//{
//    public static readonly string Name = "Read";

//    public override IAction Clone()
//    {
//        return new ReadAction(this);
//    }

//    protected override void OnExecute(AIUtilityComponent aic)
//    {
//        base.OnExecute(aic); // Just records LastAction

//        aic.Energy -= 1f;
//        aic.Wealth -= 10f;
//        aic.Fitness -= 1f;

//        EndInSuccess(aic);
//    }

//    protected override void OnUpdate(AIUtilityComponent context)
//    {
//    }

//    public ReadAction()
//    {
//    }

//    ReadAction(ReadAction other) : base(other)
//    {
//    }

//    public ReadAction(IActionCollection collection) : base(Name, collection)
//    {
//    }
//}