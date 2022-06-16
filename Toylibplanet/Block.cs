using System;
using Libplanet.Crypto;
using Libplanet;
using Bencodex;
using System.Security.Cryptography;
using System.Linq;
using System.Numerics;

namespace Toylibplanet
{
    public class Block
    {

        private readonly int _index;
        private readonly int _difficulty;
        private readonly byte[] _nonce;
        private readonly byte[] _rewardBeneficiary;
        private readonly byte[] _previousHash;
        private readonly byte[] _blockHash;
        private readonly IEnumerable<Tx> _transactions;
        private readonly IState _state;
        private readonly DateTimeOffset _timestamp;

        public byte[] Nonce { get => _nonce; }
        public byte[] BlockHash { get => _blockHash; }
        public byte[] PreviousHash { get => _previousHash; } 
        public IState State { get => _state; }
        public IEnumerable<Tx> Transactions { get => _transactions; }

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
            (this._nonce, this._blockHash) = Solve(Payload(), this._difficulty, seed);
        }
        public Block(IState initialState)
         // Null block for genesis block
         // I think it's better to declare genesis block class, but for convinience, implemented like this
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
            (this._nonce, this._blockHash) = Solve(Payload(), this._difficulty, seed);
        }

        public byte[] Payload()
        // Actually, serialization is not needed for single-node condition (without network),
        // but at least, payload generation is needed for PoW.
        // It's not implemented in detail, but basic idea is to add fixed length bytes alone,
        // and add varied length bytes with its length bytes ahead.
        {
            byte[] indexBytes = BitConverter.GetBytes(this._index);
            byte[] difficultyBytes = BitConverter.GetBytes(this._difficulty);
            byte[] timestampBytes = BitConverter.GetBytes(this._timestamp.Ticks).
                Concat(BitConverter.GetBytes((Int16)this._timestamp.Offset.TotalMinutes)).ToArray();
            byte[] stateBytes = this._state.Serialize();
            IEnumerable <byte[]> txBytes = from tx in this._transactions select tx.Serialize();
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
            // [indexBytes (4byte)] [difficultyBytes (4byte)] [rewardBeneficiary (32byte)]
            // [previousHash (32byte)] [timestampBytes (4byte)] [stateBytes (?byte)] [txBytes (?byte)]
            return payload;
        }
        public byte[] Serialize()
        {
            return Payload().Concat(this._nonce).Concat(this._blockHash).ToArray();
            // Actually, if we don't use message on network, serialization is not needed
        }
 
        private static (byte[], byte[]) Solve(
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
                    return (nonceBytes, blockHash);
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
        public void Verify()
        {
            byte[] blockHash = HashBlock(Payload(), this._nonce);
            if (!blockHash.SequenceEqual(this._blockHash))
            {
                throw new Exception("Computed block hash is different from recorded block hash");
            }
            // Block hash verification with payload and nonce

            if (!Satisfies(blockHash, this._difficulty))
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
        }
    }
}