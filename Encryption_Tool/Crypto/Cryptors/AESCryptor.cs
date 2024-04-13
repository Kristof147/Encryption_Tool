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
				aes.GenerateIV();
				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
				using (MemoryStream msEncrypt = new())
				{
					msEncrypt.Write(aes.IV, 0, aes.IV.Length);

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
			byte[] iv = new byte[16];
			Array.Copy(request.DataToDecrypt, 0, iv, 0, 16);

			DecryptionResult result = new();
			if (request.Parameters.GetParameters(out Aes aes))
			{
				aes.IV = iv;
				try
				{
					using (aes)
					{
						ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
						using (MemoryStream msDecrypt = new(request.DataToDecrypt, 16, request.DataToDecrypt.Length - 16))
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
					result.Errors.Add(ex.Message);
				}

			}
			return result;
		}
	}
}
