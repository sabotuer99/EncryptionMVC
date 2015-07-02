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
            return cipherText;
        }

        public String StreamEncrypt(String plainText)
        {
            var textStream = new MemoryStream();

            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.Padding = PaddingMode.ISO10126;

            byte[] Key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
            byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            //Create a CryptoStream, pass it the NetworkStream, and encrypt 
            //it with the Rijndael class.
            CryptoStream CryptStream = new CryptoStream(textStream,
            RMCrypto.CreateEncryptor(Key, IV),
            CryptoStreamMode.Write);

            //Create a StreamWriter for easy writing to the 
            //network stream.
            StreamWriter SWriter = new StreamWriter(CryptStream);
            SWriter.Flush();
            CryptStream.FlushFinalBlock();
            textStream.Position = 0;

            var encrypted = Convert.ToBase64String(textStream.GetBuffer(), 0, (int)textStream.Length);

            //var encStream = new MemoryStream(Convert.FromBase64String(encrypted));

            //CryptoStream DecryptStream = new CryptoStream(textStream, 
            //RMCrypto.CreateDecryptor(Key, IV), 
            //CryptoStreamMode.Read);

            //StreamReader SReader = new StreamReader(DecryptStream);

            //var decrypted = SReader.ReadToEnd();
            string result;
            using (var input = new MemoryStream(Convert.FromBase64String(encrypted)))
            using (var output = new MemoryStream())
            {
                var decryptor = RMCrypto.CreateDecryptor(Key, IV);
                using (var cryptStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
                {
                    var buffer = new byte[1024];
                    var read = cryptStream.Read(buffer, 0, buffer.Length);
                    while (read > 0)
                    {
                        output.Write(buffer, 0, read);
                        read = cryptStream.Read(buffer, 0, buffer.Length);
                    }
                    cryptStream.Flush();
                    result = Encoding.Unicode.GetString(output.ToArray());
                }
            }

            return encrypted + "  |  " + result;
        }
    }
}