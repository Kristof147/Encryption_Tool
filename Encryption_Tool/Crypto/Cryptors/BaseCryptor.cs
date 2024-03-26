using Encryption_Tool.EncryptionEngine.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encryption_Tool.EncryptionEngine.Encryptors
{
    internal class BaseCryptor
    {
        public virtual EncryptionResult Encrypt(EncryptionRequest request)
        {
            throw new NotImplementedException();
        }
        public virtual DecryptionResult Decrypt(DecryptionRequest request)
        {
            throw new NotImplementedException();
        }

    }
}
