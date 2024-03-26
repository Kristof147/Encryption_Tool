using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Encryption_Tool.Service
{
    public class KeyHelper
    {
        static void GenerateRSAKey(out string publickey, out string privatekey)
        {
            var rsa = new RSACryptoServiceProvider();
            publickey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
            privatekey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

        }
        static void GenerateAESKey(string[] args)
        {
            // Create an instance of the AES crypto provider
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                // Generate a random 256-bit key
                aes.GenerateKey();

                // Serialize the key to XML
                XmlSerializer serializer = new XmlSerializer(typeof(byte[]));
                using (TextWriter writer = new StreamWriter("aeskey.xml"))
                {
                    serializer.Serialize(writer, aes.Key);
                }
            }

            Console.WriteLine("AES key generated and saved to aeskey.xml.");
        }

        public static void SaveKeys(out string aPublicKey, out string aPrivateKey)
        {
            aPublicKey = "";
            aPrivateKey = "";
            var ofd = new SaveFileDialog()
            {
                FileName = "PublicKey",
                Filter = "txt files (*.txt)|*.txt"
            };
            if (ofd.ShowDialog() == true)
            {
                string folderPath = Path.GetDirectoryName(ofd.FileName);
                string publicKeyPath = Path.Combine(folderPath, "publicKey.xml");
                string privateKeyPath = Path.Combine(folderPath, "privateKey.xml");
                string publickey;
                string privatekey;
                GenerateRSAKey(out publickey, out privatekey);
                using (var sw = File.CreateText(publicKeyPath))
                    sw.Write(publickey);
                using (var sw = File.CreateText(privateKeyPath))
                    sw.Write(privatekey);
                aPublicKey = publickey;
                aPrivateKey = privatekey;
            }
        }
    }
}
