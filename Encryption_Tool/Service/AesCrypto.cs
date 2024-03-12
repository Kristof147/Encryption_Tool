using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Controls;

namespace Encryption_Tool.Service
{
	public static class AesCrypto
	{
		#region Encrypt and Decrypt
		public static byte[] EncryptText(Aes aes, string plainText)
		{
			byte[] encryptedData = [];
			if (CheckArguments(aes, plainText))
			{
				plainText = Coding.Base64Encode(plainText);
				using (Aes myAes = Aes.Create())
				{
					myAes.Key = aes.Key;
					myAes.IV = aes.IV;
					ICryptoTransform encryptor = myAes.CreateEncryptor(myAes.Key, myAes.IV);
					using (MemoryStream msEncrypt = new MemoryStream())
					{
						using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
						{
							using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
							{
								swEncrypt.Write(plainText);
							}
							encryptedData = msEncrypt.ToArray();
						}
					}
				}
			}
			return encryptedData;
		}

		public static string DecryptText(Aes aes, byte[] cipherText)
		{
			string plainText = string.Empty;
			if (CheckArguments(aes, cipherText))
			{
				using (Aes myAes = Aes.Create())
				{
					myAes.Key = aes.Key;
					myAes.IV = aes.IV;
					ICryptoTransform decryptor = myAes.CreateDecryptor(myAes.Key, aes.IV);
					using (MemoryStream msDecrypt = new MemoryStream(cipherText))
					{
						using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
						{
							using (StreamReader srDecrypt = new StreamReader(csDecrypt))
							{
								plainText = srDecrypt.ReadToEnd();
							}
						}
					}
				}
			}
			plainText = Coding.Base64Decode(plainText);
			return plainText;
		}

		private static bool CheckArguments<T>(Aes aes, T data) where T : IEnumerable
		{
			if (data == null || !data.Cast<object>().Any())
				throw new ArgumentNullException(nameof(data));
			if (aes.Key == null || aes.Key.Length <= 0)
				throw new ArgumentNullException(nameof(aes.Key));
			if (aes.IV == null || aes.IV.Length <= 0)
				throw new ArgumentNullException(nameof(aes.IV));
			return true;
		}

		#endregion

		#region Encode and Decode an Image
		public static byte[] DecodeImage(Aes aes, string imagePath)
		{
			byte[] imageArray = File.ReadAllBytes(imagePath);
			string base64Image = Convert.ToBase64String(imageArray);
			return(EncryptText(aes, base64Image));
		}
		public static System.Drawing.Image EncodeImage(Aes aes, byte[] imgData)
		{
			string base64Image = DecryptText(aes, imgData);
			var img = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(base64Image)));
			return img;
		}
		#endregion

		#region Folder selection

		#endregion
	}
}
