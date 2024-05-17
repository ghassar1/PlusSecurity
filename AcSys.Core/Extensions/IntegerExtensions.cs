using System;

namespace AcSys.Core.Extensions
{
    public static class IntegerExtensions
    {
        public static Guid ToGuid(this int value)
        {
            //if (value < 0) throw new ApplicationException("A negative number cannot be converted to ");
            if (value < 0) value = (value + 1) * -1;
            
            byte[] bytes = new byte[16];
            //BitConverter.GetBytes(value).CopyTo(bytes, 0);
            //byte[] intBytes = BitConverter.GetBytes(value);
            //intBytes.CopyTo(bytes, bytes.Length - intBytes.Length);
            //return new Guid(bytes);

            string bytesString = new Guid(bytes).ToString();

            string intString = value.ToString();
            bytesString = bytesString.Remove(bytesString.Length - intString.Length);
            bytesString = bytesString + intString;
            return new Guid(bytesString);
        }
    }
}
