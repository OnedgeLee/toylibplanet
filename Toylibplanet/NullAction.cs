using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toylibplanet
{
    public class NullAction : IAction
    {
        public NullAction(string actionInfo) : base(actionInfo)
        {

        }
        public NullAction()
        {
            _actionName = "null";
            _actionInfo = "null";
        }

        protected override string SetActionName()
        {
            string actionName = "null";
            return actionName;
        }

        public override IState Execute(IState state)
        {
            return state;
        }
    }
}
