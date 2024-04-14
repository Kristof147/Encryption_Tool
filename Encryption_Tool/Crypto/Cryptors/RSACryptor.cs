using Encryption_Tool.EncryptionEngine.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Encryption_Tool.EncryptionEngine.Encryptors
{
    internal class RSACryptor : BaseCryptor
    {
        public override EncryptionResult Encrypt(EncryptionRequest request)
        {
            EncryptionResult result = new();
            if (request.Parameters.GetParameters(out RSA rsa, out RSAEncryptionPadding padding))
            {
                try
                {
                    result.Data = rsa.Encrypt(request.DataToEncrypt, padding);
                }
                catch (CryptographicException ex)
                {
                    result.AddError(ex.Message);
                }
            }
            return result;
        }
        public override DecryptionResult Decrypt(DecryptionRequest request)
        {
            DecryptionResult result = new();
            if (request.Parameters.GetParameters(out RSA rsa, out RSAEncryptionPadding padding))
            {
                try
                {
                    result.Data = rsa.Decrypt(request.DataToDecrypt, padding);
                }
                catch (CryptographicException ex)
                {
                    result.AddError(ex.Message);
                }
            }
            return result;
        }

    }
}
