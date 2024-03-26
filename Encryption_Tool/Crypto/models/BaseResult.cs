using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encryption_Tool.EncryptionEngine.models
{
    public class BaseResult
    {
        public byte[] Data { get; set; }

        List<string> errors;
        public List<string> Errors
        {
            get
            {
                errors ??= [];
                return errors;
            }
        }
        public bool Success => Errors.Count == 0;
        public void AddError(string error)
        {
            errors ??= [];
            errors.Add(error);
        }
    }
}
