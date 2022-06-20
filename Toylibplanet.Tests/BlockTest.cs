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
            TestState state_original = (TestState)state.Clone();
            IEnumerable<TestAction> actions = new List<TestAction> { new TestAction(), new TestAction() };
            foreach(TestAction action in actions)
            {
                action.Execute(state);
            }
            Tx tx = new(
                privateKey,
                publicKey,
                actions);
            IEnumerable<Tx> transactions = new List<Tx> { tx };
            Block sampleBlock = new(index, difficulty, rewardBeneficiary, previousHash, state, transactions, 0);
            output.WriteLine(BitConverter.ToString(sampleBlock.BlockHash).Replace("-", ""));
            output.WriteLine(BitConverter.ToString(sampleBlock.Transactions.ElementAt(0).Signature).Replace("-", ""));

            output.WriteLine("Checking if block is valid");
            bool blockVerifyTest = false;
            try
            {
                sampleBlock.Verify(state_original, difficulty);
                output.WriteLine("\t => Block hash matches difficulty");
                output.WriteLine("\t => All signatures in transactions are valid");
                output.WriteLine("\t => Evaluated state(from previous block state) is same as last block state");
                blockVerifyTest = true;
            }
            catch
            {
                output.WriteLine("\tBlock is not valid");
            }
            sampleBlock.Verify(state_original, difficulty);
            Assert.True(blockVerifyTest);
            output.WriteLine("End of sample block test");
            Assert.True(sampleBlock.BlockHash.Length == 32);
        }

    }
}