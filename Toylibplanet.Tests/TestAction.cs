namespace Toylibplanet.Tests
{
    public class TestAction : IAction
    {
        protected override string SetActionName()
        {
            return "test";
        }
        public override IState Execute(IState state)
        {
            state.StateInts[1] += 1;
            state.StateInts[2] += 2;
            return state;
        }
    }
}
