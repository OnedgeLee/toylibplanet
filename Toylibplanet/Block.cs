using System.Security.Cryptography;
using System.Numerics;

namespace Toylibplanet
{
    public class Block
    {

        private readonly int _index;
        // Actually I think block index is not needed, since we have previous block hash
        // If there are any reason that I couldn't figure out, please tell me

        private readonly int _difficulty;
        // Difficulty have to be included on payload, since we have to use it on verification stage
        // Verification is, to verify if computed block hash is lower than difficulty
        // So if this information is missed, we can't know if block hash was generated properly

        // Imagine the situation someone generated fake block with low difficulty than blockchain consensus
        // Without this difficulty information, we can't figure out whether it's a fake block

        private readonly byte[] _nonce;
        // Nonce have to included on payload, since we have to use it on verification stage
        // We have to figure out if block hash that matches difficulty has been generated without cheating
        // So if this information is missed, we cn't know if block hash was generated properly

        // Imagine the situation someone generated fake block hash with low value without generating it from nonce
        // Without this nonce information, we can't reproduce block hash so we can't figure out whether it's a fake block

        private readonly byte[] _rewardBeneficiary;
        // Reward beneficiary have to be included on payload, to give a reward to block miner
        // Without it, evidence of mining subject disappears, so impossible to give a reward to miner
        // Since it's added to payload and hashed together, we can secure miner with verification

        private readonly byte[] _previousHash;
        // Previous hash have to be included on payload, to secure block sequence
        // Since it's added to payload and hashed together, we can secure block sequence with verification

        private readonly IState _state;
        // State have to be included on payload, not to evaluate action from initial (genesis) state
        // If this information isn't exist, we always have to compute whole actions on block chain from initial state
        // (Or, client will matain its own state, and each state can be diverged)
        // It's waste of computation and time, so adding state to chain would be better design
        // But state is so huge to be loaded on block,
        // using state hash as a key of storage and handlig it as state would be enough (but this implementation uses raw state)

        // Since each client will sync to state on block repeatedly,
        // Verification of block state is crucial
        // This can be accomplished by action evaluation
        // Each nodes will evaluate actions with their own states,
        // then figure out if new state is same as state in new block,
        // or if blockHashes are same

        private readonly IEnumerable<Tx> _transactions;
        // Transactions have to be included on payload, to secure properly signed action will be added to blockchain
        // Generating and verifying blocks, transaction verification is also performed
        // So every transaction in blockchain is secure and cannot be currupted

        private readonly DateTimeOffset _timestamp;
        // Timestamp have to be included on payload, to secure proper difficulty
        // It's critical to apply fair difficulty mining blocks,
        // and difficulty can be calculated with timestamps, computing block generation speed
        // So secure timestamp is crucial for blockchain system

        public byte[] Nonce { get => _nonce; }
        public byte[] BlockHash { 
            get
            {
                return HashBlock(Payload(), this._nonce);
            }
        }
        // Block hash is used to secure data in its payload
        // and also used for PoW, to secure blockchain consensus
        // Actually, block does not have to have this, and function for computing this is enough

        public byte[] PreviousHash { get => _previousHash; }
        public IState State { get => _state; }
        public IEnumerable<Tx> Transactions { get => _transactions; }
        public DateTimeOffset Timestamp { get => _timestamp; }

        public Block(
            int index,
            int difficulty,
            byte[] rewardBeneficiary,
            byte[] previousHash,
            IState state,
            IEnumerable<Tx> transactions)
        {
            this._index = index;
            this._difficulty = difficulty;
            this._rewardBeneficiary = rewardBeneficiary;
            this._previousHash = previousHash;
            this._state = state.Clone();
            this._transactions = transactions.Where(tx => tx.Verify());
            this._timestamp = DateTimeOffset.UtcNow;
            int seed = 0;
            this._nonce = Solve(Payload(), this._difficulty, seed);
        }
        public Block(IState initialState)
        // Null block for genesis block
        // I think it's better to declare genesis block class, but for convinience, implemented like this
        // New genesis block class always have to be verified, and have to indicate null previous block hash
        // Also, it have to be encoded with simple byte code, to people know it's genesis block
        // And it does not have to be mined
        // It doesn't need to have most of things, but at least it needs initial state
        {
            this._index = 0;
            this._difficulty = 1;
            this._rewardBeneficiary = new byte[256];
            Array.Clear(this._rewardBeneficiary, 0, 256);
            this._previousHash = new byte[256];
            Array.Clear(this._previousHash, 0, 256);
            this._state = initialState.Clone();
            this._transactions = new List<Tx> { new Tx() };
            this._timestamp = DateTimeOffset.UtcNow;
            int seed = 0;
            this._nonce = Solve(Payload(), this._difficulty, seed);
        }



        public byte[] Payload()
        // Actually, serialization is not needed for single-node condition (without network),
        // but at least, payload generation is needed for PoW.
        // It's not implemented in detail, but basic idea is to add fixed length bytes alone,
        // and add varied length bytes with its length bytes ahead.
        {
            byte[] indexBytes = BitConverter.GetBytes(this._index);
            byte[] difficultyBytes = BitConverter.GetBytes(this._difficulty);
            byte[] timestampBytes = Utility.DateTimeOffsetToBytes(this._timestamp);
            byte[] stateBytes = this._state.Serialize();
            IEnumerable<byte[]> txBytes = from tx in this._transactions select tx.Serialize();
            byte[] txsBytes = new byte[(from txByte in txBytes select txByte.Length).Sum()];

            int txsBytesOffset = 0;
            foreach (byte[] txByte in txBytes)
            {
                Buffer.BlockCopy(txByte, 0, txsBytes, txsBytesOffset, txByte.Length);
                txsBytesOffset += txByte.Length;
            }
            byte[] payload = indexBytes.Concat(difficultyBytes).
                Concat(this._rewardBeneficiary).Concat(this._previousHash).
                Concat(timestampBytes).Concat(stateBytes).Concat(txsBytes).ToArray();
            // [indexBytes (4bytes)] [difficultyBytes (4bytes)] [rewardBeneficiary (32bytes)]
            // [previousHash (32bytes)] [timestampBytes (16bytes)] [stateBytes (?bytes)] [txBytes (?bytes)]
            return payload;
        }
        public byte[] Serialize()
        {
            return Payload().Concat(this._nonce).Concat(this.BlockHash).ToArray();
            // Actually, if we don't use message on network, serialization is not needed
        }

        private static byte[] Solve(
            byte[] payload,
            long difficulty,
            int seed)
        {

            byte[] nonceBytes = new byte[10];
            Random random = new(seed);

            while (true)
            {
                random.NextBytes(nonceBytes);
                // nonceBytes will be filled with random values
                // Follows endian of computer architecture

                byte[] blockHash = HashBlock(payload, nonceBytes);
                // Generate blockHash, payload + nonceBytes with SHA256

                if (Satisfies(blockHash, difficulty))
                {
                    return nonceBytes;
                }
                // If blockHash matches difficulty, return answer nonce with its blockHash
                // blockHash is returned along to avoid redundant computation
            }
        }
        public static byte[] HashBlock(byte[] payload, byte[] nonceBytes)
        {
            byte[] blockBytes = payload.Concat(nonceBytes).ToArray();
            // blockBytes = payload + nonce

            SHA256 hasher = SHA256.Create();
            byte[] blockHash = hasher.ComputeHash(blockBytes);
            return blockHash;
            // blockHash = SHA256(payload + nonce)
        }

        private static bool Satisfies(
            byte[] blockHash, long difficulty)
        {
            // 2^256 exceeds the range of 64 bit, so cannot be interpreted by long
            // So, used BigInteger

            BigInteger blockHashNumber = new(blockHash);
            // Cast 32byte byte[] of blockHash into BigInteger

            byte[] maxTargetBytes = new byte[33];
            maxTargetBytes[32] = 0x01;
            // When generating BigInteger with byte array, byte array will be reversed
            // Why not 2^256 instead of 2^256 - 1? Still not sure

            BigInteger maxTarget = new(maxTargetBytes);
            BigInteger target = BigInteger.Divide(maxTarget, new BigInteger(difficulty));
            // target hash value = maximum hash value / difficulty

            return blockHashNumber < target;
            // If blockHashNumber is a number less than target, it's satisfied
        }
        public void Verify(IState previousState, int difficulty)
        {
            // This verification will be conducted by peers that recieved new mined block message
            // Before they add new block to their blockchain, they will verify the block
            // If it's not valid, they won't add block to their blockchain
            // If majority does not add block to blockchain,
            // next block that will be mined has low chance to indicate the invalid block as its previous block hash
            // So, invalid block will be forgotten from whole blockchain, as time goes by

            if (difficulty != this._difficulty)
            {
                throw new Exception("Block difficulty is different from difficulty that calculated from blockchain");
            }
            // If difficulty of new block is different from difficulty calculated from blockchain,
            // we consider new block has not been mined properly, and does not accept it as new member of blockchain

            if (!Satisfies(this.BlockHash, this._difficulty))
            {
                throw new Exception("Block hash does not matches difficulty");
            }
            // Block hash verification checking if it's smaller than target hash value

            foreach (Tx tx in this._transactions)
            {
                if (!tx.Verify())
                {
                    throw new Exception("Some of signatures in transactions are not valid");
                }
            }
            // Transaction verification if its signature is valid for given payload and public key

            IState newState = EvaluateActions(previousState, this._transactions);
            if (!(this.State.StateInts.SequenceEqual(newState.StateInts)))
            {
                throw new Exception("Evaluated state is different from block state");
            }

            // Actually, validation can be done by just reproducing block hash
            // injecting re-computed difficulty, previous state
            // For now, to find out where block has been corrupted, divided each cases
        }
        public static IState EvaluateActions(IState previousState, IEnumerable<Tx> txs)
        {
            IState state = previousState.Clone();
            foreach (Tx tx in txs)
            {
                foreach (IAction action in tx.Actions)
                {
                    state = action.Execute(state);
                }
            }
            // Get state from last block in block chain, apply all actions in mempool to this state
            // Generate next state and use it as state in new block to be mined
            // This state have to be same as state computed from each client
            return state;
        }
    }
}