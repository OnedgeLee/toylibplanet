using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toylibplanet
{
    public class NullState : IState
    {
        public NullState() => this._stateInts = new List<int>(new int[1]);
    }
}
