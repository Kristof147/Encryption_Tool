using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Encryption_Tool.Service
{
    public static class KeyHelper
    {
        // Genereer een RSA-sleutelpaar
        public static void GenerateRSAKey(out string publicKey, out string privateKey)
        {
            // Maak een nieuwe instantie van RSACryptoServiceProvider
            using (var rsa = new RSACryptoServiceProvider())
            {
                // Exporteer de openbare en privésleutels naar Base64-stringrepresentaties
                publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
            }
        }

        // Genereer een AES-sleutel en sla deze op in een XML-bestand
        public static void GenerateAESKey(string path, string fileName)
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                // Genereer een willekeurige AES-sleutel
                aes.GenerateKey();

                // Creëer het pad naar het XML-bestand waarin de AES-sleutel wordt opgeslagen
                string filePath = Path.Combine(path, fileName + ".aeskey");

                // Schrijf de AES-sleutel naar het XML-bestand
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    new XmlSerializer(typeof(byte[])).Serialize(fs, aes.Key);
                }

                // Geef een bericht weer om te bevestigen dat de sleutel is gegenereerd en opgeslagen
                Console.WriteLine($"AES key generated and saved to {filePath}.");
            }
        }

        // Deserialiseer een AES-sleutel uit een XML-bestand
        public static byte[] DeserializeAes(string filePath)
        {
            byte[] result;
            // Lees het XML-bestand en deserialiseer de AES-sleutel
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                result = (byte[])new XmlSerializer(typeof(byte[])).Deserialize(fs);
            }
            return result;
        }

        // Genereer RSA-sleutelpaar en sla ze op in XML-bestanden
        public static void SaveKeys(out string publicKey, out string privateKey)
        {
            publicKey = "";
            privateKey = "";
            var ofd = new SaveFileDialog()
            {
                FileName = "PublicKey",
                Filter = "txt files (*.txt)|*.txt"
            };
            if (ofd.ShowDialog() == true)
            {
                // Bepaal de map waarin de sleutels moeten worden opgeslagen
                string folderPath = Path.GetDirectoryName(ofd.FileName);
                string publicKeyPath = Path.Combine(folderPath, "publicKey.xml");
                string privateKeyPath = Path.Combine(folderPath, "privateKey.xml");

                // Genereer een RSA-sleutelpaar
                string publickey;
                string privatekey;
                GenerateRSAKey(out publickey, out privatekey);

                // Schrijf de RSA-sleutels naar XML-bestanden
                File.WriteAllText(publicKeyPath, publickey);
                File.WriteAllText(privateKeyPath, privatekey);

                // Wijs de gegenereerde sleutels toe aan de uitvoerparameters
                publicKey = publickey;
                privateKey = privatekey;
            }
        }

        // Genereer een RSA-sleutelpaar en sla ze op in XML-bestanden
        public static void GenerateRSAKeyPair(string path, string fileName, out string publicKey, out string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                // Exporteer de openbare en privésleutels naar XML-stringrepresentaties
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);

                // Creëer het pad naar de XML-bestanden waarin de sleutels worden opgeslagen
                string publicKeyFilePath = Path.Combine(path, fileName + "_public.xml");
                string privateKeyFilePath = Path.Combine(path, fileName + "_private.xml");

                // Schrijf de openbare sleutel naar een XML-bestand
                File.WriteAllText(publicKeyFilePath, publicKey);

                // Schrijf de privésleutel naar een XML-bestand
                File.WriteAllText(privateKeyFilePath, privateKey);
            }
        }

        public static void SerializeAes(string filePath, byte[] key, byte[] iv)
        {
            // Open een FileStream om het bestand te maken
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                // Gebruik BinaryWriter om de gegevens naar het bestand te schrijven
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    // Schrijf de lengte van de sleutel en IV naar het bestand
                    bw.Write(key.Length);
                    // Schrijf de sleutel zelf naar het bestand
                    bw.Write(key);
                    // Schrijf de lengte van de IV naar het bestand
                    bw.Write(iv.Length);
                    // Schrijf de IV zelf naar het bestand
                    bw.Write(iv);
                }
            }
        }

        // Deserialiseer de AES-sleutel en IV vanuit een binair bestand
        public static byte[] DeserializeAesFile(string filePath)
        {
            byte[] keyAndIV;
            // Open een FileStream om het bestand te lezen
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                // Gebruik BinaryReader om de gegevens van het bestand te lezen
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // Lees de lengte van de sleutel uit het bestand
                    int keyLength = br.ReadInt32();
                    // Lees de sleutel zelf uit het bestand
                    byte[] key = br.ReadBytes(keyLength);
                    // Lees de lengte van de IV uit het bestand
                    int ivLength = br.ReadInt32();
                    // Lees de IV zelf uit het bestand
                    byte[] iv = br.ReadBytes(ivLength);

                    // Combineer de sleutel en IV in een enkel byte-array
                    keyAndIV = new byte[keyLength + ivLength];
                    // Kopieer de sleutel naar de gecombineerde byte-array
                    Buffer.BlockCopy(key, 0, keyAndIV, 0, keyLength);
                    // Kopieer de IV naar de gecombineerde byte-array
                    Buffer.BlockCopy(iv, 0, keyAndIV, keyLength, ivLength);
                }
            }
            // Retourneer de gecombineerde byte-array met de sleutel en IV
            return keyAndIV;
        }
    }

}
