namespace ExampleAI
{
    public abstract class AbstractAction : Crystal.ActionBase<AIUtilityComponent>
    {
        public abstract override Crystal.IAction Clone();

        public AbstractAction(AbstractAction other) : base(other)
        {
        }

        public AbstractAction(ActionTypeEnums actionType, Crystal.IActionCollection collection) : base(actionType.ToString(), collection)
        {
        }
    }

    public enum ActionTypeEnums
    {
        Drink,
        Eat,
        Shower,
        Toilet,
        Exercise,
        Work,
        Sleep,
        Read,
        Idle
    }
}


