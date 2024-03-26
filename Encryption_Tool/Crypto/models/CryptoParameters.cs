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
        public RSAParameters? RSAParameters { get; set; }
        public RSAEncryptionPadding? RSAEncryptionPadding { get; set; }
        //public byte[]? Key { get; set; }
        //public byte[]? IV { get; set; }
        public Aes? Aes { get; set; }
        public bool GetParameters(out RSAParameters rsaParams, out RSAEncryptionPadding padding)
        {
            rsaParams = new RSAParameters();
            padding = RSAEncryptionPadding;
            if (RSAParameters == null)
                return false;
            
            rsaParams = (RSAParameters)RSAParameters;
            return true;
        }
        public bool GetParameters(out Aes aes)
        {
            aes = Aes;
			if (Aes == null)
				return false;
			//if (aes.Key == null || aes.IV == null)
   //             return false;
            return true;
        }
    }
}
