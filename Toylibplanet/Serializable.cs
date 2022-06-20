using System;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Toylibplanet
{
    public class Serializable
    {
        // public byte[] Serialize()
        // {
        //     FieldInfo[] fields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        // 
        //     byte[] fieldBytes = Array.Empty<byte>();
        // 
        //     foreach (FieldInfo field in fields)
        //     {
        //         string fieldType = field.FieldType.ToString();
        //         string fieldName = field.Name;
        //         var fieldValue = field.GetValue(this);
        //         byte[] fieldTypeBytes = Utility.StringToBytes(fieldType);
        //         byte[] fieldTypeBytesSize = Utility.IntToBytes(fieldTypeBytes.Length);
        //         byte[] fieldNameBytes = Utility.StringToBytes(fieldName);
        //         byte[] fieldNameBytesSize = Utility.IntToBytes(fieldNameBytes.Length);
        //         byte[] fieldValueBytes = Array.Empty<byte>();
        //         if (fieldType == "System.String")
        //         { fieldValueBytes = Utility.StringToBytes((string)fieldValue); }
        //         if (fieldType == "System.Int32")
        //         { fieldValueBytes = Utility.IntToBytes((int)fieldValue); }
        //         if (fieldType == "System.DateTimeOffset")
        //         { fieldValueBytes = Utility.DateTimeOffsetToBytes((DateTimeOffset)fieldValue); }
        //         byte[] fieldValueBytesSize = BitConverter.GetBytes(fieldValueBytes.Length);
        //         fieldBytes = fieldBytes.Concat(fieldTypeBytesSize).Concat(fieldTypeBytes).
        //             Concat(fieldNameBytesSize).Concat(fieldNameBytes).
        //             Concat(fieldValueBytesSize).Concat(fieldValueBytes).ToArray();
        //     }
        //     return fieldBytes;
        // }
        // 
        // public Deserialize(byte[] fieldBytes)
        // {
        // 
        //     int skip = 0;
        //     int fieldTypeBytesSize = Utility.BytesToInt(fieldBytes.Skip(skip).Take(4).ToArray());
        //     skip += 4;
        //     string fieldType = Utility.BytesToString(fieldBytes.Skip(skip).Take(fieldTypeBytesSize).ToArray());
        //     skip += fieldTypeBytesSize;
        //     int fieldNameByteSize = Utility.BytesToInt(fieldBytes.Skip(skip).Take(4).ToArray());
        //     skip += 4;
        //     string fieldName = Utility.BytesToString(fieldBytes.Skip(skip).Take(fieldNameByteSize).ToArray());
        //     skip += fieldNameByteSize;
        //     int fieldValueByteSize = Utility.BytesToInt(fieldBytes.Skip(skip).Take(4).ToArray());
        //     skip += 4;
        //     if (fieldType == "System.String")
        //     {
        //         string fieldValue = Utility.BytesToString(fieldBytes.Skip(skip).Take(fieldValueByteSize).ToArray());
        //     }
        //     if (fieldType == "System.Int32")
        //     {
        //         int fieldValue = Utility.BytesToInt(fieldBytes.Skip(skip).Take(fieldValueByteSize).ToArray());
        //     }
        //     if (fieldType == "System.DateTimeOffset")
        //     {
        //         DateTimeOffset fieldValue = Utility.BytesToDateTimeOffset(fieldBytes.Skip(skip).Take(fieldValueByteSize).ToArray());
        //     }
        // }
    }
}
