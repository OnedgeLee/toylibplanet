using Libplanet.Crypto;
using System.Security.Cryptography;

namespace Toylibplanet
{
    public class Tx
    {
        private readonly PublicKey _publicKey;
        // Public key represents transaction generator

        private readonly IEnumerable<IAction> _actions;
        // Action is core information of transaction to be secured

        private readonly DateTimeOffset _timestamp;
        // Timestamp is used for aligning transactions in sequence
        // If other purpose exists, please let me know

        private readonly byte[] _signature;
        // Signature is used for verifying transaction
        // Signature is generated from private key and payload
        // With signature, we can secure if public key and payload is not currupted
        // With signature, we can secure if transaction is generated from proper private key

        public IEnumerable<IAction> Actions { get => _actions; }
        public PublicKey PublicKey { get => _publicKey; }
        public byte[] Signature { get => _signature; }

        public Tx(
            PrivateKey privateKey,
            PublicKey publicKey,
            IEnumerable<IAction> actions)
        {
            this._publicKey = publicKey;
            this._actions = actions;
            this._timestamp = DateTimeOffset.UtcNow;
            this._signature = Sign(privateKey);
        }
        public Tx()
        // Null Transaction
        {
            byte[] nullBytes = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                nullBytes[i] = 0x01;
            }
            // Cannot be initialized with 0x00, because ECDSA won't use 0 as private key
            // It's because elliptic curve point multiplication is not defined for 0 multiplication.
            PrivateKey nullPrivateKey = new(nullBytes);
            this._publicKey = nullPrivateKey.PublicKey;
            this._actions = new List<IAction> { new NullAction() };
            this._timestamp = DateTimeOffset.UtcNow;
            this._signature = Sign(nullPrivateKey);
        }

        public byte[] Id
        {
            get
            {
                SHA256 hasher = SHA256.Create();
                byte[] payload = Payload();
                return hasher.ComputeHash(payload);
                // txId = SHA256(Tx)
            }
        }
        // It's just used for hash table of transactions
        // TxId secures if proper transaction is on proper storage address

        public byte[] Payload()
        {
            byte[] publicKeyBytes = this._publicKey.Format(true);
            IEnumerable<byte[]> actionBytes = from action in this._actions select action.Serialize();
            byte[] actionsBytes = new byte[(from actionByte in actionBytes select actionByte.Length).Sum()];
            int actionsBytesOffset = 0;
            foreach (byte[] actionByte in actionBytes)
            {
                Buffer.BlockCopy(actionByte, 0, actionsBytes, actionsBytesOffset, actionByte.Length);
                actionsBytesOffset += actionByte.Length;
            }

            byte[] timestampBytes = BitConverter.GetBytes(this._timestamp.Ticks)
                .Concat(BitConverter.GetBytes((Int16)this._timestamp.Offset.TotalMinutes)).ToArray();

            byte[] publicKeyBytesSize = BitConverter.GetBytes(publicKeyBytes.Length);
            byte[] actionsBytesSize = BitConverter.GetBytes(actionsBytes.Length);
            byte[] timestampBytesSize = BitConverter.GetBytes(timestampBytes.Length);

            byte[] payload = publicKeyBytesSize.Concat(actionsBytesSize).Concat(timestampBytesSize).
                Concat(publicKeyBytes).Concat(actionsBytes).Concat(timestampBytes).ToArray();

            return payload;
        }

        public byte[] Serialize()
        {
            return Payload().Concat(_signature).ToArray();
        }
        public byte[] Sign(PrivateKey privateKey)
        {
            return privateKey.Sign(Payload());
        }
        // Generate signature with transaction generator's private key and payload
        public bool Verify()
        {
            return this.PublicKey.Verify(Payload(), this.Signature);
        }
        // Verify if signature is valid for its public key and payload
    }
}