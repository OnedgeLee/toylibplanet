using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toylibplanet
{
    public abstract class IAction
    {
        protected string _actionName;
        protected string _actionInfo;

        public string ActionName { get { return _actionName; } }
        public string ActionInfo { get { return _actionInfo; } }

        public IAction(string actionInfo)
        {
            _actionName = SetActionName();
            _actionInfo = actionInfo;
        }
        public IAction()
        {
            _actionName = SetActionName();
            _actionInfo = "";
        }
        public IAction(string actionName, string actionInfo)
        {
            _actionName = actionName;
            _actionInfo = actionInfo;
        }

        public byte[] Serialize()
        {
            byte[] actionNameBytes = Encoding.UTF8.GetBytes(this._actionName);
            byte[] actionInfoBytes = Encoding.UTF8.GetBytes(this._actionInfo);
            byte[] actionNameBytesSize = BitConverter.GetBytes(actionNameBytes.Length);
            byte[] actionInfoBytesSize = BitConverter.GetBytes(actionInfoBytes.Length);
            byte[] payload = actionNameBytesSize.Concat(actionNameBytes)
                .Concat(actionInfoBytesSize).Concat(actionInfoBytes).ToArray();
            // Returned byte will be
            // [actionNameBytesSize(4byte)] [actionNameBytes(?byte)]
            // [actionInfoBytesSize(4byte)] [actionInfoBytes(?byte)]
            return payload;
        }
        //public IAction Deserialize(byte[] serializedIAction)
        //{
        //    int actionNameBytesSize = BitConverter.ToInt32(serializedIAction.Take(4).ToArray());
        //    string actionName = Encoding.UTF8.GetString(serializedIAction.Skip(4).Take(actionNameBytesSize).ToArray());
        //    int actionInfoBytesSize = BitConverter.ToInt32(serializedIAction.Skip(4 + actionNameBytesSize).Take(8 + actionNameBytesSize).ToArray());
        //    string actionInfo = Encoding.UTF8.GetString(serializedIAction.Skip(8 + actionNameBytesSize).Take(8 + actionNameBytesSize + actionInfoBytesSize).ToArray());
        //    return new IAction(actionName, actionInfo);
        //}

        protected abstract string SetActionName();
        public abstract IState Execute(IState state);
    }
}
