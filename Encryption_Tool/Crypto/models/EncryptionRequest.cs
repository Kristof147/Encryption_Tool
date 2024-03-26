using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encryption_Tool.EncryptionEngine.models
{
    public class EncryptionRequest : BaseRequest
    {
        public byte[] DataToEncrypt { get; set; }
    }
}
