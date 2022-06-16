using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toylibplanet
{
    public class BlockChain
    {
        private readonly Dictionary<byte[], Block> _blocks;
        private readonly Block _genesisBlock;
        private Block _lastBlock;
        public Block GenesisBlock { get => _genesisBlock; }
        public Block LastBlock { get => _lastBlock; }
        public Dictionary<byte[], Block> Blocks { get => _blocks; }

        public BlockChain(IState initialState)
        {
            this._blocks = new Dictionary<byte[], Block>();
            this._genesisBlock = new Block(initialState);
            Append(this._genesisBlock);
            this._lastBlock = this._genesisBlock;
        }
        
        public void Append(Block block)
        {
            this._lastBlock = block;
            this._blocks.Add(block.BlockHash, block);
        }

        public IState EvaluateActions(IEnumerable<Tx> txs)
        {
            IState state = _lastBlock.State.Clone();
            foreach (Tx tx in txs)
            {
                foreach (IAction action in tx.Actions)
                {
                    state = action.Execute(state);
                }
            }
            // Get state from last block in block chain, apply all actions in mempool to this state,
            // generate next state and use it as state in new block to be mined
            return state;
        }
        public void Mine(
            byte[] minerPublicKey,
            IEnumerable<Tx> txs)
        {
            int index = this._blocks.Count;
            int difficulty = 10;
            byte[] rewardBeneficiary = minerPublicKey;
            byte[] previousHash = this._lastBlock.BlockHash;
            txs = txs.Where(tx => tx.Verify());
            IState state = EvaluateActions(txs);
            Block block = new(
                index,
                difficulty,
                rewardBeneficiary,
                previousHash,
                state,
                txs);
            Append(block);
        }
    }
}
