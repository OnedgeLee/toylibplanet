using Xunit;
using Xunit.Abstractions;

namespace Toylibplanet.Tests
{
    public class UtilityTest
    {
        private readonly ITestOutputHelper output;
        public UtilityTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TimeConversionTest()
        {
            DateTimeOffset timestamp = DateTimeOffset.UtcNow;
            output.WriteLine("Original timestamp : " + timestamp.ToString());
            byte[] timestampBytes = Utility.DateTimeOffsetToBytes(timestamp);
            output.WriteLine("Byte encoding of timestamp : " + Utility.BytesToHex(timestampBytes));
            DateTimeOffset restoredTimeStamp = Utility.BytesToDateTimeOffset(timestampBytes);
            output.WriteLine("Restored timestamp : " + restoredTimeStamp.ToString());
            output.WriteLine("Check if restored time is same as original");
            output.WriteLine("\t : " + (timestamp == restoredTimeStamp).ToString());
        }
    }
}