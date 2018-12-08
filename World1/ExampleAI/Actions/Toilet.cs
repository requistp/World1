namespace ExampleAI
{
    public class ToiletAction : AbstractAction
    {
        public static ActionTypeEnums ActionType = ActionTypeEnums.Toilet;

        public override Crystal.IAction Clone() => new ToiletAction(this);

        protected override void OnExecute(AIUtilityComponent aic)
        {
            aic.LastAction = ActionType;

            aic.Bladder -= 90f;
            aic.Cleanliness -= 10f;

            EndInSuccess(aic);
        }

        //protected override void OnUpdate(AIUtilityComponent aic)
        //{
        //}

        public ToiletAction(ToiletAction other) : base(other)
        {
        }

        public ToiletAction(Crystal.IActionCollection collection) : base(ActionType, collection)
        {
        }
    }
}



//public class ToiletAction : AbstractAction
//{
//    //public static readonly string Name = "Toilet";

//    //public override IAction Clone()
//    //{
//    //    return new ToiletAction(this);
//    //}

//    protected override void OnExecute(AIUtilityComponent aic)
//    {
//        base.OnExecute(aic); // Just records LastAction

//        aic.Bladder -= 90f;
//        aic.Cleanliness -= 10f;

//        EndInSuccess(aic);
//    }

//    //protected override void OnUpdate(AIUtilityComponent context)
//    //{
//    //}

//    //public ToiletAction()
//    //{
//    //}

//    //ToiletAction(ToiletAction other) : base(other)
//    //{
//    //}

//    public ToiletAction(Crystal.IActionCollection collection) : base(ActionTypeEnum.Toilet, collection)
//    {
//    }
//}
