using Xunit;
using Xunit.Abstractions;
using Libplanet.Crypto;

namespace Toylibplanet.Tests
{
    public class BlockChainTest
    {
        private readonly ITestOutputHelper output;
        public BlockChainTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void BlockChainExecutionTest()
        {
            PrivateKey privateKey = new();
            PublicKey publicKey = privateKey.PublicKey;
            // User has his own keys

            IState state = new TestState();
            // State has been initialized

            BlockChain blockChain = new(state);
            // With its initial state, block chain with genesis block has been generated

            TestAction action1 = new();
            TestAction action2 = new();
            // User will make some actions

            IEnumerable<TestAction> actions = new List<TestAction> { action1, action2 };
            Tx tx = new(
                privateKey,
                publicKey,
                actions);
            // With generated actions, user will construct transaction, signing with his keys

            IEnumerable<Tx> transactions = new List<Tx> { tx };
            // Transactions will be gathered

            blockChain.Mine(publicKey.Format(true), transactions);
            // Miner will mine with gathered transactions

            state = action1.Execute(state);
            state = action2.Execute(state);
            // User will apply action to his state

            List<bool> stateCheckingList = new();
            for (int i = 0; i < state.StateInts.Count; i++)
            {
                stateCheckingList.Add(state.StateInts[i] == blockChain.LastBlock.State.StateInts[i]);
            }
            output.WriteLine("Checking if transitioned state in block is same as in user client");
            output.WriteLine("\t" + stateCheckingList.All(stateCheck => stateCheck).ToString());
            // Check if transitioned state in block is same as state applied to user client

            Block previousBlock = blockChain.Blocks[blockChain.LastBlock.PreviousHash];
            Block genesisBlock = blockChain.GenesisBlock;
            string previousBlockHashHex = BitConverter.ToString(genesisBlock.BlockHash).Replace("-", "");
            string genesisBlockHashHex = BitConverter.ToString(previousBlock.BlockHash).Replace("-", "");
            output.WriteLine("Checking if previous block of last block in 1 block added chain is genesis block");
            output.WriteLine("\t" + (previousBlockHashHex == genesisBlockHashHex).ToString());
            // Check if previous block is genesis block
            // Since single transaction has been added to genesis block, it have to be same

            output.WriteLine("Checking if last block is valid");
            try
            {
                blockChain.LastBlock.Verify(previousBlock.State, blockChain.Difficulty());
                output.WriteLine("\tLast block is valid");
            }
            catch
            {
                output.WriteLine("\tLast block is not valid");
            }
            // Verify last block
        }
    }
}