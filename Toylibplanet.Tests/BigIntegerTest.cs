using Xunit;
using Xunit.Abstractions;
using System.Numerics;

namespace Toylibplanet.Tests
{
    public class BigIntegerTest
    {
        private readonly ITestOutputHelper output;
        public BigIntegerTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void BigIntegerConvTest()
        {

            byte[] maxTargetBytes = new byte[33];
            maxTargetBytes[32] = 0x01;
            // When generating BigInteger with byte array, byte array will be reversed
            // Why not 2^256 instead of 2^256 - 1? Still not sure

            BigInteger maxTarget = new(maxTargetBytes);
            output.WriteLine(maxTarget.ToString());

            BigInteger direct = BigInteger.Pow(new BigInteger(2), 256);
            output.WriteLine(direct.ToString());

            BigInteger target = BigInteger.Divide(maxTarget, new BigInteger(100000));
            output.WriteLine(target.ToString());
        }
    }
}