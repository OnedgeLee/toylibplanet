using Xunit;
using Xunit.Abstractions;
using Libplanet.Crypto;

namespace Toylibplanet.Tests
{
    public class TxTest
    {
        private readonly ITestOutputHelper output;
        public TxTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void NullTxTest()
        {
            Tx tx = new();
            byte[] nullBytes = new byte[32]; for (int i = 0; i < 32; i++) {nullBytes[i] = 0x01;}
            PrivateKey nullPrivateKey = new(nullBytes);
            output.WriteLine("Public Key : " + tx.PublicKey.ToString());
            output.WriteLine("Private Key : " + Utility.BytesToHex(nullPrivateKey.ToByteArray()));
            output.WriteLine("Payload : " + Utility.BytesToHex(tx.Payload()));
            output.WriteLine("Signature : " + Utility.BytesToHex(tx.Signature));
            output.WriteLine("Verify Transaction");
            bool txVerifyTest = tx.Verify();
            output.WriteLine("\t : " + txVerifyTest);
            Assert.True(txVerifyTest);
            output.WriteLine("End of NullTxTest");
        }
        [Fact]
        public void SampleTxTest()
        {
            PrivateKey privateKey = new();
            PublicKey publicKey = privateKey.PublicKey;
            IEnumerable<TestAction> actions = new List<TestAction> { new TestAction(), new TestAction() };
            Tx tx = new(
                privateKey,
                publicKey,
                actions);
            output.WriteLine("Public Key : " + tx.PublicKey.ToString());
            output.WriteLine("Private Key : " + Utility.BytesToHex(privateKey.ToByteArray()));
            output.WriteLine("Payload : " + Utility.BytesToHex(tx.Payload()));
            output.WriteLine("Signature : " + Utility.BytesToHex(tx.Signature));
            output.WriteLine("Verify Transaction");
            bool txVerifyTest = tx.Verify();
            output.WriteLine("\t : " + txVerifyTest);
            Assert.True(txVerifyTest);
            output.WriteLine("End of SampleTxTest");
        }
    }
}