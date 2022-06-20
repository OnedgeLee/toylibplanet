namespace Toylibplanet
{
    public class BlockChain
    {
        private readonly Dictionary<string, Block> _blocks;
        // Hash table of blocks

        private readonly Block _genesisBlock;
        // Genesis block will be maintained separately

        private Block _lastBlock;
        // Block that has been mined most recently will added to here

        public void Pop()
        {
            _lastBlock = FindBlock(_lastBlock.PreviousHash);
        }

        public Dictionary<string, Block> Blocks { get => _blocks; }
        public Block GenesisBlock { get => _genesisBlock; }
        public Block LastBlock { get => _lastBlock; }

        public BlockChain(IState initialState)
        {
            this._blocks = new Dictionary<string, Block>();
            // When block chain has been estabilished, genesis block will be automatically generated

            this._genesisBlock = new Block(initialState);
            Append(this._genesisBlock);
            this._lastBlock = this._genesisBlock;
            // Generated genesis block will be added to block pool
        }

        public void Append(Block block)
        {
            this._lastBlock = block;
            this._blocks.Add(Utility.BytesToHex(block.BlockHash), block);
            // Block will be added to its hash table and latest block cache
        }

        public Block FindBlock(byte[] blockHash)
        {
            return this.Blocks[Utility.BytesToHex(blockHash)];
        }

        public void Mine(
            byte[] minerPublicKey,
            IEnumerable<Tx> txs)
        {
            int index = this._blocks.Count;
            // Block index will be started from 0, and will be added as block mined

            int difficulty = Difficulty();
            // Difficulty have to be adjusted automatically 

            byte[] rewardBeneficiary = minerPublicKey;
            // Minder will be paid bitcoin as a reward

            byte[] previousHash = this._lastBlock.BlockHash;
            // To track previous block, block hash of previous block will be added to block
            // And this hash of previous block is also hashed to generate new block hash, we can sure about block sequence
            // So it can be called 'block chain'

            txs = txs.Where(tx => tx.Verify());
            // Remove transactions that cannot be verified from mempool not to be selected as block candidate

            IState state = Block.EvaluateActions(this._lastBlock.State, txs);
            // Compute new state with actions in transactions that will be added to block
            // Actually, it indicates address of state storage, not actual state in libplanet
            
            // For detailed implementation, previous state fetching and  state hash computation line will be added here
            // And will be injected to block instead of actual state, and saved to store with its hash as address

            Block block = new(
                index,
                difficulty,
                rewardBeneficiary,
                previousHash,
                state,
                txs);
            // Block will be mined

            Append(block);
            // If block is properly mined, it will be added to blockchain
            // On detailed implementation, this will be broadcasted with message
            // And each client peers will verify it, and if it's not valid, won't add it to their blockchain
            // If most of peers don't add block, block that will be mined later has low chance to include this block
            // So invalid block will be ignored on whole network
        }

        public int Difficulty()
        {
            if (this._lastBlock.BlockHash.SequenceEqual(this._genesisBlock.BlockHash))
            {
                return 10;
            }
            TimeSpan miningInterval = this._lastBlock.Timestamp - FindBlock(this._lastBlock.PreviousHash).Timestamp;
            int miningSeconds = miningInterval.Seconds;
            return 1 / miningSeconds;
            // Actually, difficulty have to computed with average block generation time
            // Average block generation time can be computed with timestamp in blocks
            // Chance to find a nonce is 1 / difficulty
            // If mining speed is too fast, raise difficulty to slow down, and vice versa
        }
    }
}
