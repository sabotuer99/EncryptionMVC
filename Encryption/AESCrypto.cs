using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Encryption
{
    //Adapted from user3016854's answer on Stack Overflow
    //http://stackoverflow.com/questions/165808/simple-two-way-encryption-for-c-sharp/212707#22780423
    public class AESCrypto
    {
        byte[] Key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
        byte[] IV = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };


        public string Encrypt(string userID)
        {
            using (var cryptoProvider = new AesCryptoServiceProvider())
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(userID);
                writer.Flush();
                cryptoStream.FlushFinalBlock();
                //writer.Flush();
                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
        }


        public string Decrypt(string userID)
        {
            using (var cryptoProvider = new AesCryptoServiceProvider())
            using (var memoryStream = new MemoryStream(Convert.FromBase64String(userID)))
            using (var cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
            using (var reader = new StreamReader(cryptoStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}