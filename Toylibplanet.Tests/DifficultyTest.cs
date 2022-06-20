using Xunit;
using Xunit.Abstractions;
using Libplanet.Crypto;

namespace Toylibplanet.Tests
{
    public class DifficultyTest
    {
        private readonly ITestOutputHelper output;
        public DifficultyTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void SolveTest()
        {
            byte[] payload = new byte[100];
            Random random = new(0);
            random.NextBytes(payload);

            DateTimeOffset time1 = DateTimeOffset.UtcNow;
            Block.Solve(payload, 1, 0);
            
            DateTimeOffset time2 = DateTimeOffset.UtcNow;
            Block.Solve(payload, 10000, 0);

            DateTimeOffset time3 = DateTimeOffset.UtcNow;
            Block.Solve(payload, 1, 0);

            DateTimeOffset time4 = DateTimeOffset.UtcNow;
            Block.Solve(payload, 10000, 0);

            DateTimeOffset time5 = DateTimeOffset.UtcNow;
            Block.Solve(payload, 1, 0);

            DateTimeOffset time6 = DateTimeOffset.UtcNow;

            output.WriteLine("difficulty:1\ttime:" + (time2 - time1).TotalMilliseconds.ToString());
            output.WriteLine("difficulty:10000\ttime:" + (time3 - time2).TotalMilliseconds.ToString());
            output.WriteLine("difficulty:1\ttime:" + (time4 - time3).TotalMilliseconds.ToString());
            output.WriteLine("difficulty:10000\ttime:" + (time5 - time4).TotalMilliseconds.ToString());
            output.WriteLine("difficulty:1\ttime:" + (time6 - time5).TotalMilliseconds.ToString());
        }   

    }
}