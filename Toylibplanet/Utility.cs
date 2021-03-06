using System.Text;

namespace Toylibplanet
{
    public class Utility
    {
        public static string BytesToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
        public static byte[] DateTimeOffsetToBytes(DateTimeOffset timestamp)
        {
            return BitConverter.GetBytes(timestamp.Ticks).Concat(BitConverter.GetBytes(timestamp.Offset.Ticks)).ToArray();
            // Since data type of DateTimeOffset.Ticks is int64, timestamp tick (8 bytes) + offset tick (8 bytes) = total byte (16 bytes)
        }
        public static DateTimeOffset BytesToDateTimeOffset(byte[] timestampByte)
        {
            return new DateTimeOffset(BitConverter.ToInt64(timestampByte.Take(8).ToArray()), new TimeSpan(BitConverter.ToInt64(timestampByte.Skip(8).ToArray())));
            // Front 8 bytes are timestamp tick, later 8 bytes are offset tick
        }
        public static byte[] IntToBytes(Int32 integer)
        { 
            return BitConverter.GetBytes(integer).ToArray(); 
        }
        public static Int32 BytesToInt(byte[] bytes)
        { 
            return BitConverter.ToInt32(bytes, 0); 
        }
        public static byte[] StringToBytes(String str)
        { 
            return Encoding.Unicode.GetBytes(str); 
        }
        public static string BytesToString(byte[] bytes)
        { 
            return Encoding.Unicode.GetString(bytes); 
        }
    }
}
