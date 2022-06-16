using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toylibplanet;

namespace Toylibplanet.Tests
{
    public class TestState : IState
    {
        public TestState()
        {
            this._stateInts = new List<int> { 0, 0, 0 };
        }
    }

}
