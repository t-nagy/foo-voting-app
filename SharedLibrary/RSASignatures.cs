using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class RSASignatures
    {
        public byte[] SignData(string data, RSACryptoServiceProvider rsa)
        { 
            var encoder = new UTF8Encoding();
            byte[] originalData = encoder.GetBytes(data);
            return rsa.SignData(originalData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
