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
		static void GenerateRSAKey(out string publickey, out string privatekey)
		{
			var rsa = new RSACryptoServiceProvider();
			publickey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
			privatekey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

		}
		public static void GenerateAESKey(string path, string fileName)
		{
			// Create an instance of the AES crypto provider
			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
			{
				// Generate a random 256-bit key
				aes.GenerateKey();
				aes.GenerateIV();

				// Serialize the key and iv to XML
				using (TextWriter writer = new StreamWriter(@$"{path}\{fileName}_key.xml"))
				{
					XmlSerializer keySerializer = new XmlSerializer(typeof(byte[]));
					keySerializer.Serialize(writer, aes.Key);
				}
				using (StreamWriter writer = new($@"{path}\{fileName}_iv.xml"))
				{
					XmlSerializer ivSerializer = new(typeof(byte[]));
					ivSerializer.Serialize(writer, aes.IV);
				}
			}

			Console.WriteLine($"AES key and IV generated and saved to {fileName}_key.xml and {fileName}_iv.xml.");
		}

		public static byte[] DeserializeAes(string value)
		{
			byte[] result;
			using (FileStream fs = new(value, FileMode.Open))
			{
				XmlSerializer serializer = new(typeof(byte[]));
				result = (byte[])serializer.Deserialize(fs);
			}
			return result;
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
