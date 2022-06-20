using Xunit;
using Xunit.Abstractions;
using Libplanet.Crypto;

namespace Toylibplanet.Tests
{
    public class ClientScenarioTest
    {
        private readonly ITestOutputHelper output;
        public ClientScenarioTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void FullExecutionTest()
        {

            IState state = new TestState();
            output.WriteLine("Global game state has been initialized");
            // State has been initialized

            PrivateKey privateKey = new();
            output.WriteLine("Private key has been generated");
            PublicKey publicKey = privateKey.PublicKey;
            output.WriteLine("Public key has been generated");
            // User has his own keys

            BlockChain blockChain = new(state);
            output.WriteLine("Genesis block has been generated and registered on new blockchain");
            // With its initial state, block chain with genesis block has been generated

            TestAction action1 = new();
            output.WriteLine("Client generated action 1");
            TestAction action2 = new();
            output.WriteLine("Client generated action 2");
            // User will make some actions

            IEnumerable<TestAction> actions = new List<TestAction> { action1, action2 };
            Tx tx = new(
                privateKey,
                publicKey,
                actions);
            output.WriteLine("Client generated transaction with action 1 and action 2, signing with his private key");
            // With generated actions, user will construct transaction, signing with his keys

            IEnumerable<Tx> transactions = new List<Tx> { tx };
            output.WriteLine("Transactions has been gathered on mempool");
            // Transactions will be gathered

            blockChain.Mine(publicKey.Format(true), transactions);
            output.WriteLine("Miner mined block with transactions on mempool, and added to blockchain");
            // Miner will mine with gathered transactions

            output.WriteLine("Client will recieve message of updated blockchain");
            state = action1.Execute(state);
            output.WriteLine("Client applied action1 to its state");
            state = action2.Execute(state);
            output.WriteLine("Client applied action2 to its state");
            // User will apply action to his state

            output.WriteLine("Checking if transitioned state in block is same as in user client");
            bool stateTransitionTest = state.StateInts.SequenceEqual(blockChain.LastBlock.State.StateInts);
            
            output.WriteLine("\t : " + stateTransitionTest.ToString());
            Assert.True(stateTransitionTest);
            // Check if transitioned state in block is same as state applied to user client

            output.WriteLine("Checking if previous block of last block in 1 block added chain is genesis block");
            Block previousBlock = blockChain.Blocks[Utility.BytesToHex(blockChain.LastBlock.PreviousHash)];
            Block genesisBlock = blockChain.GenesisBlock;
            bool blockSequenceTest = (Utility.BytesToHex(previousBlock.BlockHash) == Utility.BytesToHex(genesisBlock.BlockHash));
            
            output.WriteLine("\t : " + blockSequenceTest.ToString());
            Assert.True(blockSequenceTest);
            // Check if previous block is genesis block
            // Since single transaction has been added to genesis block, it have to be same

            output.WriteLine("Checking if last block is valid");
            bool lastBlockTest = false;
            try
            {
                Block lastBlock = blockChain.LastBlock;
                IState lastState = previousBlock.State;
                blockChain.Pop();
                lastBlock.Verify(lastState, blockChain.Difficulty());
                output.WriteLine("\t : Last block is valid");
                output.WriteLine("\t => Block difficulty is same as difficulty that calculated from the blockchain");
                output.WriteLine("\t => Block hash matches difficulty");
                output.WriteLine("\t => All signatures in transactions are valid");
                output.WriteLine("\t => Evaluated state(from previous block state) is same as last block state");
                // Since serialization is not supported yet, workaround applied
                // Suppose verifying user is a user whose blockchain is 1 block before
                // Generated this user condition popping out last block

                lastBlockTest = true;
            }
            catch
            {
                output.WriteLine("\tLast block is not valid");
            }
            // Verify last block

            Assert.True(lastBlockTest);

            output.WriteLine("End of scenario test");
        }
    }
}