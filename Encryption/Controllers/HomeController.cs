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
            
            //Create the crypto provider, set the mode and padding, 
            //create a key and insertion vector
            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.Mode = CipherMode.CBC;
            RMCrypto.Padding = PaddingMode.ISO10126;
            byte[] Key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
            byte[] IV = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };

            //Create a CryptoStream, passing in the new MemoryStream 
            //and using the RijndaelManaged object to create a decryptor
            //finally setting the stream mode to write (you can't read AND write, it's either/or)
            var textStream = new MemoryStream();
            CryptoStream CryptStream = new CryptoStream(textStream,
                RMCrypto.CreateEncryptor(Key, IV),
                CryptoStreamMode.Write);

            //Create a StreamWrite for the CryptoStream, write the plainText
            //call Flush on the writer to clear the buffers and write the text to the CryptoStream
            //(which does the encryption) and then call FlushFinalBlock to clear the CryptoStream
            //buffers and write the encrypted bytes to the underlying MemoryStream
            StreamWriter SWriter = new StreamWriter(CryptStream);
            SWriter.WriteLine(plainText);
            SWriter.Flush();
            CryptStream.FlushFinalBlock();

            //Here I'm getting the underlying byte[] directly and converting it
            //to a Base64String, then I'm converting the string back into a byte[]
            //and passing it into a MemoryStream
            var encrypted = Convert.ToBase64String(textStream.GetBuffer(), 0, (int)textStream.Length);
            var encStream = new MemoryStream(Convert.FromBase64String(encrypted));

            //THIS WORKS... Create a decryption stream from the newly created stream of encrypted data
            //CryptoStream DecryptStream = new CryptoStream(encStream, 
            //    RMCrypto.CreateDecryptor(Key, IV), 
            //    CryptoStreamMode.Read);

            //THIS ALSO WORKS... Here I'm just recycling the existing stream of encrypted data.  Position
            //has to be set to 0 so you can read from the beginning again (writing puts the position at the end)
            textStream.Position = 0;
            CryptoStream DecryptStream = new CryptoStream(textStream,
                RMCrypto.CreateDecryptor(Key, IV),
                CryptoStreamMode.Read);

            //Create a StreamReader for the CryptoStream (however it was created)
            //and read back the string. Bytes flow from the underlying memory stream, 
            //decrypted by the CryptoStream, and returned as a string by the StreamReader
            StreamReader SReader = new StreamReader(DecryptStream);
            string result = SReader.ReadToEnd();

            return encrypted + "  |  " + result;
        }

        public String SignData(String plainText)
        {
            string message = "";
            //PublicKeyInfrastructure pki = new PublicKeyInfrastructure();
            Cryptograph crypto = new Cryptograph();
            //RSAParameters privateKey = crypto.GenerateKeys("simlanghoff@gmail.com");
            //RSAParameters publicKey = crypto.GetPublicKey("simlanghoff@gmail.com");
            //const string PlainText = "This is really sent by me, really!";

            //copied from MSDN article on generating keys
            //Generate a public/private key pair.
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            //Save the public key information to an RSAParameters structure.
            RSAParameters publicKey = RSA.ExportParameters(false);
            RSAParameters privateKey = RSA.ExportParameters(true);

            //This is an interesting exercise in asymmetric encryption, but really isn't 
            //needed for signing...
            //string encryptedText = Cryptograph.Encrypt(plainText, publicKey);
            //message += "<br/>This is the encrypted text: " + encryptedText + "<br/>";
            //string decryptedText = Cryptograph.Decrypt(encryptedText, privateKey);
            //message += "<br/>This is the decrypted text: " + decryptedText + "<br/>";
            //string messageToSign = encryptedText;

            string messageToSign = plainText;
            message += "<br/>This is the message: " + messageToSign + "<br/>";

            string signedMessage = Cryptograph.SignData(messageToSign, privateKey);

            message += "<br/>This is the signed message: " + signedMessage + "<br/>";

            //// Is this message really, really, REALLY sent by me?
            bool success = Cryptograph.VerifyData(messageToSign, signedMessage, publicKey);

            message += "<br/>Is this message really, really, REALLY sent by me? " + success;
            return message;
        }
    }
}