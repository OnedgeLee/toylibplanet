namespace Toylibplanet
{
    public class NullState : IState
    {
        public NullState() => this._stateInts = new List<int>(new int[1]);
    }
}
