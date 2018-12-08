using Crystal;


namespace ExampleAI {

    internal class ExAiConstructor : AiConstructor
    {
        protected override void DefineActions()
        {
            A = new DrinkAction(Actions);
            A = new EatAction(Actions);
            A = new ShowerAction(Actions);
            A = new ToiletAction(Actions);
            A = new SleepAction(Actions);
            A = new ExerciseAction(Actions);

            A = new WorkAction(Actions);
            A = new ReadAction(Actions);

            A = new IdleAction(Actions);
        }

        protected override void DefineConsiderations()
        {
            C = new BladderConsideration(Considerations);
            C = new ShowerConsideration(Considerations);
            C = new HungerConsideration(Considerations);
            C = new ThirstConsideration(Considerations);
            C = new EnergyConsideration(Considerations);
            C = new TirednessConsideration(Considerations);
            C = new HowUnfitConsideration(Considerations);
            C = new GreedConsideration(Considerations);
            C = new CuriosityConsideration(Considerations);

            Cc = new CompositeConsideration(ConsiderationDefs.LiveLong, Considerations);
            Cc.AddConsideration(ConsiderationDefs.Tiredness);
            Cc.AddConsideration(ConsiderationDefs.Hunger);
            Cc.AddConsideration(ConsiderationDefs.Thirst);
            Cc.Measure = new Chebyshev();

            Cc = new CompositeConsideration(ConsiderationDefs.Prosper, Considerations);
            Cc.AddConsideration(ConsiderationDefs.Curiosity);
            Cc.AddConsideration(ConsiderationDefs.Greed);
            Cc.Measure = new WeightedMetrics(3.0f);
        }

        protected override void DefineOptions()
        {
            O = new Option(OptionDefs.Drink, Options);
            O.SetAction(ActionTypeEnums.Drink.ToString());
            O.AddConsideration(ConsiderationDefs.Thirst);

            O = new Option(OptionDefs.Eat, Options);
            O.SetAction(ActionTypeEnums.Eat.ToString());
            O.AddConsideration(ConsiderationDefs.Hunger);

            O = new Option(OptionDefs.Shower, Options);
            O.SetAction(ActionTypeEnums.Shower.ToString());
            O.AddConsideration(ConsiderationDefs.Shower);

            O = new Option(OptionDefs.Sleep, Options);
            O.SetAction(ActionTypeEnums.Sleep.ToString());
            O.AddConsideration(ConsiderationDefs.Tiredness);

            O = new Option(OptionDefs.Toilet, Options);
            O.SetAction(ActionTypeEnums.Toilet.ToString());
            O.AddConsideration(ConsiderationDefs.Bladder);

            O = new Option(OptionDefs.Exercise, Options);
            O.SetAction(ActionTypeEnums.Exercise.ToString());
            O.AddConsideration(ConsiderationDefs.HowUnfit);
            O.AddConsideration(ConsiderationDefs.Energy);
            O.Measure = new MultiplicativePseudoMeasure();

            O = new Option(OptionDefs.Work, Options);
            O.SetAction(ActionTypeEnums.Work.ToString());
            O.AddConsideration(ConsiderationDefs.Greed);

            O = new Option(OptionDefs.Read, Options);
            O.SetAction(ActionTypeEnums.Read.ToString());
            O.AddConsideration(ConsiderationDefs.Curiosity);

            O = new ConstantUtilityOption(OptionDefs.Idle, Options);
            O.SetAction(ActionTypeEnums.Idle.ToString());
            O.DefaultUtility = new Utility(0.01f, 1f);
        }

        protected override void DefineBehaviours()
        {
            B = new Behaviour(BehaviourDefs.LiveLong, Behaviours);
            B.AddOption(OptionDefs.Drink);
            B.AddOption(OptionDefs.Eat);
            B.AddOption(OptionDefs.Toilet);
            B.AddOption(OptionDefs.Sleep);
            B.AddOption(OptionDefs.Shower);
            B.AddOption(OptionDefs.Exercise);
            B.AddOption(OptionDefs.Idle);
            B.AddConsideration(ConsiderationDefs.LiveLong);

            B = new Behaviour(BehaviourDefs.Prosper, Behaviours);
            B.AddOption(OptionDefs.Work);
            B.AddOption(OptionDefs.Read);
            B.AddConsideration(ConsiderationDefs.Prosper);
        }

        protected override void ConfigureAi()
        {
            Ai = new UtilityAi(BrainTypeEnum.Animal0.ToString(), AIs);
            Ai.AddBehaviour(BehaviourDefs.LiveLong);
            Ai.AddBehaviour(BehaviourDefs.Prosper);
        }

        public ExAiConstructor(IAiCollection collection) : base(collection)
        {
        }
    }

}