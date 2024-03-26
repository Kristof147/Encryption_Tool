using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encryption_Tool.EncryptionEngine.models
{
    public class BaseRequest
    {
        /// <summary>
        /// Method for encryption and decryption
        /// </summary>
        public EncryptionType EncryptionType { get; set; }
        /// <summary>
        /// Parameters for Encryption and decryption
        /// </summary>
        public CryptoParameters Parameters { get; set; }

    }
    public enum EncryptionType
    {
        AES,
        RSA
    }
}
