using Encryption_Tool.EncryptionEngine.Encryptors;
using Encryption_Tool.EncryptionEngine.models;
using Encryption_Tool.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Encryption_Tool.Crypto.Cryptors
{
	internal class AESCryptor : BaseCryptor
	{
		public override EncryptionResult Encrypt(EncryptionRequest request)
		{
			EncryptionResult result = new();
			if (request.Parameters.GetParameters(out Aes aes))
			{
				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
				using (MemoryStream msEncrypt = new())
				{
					using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						csEncrypt.Write(request.DataToEncrypt, 0, request.DataToEncrypt.Length);
						csEncrypt.FlushFinalBlock();
						result.Data = msEncrypt.ToArray();
					}
				}
			}
			return result;
		}

		public override DecryptionResult Decrypt(DecryptionRequest request)
		{
			DecryptionResult result = new();
			string text = string.Empty;
			if (request.Parameters.GetParameters(out Aes aes))
			{
				try
				{
					using (aes)
					{
						ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
						using (MemoryStream msDecrypt = new(request.DataToDecrypt))
						{
							using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
							{
								using (MemoryStream msPlain = new())
								{
									csDecrypt.CopyTo(msPlain);
									result.Data = msPlain.ToArray();
								}
							}
						}
					}
				}
				catch (CryptographicException ex)
				{
					MessageBox.Show("Er is een fout opgetreden bij het decrypteren: " + ex.Message);
				}

			}
			return result;
		}
	}
}
