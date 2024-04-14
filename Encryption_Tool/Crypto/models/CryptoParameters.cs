using Encryption_Tool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Encryption_Tool.EncryptionEngine.models
{
    public class CryptoParameters
    {
        public RSA RSA { get; set; }
        public RSAEncryptionPadding RSAEncryptionPadding { get; set; } = RSAEncryptionPadding.CreateOaep(HashAlgorithmName.SHA256);
        public Aes? Aes { get; set; }
        public bool GetParameters(out Aes aes)
        {
            aes = Aes;
			if (Aes == null)
				return false;
            return true;
        }
        public bool GetParameters(out RSA rsa, out RSAEncryptionPadding padding)
        {
            rsa = RSA;
            padding = RSAEncryptionPadding;
            if (RSA == null)
                return false;
            return true;
        }
    }
}
