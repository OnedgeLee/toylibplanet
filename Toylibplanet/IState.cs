using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toylibplanet
{
    public abstract class IState : ICloneable
    {
        protected List<int> _stateInts;
        public List<int> StateInts { get => _stateInts; set { _stateInts = value; } }

        public IState()
        {
            this._stateInts = new List<int>();
        }
        public byte[] Serialize()
        {
            // For simplification, added some restrictions
            // 1. State is a set of information represented with integers
            // 2. Each integer represents some value of system
            // 3. These integers will be represented as a list format

            byte[] stateIntBytes = Array.Empty<byte>();
            foreach (int stateInt in this._stateInts)
	        {
                stateIntBytes = stateIntBytes.Concat(BitConverter.GetBytes(stateInt)).ToArray();
	        }
            stateIntBytes = BitConverter.GetBytes(stateIntBytes.Length).Concat(stateIntBytes).ToArray();
            return stateIntBytes;
        }
        protected virtual IState DeepCopy()
        {
            IState other = (IState)this.MemberwiseClone();
            other._stateInts = this._stateInts.ConvertAll(i => i);
            return other;
        }
        public IState Clone()
        {
            return DeepCopy();
        }
        object ICloneable.Clone()
        {
            return DeepCopy();
        }
    }
}
