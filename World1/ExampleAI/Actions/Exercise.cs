namespace ExampleAI
{
    public class ExerciseAction : AbstractAction
    {
        public static ActionTypeEnums ActionType = ActionTypeEnums.Exercise;

        private float dFit;

        public override Crystal.IAction Clone() => new ExerciseAction(this);

        protected override void OnExecute(AIUtilityComponent aic)
        {
            aic.LastAction = ActionType;

            dFit = 0f;

            Exercise(aic);
        }

        protected override void OnUpdate(AIUtilityComponent aic)
        {
            if (dFit >= 60f || aic.Fitness > 98f)
            {
                EndInSuccess(aic);
            }

            Exercise(aic);
        }

        public ExerciseAction(ExerciseAction other) : base(other)
        {
        }

        public ExerciseAction(Crystal.IActionCollection collection) : base(ActionType, collection)
        {
        }

        private void Exercise(AIUtilityComponent context)
        {
            dFit += 10f;
            context.Fitness += 10f;
            context.Bladder += 1.5f;
            context.Hunger += 1.5f;
            context.Thirst += 2.5f;
            context.Energy -= 7.5f;
            // Those expensive gyms.. ;) 
            context.Wealth -= 20f;
            context.Cleanliness -= 2.5f;
        }
    }
}


//public class PhysicalExerciseAction : ActionBase<AIUtilityComponent>
//{
//    public static readonly string Name = "Exercise";
//    float dFit;
//    public override IAction Clone()
//    {
//        return new PhysicalExerciseAction(this);
//    }

//    protected override void OnExecute(AIUtilityComponent aic)
//    {
//        base.OnExecute(aic); // Just records LastAction

//        dFit = 0f;
//        Exercise(aic);
//    }

//    protected override void OnUpdate(AIUtilityComponent context)
//    {
//        if (dFit >= 60f || context.Fitness > 98f)
//            EndInSuccess(context);

//        Exercise(context);
//    }

//    public PhysicalExerciseAction()
//    {
//    }

//    PhysicalExerciseAction(PhysicalExerciseAction other) : base(other)
//    {
//    }

//    public PhysicalExerciseAction(IActionCollection collection) : base(Name, collection)
//    {
//    }

//    void Exercise(AIUtilityComponent context)
//    {
//        dFit += 10f;
//        context.Fitness += 10f;
//        context.Bladder += 1.5f;
//        context.Hunger += 1.5f;
//        context.Thirst += 2.5f;
//        context.Energy -= 7.5f;
//        // Those expensive gyms.. ;) 
//        context.Wealth -= 20f;
//        context.Cleanliness -= 2.5f;
//    }
//}