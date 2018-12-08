namespace ExampleAI
{
    public class IdleAction : AbstractAction
    {
        public static ActionTypeEnums ActionType = ActionTypeEnums.Idle;

        public override Crystal.IAction Clone() => new IdleAction(this);

        protected override void OnExecute(AIUtilityComponent aic)
        {
            System.Diagnostics.Debugger.Log(1, "test", "Idle OnExecute\n");

            aic.LastAction = ActionType;

            aic.Fitness -= 0.2f;

            EndInSuccess(aic);
        }

        //protected override void OnUpdate(AIUtilityComponent aic)
        //{
        //}

        public IdleAction(IdleAction other) : base(other)
        {
        }

        public IdleAction(Crystal.IActionCollection collection) : base(ActionType, collection)
        {
        }
    }
}



         //public static readonly string Name = "Idle";

        //public override IAction Clone()
        //{
        //    return new IdleAction(this);
        //}
       //protected override void OnUpdate(AIUtilityComponent context)
        //{

        //}

        //public IdleAction()
        //{
        //}

        //IdleAction(IdleAction other) : base(other)
        //{
        //}
