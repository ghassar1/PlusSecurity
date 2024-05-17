using System;
using System.Security.Cryptography;
using System.Text;
using AcSys.Core.Extensions;

namespace AcSys.Core.AspNet
{
    /// <summary>
    /// Creates keys to use for encryption, decryption, and validation of Forms authentication cookie data. 
    /// You can use the keys that you create using this class for the validationKey and decryptionKey 
    /// attributes of the <machineKey> section in the <system.web> element in the Machine.config file.
    /// Reference: https://support.microsoft.com/en-us/kb/312906
    /// </summary>
    public static class MachineKeyGenerator
    {
        public static MachineKey Create(int decryptionKeyLength = 24, int validationKeyLength = 64)
        {
            string decryptionKey = CreateKey(decryptionKeyLength);
            string validationKey = CreateKey(validationKeyLength);

            string machineKeyTag = "<machineKey validationKey=\"{0}\" decryptionKey=\"{1}\" validation=\"SHA1\"/>"
                .FormatWith(validationKey, decryptionKey);

            Console.WriteLine(machineKeyTag);

            MachineKey key = new MachineKey()
            {
                DecryptionKey = decryptionKey,
                ValidationKey = validationKey,
                Tag = machineKeyTag
            };

            return key;
        }

        public static String CreateKey(int numBytes)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[numBytes];

            rng.GetBytes(buff);
            return BytesToHexString(buff);
        }

        public static String BytesToHexString(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(64);

            for (int counter = 0; counter < bytes.Length; counter++)
            {
                hexString.Append(String.Format("{0:X2}", bytes[counter]));
            }
            return hexString.ToString();
        }
    }

    public struct MachineKey
    {
        public string Tag;
        public string DecryptionKey;
        public string ValidationKey;
    };
}
