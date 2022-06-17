using Xunit;
using Xunit.Abstractions;
using Libplanet.Crypto;

namespace Toylibplanet.Tests
{
    public class BlockTest
    {
        private readonly ITestOutputHelper output;
        public BlockTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void NullBlockTest()
        {
            TestState state = new();
            Block block = new(state);
            output.WriteLine("Block hash : " + Utility.BytesToHex(block.BlockHash));
            output.WriteLine("State array : " + string.Join(", ", block.State.StateInts));   
        }
        [Fact]
        public void SampleBlockTest()
        {
            PrivateKey privateKey = new();
            PublicKey publicKey = privateKey.PublicKey;
            int index = 0;
            int difficulty = 1;
            byte[] rewardBeneficiary = publicKey.Format(true);
            byte[] previousHash = new byte[32];
            TestState state = new();
            IEnumerable<TestAction> actions = new List<TestAction> { new TestAction(), new TestAction() };
            Tx tx = new(
                privateKey,
                publicKey,
                actions);
            IEnumerable<Tx> transactions = new List<Tx> { tx };
            Block sampleBlock = new(index, difficulty, rewardBeneficiary, previousHash, state, transactions);
            output.WriteLine(BitConverter.ToString(sampleBlock.BlockHash).Replace("-", ""));
            output.WriteLine(sampleBlock.State.StateInts[0].ToString());
            output.WriteLine(BitConverter.ToString(sampleBlock.Transactions.ElementAt(0).Signature).Replace("-", ""));
        }

    }
}