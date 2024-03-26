using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Encryption_Tool.EncryptionEngine.models
{
    public class DecryptionRequest : BaseRequest
    {
        public byte[] DataToDecrypt { get; set; }
    }
}
