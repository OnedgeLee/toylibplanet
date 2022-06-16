using Xunit;
using Xunit.Abstractions;
using Toylibplanet;
using Libplanet.Crypto;
using System;
using Toylibplanet.Tests;

namespace Toylibplanet.Tests;

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
        output.WriteLine(tx.PublicKey.ToString());
        output.WriteLine(BitConverter.ToString(tx.Signature).Replace("-",""));
        string message = "End of NullTxTest";
        output.WriteLine(message);
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
        output.WriteLine(tx.PublicKey.ToString());
        output.WriteLine(BitConverter.ToString(tx.Signature).Replace("-", ""));
        output.WriteLine(tx.Actions.ElementAt(0).ActionName);
        output.WriteLine(tx.PublicKey.Verify(tx.Serialize(), tx.Signature).ToString());
        string message = "End of SampleTxTest";
        output.WriteLine(message);
    }
}