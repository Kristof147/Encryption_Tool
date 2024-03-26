using Encryption_Tool.Crypto.Cryptors;
using Encryption_Tool.EncryptionEngine.Encryptors;
using Encryption_Tool.EncryptionEngine.models;

namespace Encryption_Tool.EncryptionEngine
{
    internal class CryptoEngine
    {
        /// <summary>
        /// This is used to encrypt the data
        /// </summary>
        /// <param name="request">All the necessary data for adding encryption</param>
        /// <returns></returns>
        public EncryptionResult Encrypt(EncryptionRequest request)
        {
            BaseCryptor encryptor = GetEncryptor(request.EncryptionType);
            return encryptor.Encrypt(request);
        }
        public DecryptionResult Decryption(DecryptionRequest request)
        {
            BaseCryptor encryptor = GetEncryptor(request.EncryptionType);
            return encryptor.Decrypt(request);
        }
        private BaseCryptor GetEncryptor(EncryptionType type)
        {
            return type switch
            {
                EncryptionType.RSA => new RSACryptor(),
                EncryptionType.AES => new AESCryptor(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
