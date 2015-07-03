using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.IO;
using System.Text;


namespace Encryption.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public String Encrypt(String plainText)
        {
            SimplerAES saes = new SimplerAES();
            var cipherText = saes.Encrypt(plainText);
            return cipherText + " | " + saes.Decrypt(cipherText);
        }

        public String EncryptToo(String plainText)
        {
            AESCrypto aes = new AESCrypto();
            var cipherText = aes.Encrypt(plainText);
            return cipherText + " | " + aes.Decrypt(cipherText);
        }

        public String StreamEncrypt(String plainText)
        {
            var textStream = new MemoryStream();

            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.Mode = CipherMode.CBC;
            RMCrypto.Padding = PaddingMode.ISO10126;

            byte[] Key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
            byte[] IV = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };

            //Create a CryptoStream, pass it the NetworkStream, and encrypt 
            //it with the Rijndael class.
            CryptoStream CryptStream = new CryptoStream(textStream,
            RMCrypto.CreateEncryptor(Key, IV),
            CryptoStreamMode.Write);

            //Create a StreamWriter for easy writing to the 
            //network stream.
            StreamWriter SWriter = new StreamWriter(CryptStream);
            SWriter.WriteLine(plainText);
            SWriter.Flush();
            CryptStream.FlushFinalBlock();

            var encrypted = Convert.ToBase64String(textStream.GetBuffer(), 0, (int)textStream.Length);

            var encStream = new MemoryStream(Convert.FromBase64String(encrypted));

            //THIS WORKS
            //CryptoStream DecryptStream = new CryptoStream(encStream, 
            //    RMCrypto.CreateDecryptor(Key, IV), 
            //    CryptoStreamMode.Read);

            textStream.Position = 0;
            CryptoStream DecryptStream = new CryptoStream(textStream,
                RMCrypto.CreateDecryptor(Key, IV),
                CryptoStreamMode.Read);

            StreamReader SReader = new StreamReader(DecryptStream);
            string result = SReader.ReadToEnd();

            //var decrypted = SReader.ReadToEnd();
            //using (var input = new MemoryStream(Convert.FromBase64String(encrypted)))
            //using (var output = new MemoryStream())
            //{
            //    var decryptor = RMCrypto.CreateDecryptor(Key, IV);
            //    using (var cryptStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
            //    {
            //        var buffer = new byte[1024];
            //        var read = cryptStream.Read(buffer, 0, buffer.Length);
            //        while (read > 0)
            //        {
            //            output.Write(buffer, 0, read);
            //            read = cryptStream.Read(buffer, 0, buffer.Length);
            //        }
            //        cryptStream.Flush();
            //        result = Encoding.Unicode.GetString(output.ToArray());
            //    }
            //}

            return encrypted + "  |  " + result;
        }
    }
}