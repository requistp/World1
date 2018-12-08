namespace ExampleAI {

    public class DrinkAction : AbstractAction
    {
        public static ActionTypeEnums ActionType = ActionTypeEnums.Drink;

        public override Crystal.IAction Clone() => new DrinkAction(this);

        protected override void OnExecute(AIUtilityComponent aic)
        {
            aic.LastAction = ActionType;

            aic.Bladder += 25f;
            aic.Thirst -= 90f;
            aic.Wealth -= 10f;

            EndInSuccess(aic);
        }

        //protected override void OnUpdate(AIUtilityComponent aic)
        //{
        //}

        public DrinkAction(DrinkAction other) : base(other)
        {
        }

        public DrinkAction(Crystal.IActionCollection collection) : base(ActionType, collection)
        {
        }
    }
}



/*
    public class DrinkAction : ActionBase<AIUtilityComponent>
    {
        public static readonly string Name = "Drink";

        public override IAction Clone()
        {
            return new DrinkAction(this);
        }

        protected override void OnExecute(AIUtilityComponent context)
        {
            //System.Diagnostics.Debugger.Log(1, "test", Name + "\n");
            //.Provider.UpdateLastAction(Name);
            context.LastAction = Name;
            context.Bladder += 25f;
            context.Thirst -= 90f;
            context.Wealth -= 10f;
            EndInSuccess(context);
        }

        protected override void OnUpdate(AIUtilityComponent context)
        {
        }

        public DrinkAction()
        {
        }

        DrinkAction(DrinkAction other) : base(other)
        {
        }

        public DrinkAction(IActionCollection collection) : base(Name, collection)
        {
        }
    }
 */
