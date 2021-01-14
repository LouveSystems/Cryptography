using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LouveSystems.Cryptography
{
    public class RSA : IDisposable, ITransformAlgorithm
    {
        public RSAParameters PublicKey { private set; get; }
        
        private const ushort BINARY_BUFFER_LENGTH = 1024;

        private bool isReadOnly = false;
        private RSAParameters privateKey;
        private RSACryptoServiceProvider csp;

        public class WrongKeyException : Exception { public WrongKeyException(string msg) : base(msg) { } }
        public class ConfigurationMismatchException : Exception { public ConfigurationMismatchException(string msg) : base(msg) { } }

        public RSA(string keyToImport = null, bool isDecryptOnly = true)
        {
            if (keyToImport != null)
            {
                isReadOnly = isDecryptOnly;
                csp = new RSACryptoServiceProvider();
                csp.FromXmlString(keyToImport);
            }
            else
            {
                //lets take a new CSP with a new 2048 bit rsa key pair
                csp = new RSACryptoServiceProvider(2048);
                isReadOnly = false;
                privateKey = csp.ExportParameters(true);
            }

            PublicKey = csp.ExportParameters(false);
        }

        public byte[] Encode(string str)
        {
            var bytesPlainTextData = Encoding.Unicode.GetBytes(str);

            //apply pkcs#1.5 padding and encrypt our data 
            var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

            return bytesCypherText;
        }

        public string Decode(byte[] obj)
        {
            if (isReadOnly)
            {
                throw new ConfigurationMismatchException("Cannot decrypt with a READ ONLY encrypter");
            }

            byte[] bytesPlainTextData;
            try
            {
                bytesPlainTextData = csp.Decrypt(obj, false);
            }
            catch (CryptographicException e)
            {
                throw new WrongKeyException(e.ToString());
            }

            //get our original plainText back...
            var plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

            return plainTextData;
        }

        public string Serialize(bool includePrivateParameters = true)
        {
            return csp.ToXmlString(includePrivateParameters);
        }

        public bool Encode(Stream inputStream, Stream outputStream, int? length = null)
        {
            using (BinaryReader br = new BinaryReader(inputStream))
            {
                using (BinaryWriter bw = new BinaryWriter(outputStream, Encoding.UTF8, leaveOpen: true))
                {
                    var bytes = Toolkit.BinaryReadToEnd(br, length, BINARY_BUFFER_LENGTH);

                    try
                    {
                        var encrypted = csp.Encrypt(bytes, false);
                        bw.Write(encrypted);
                    }
                    catch (CryptographicException)
                    {
                        return false;
                    }
                }
            }

            return true;

        }

        public bool Decode(Stream inputStream, Stream outputStream, int? length = null)
        {
            if (isReadOnly)
            {
                throw new ConfigurationMismatchException("Cannot decrypt with a READ ONLY encrypter");
            }

            byte[] bytesPlainTextData;
            using (BinaryReader br = new BinaryReader(inputStream))
            {
                try
                {
                    bytesPlainTextData = csp.Decrypt(Toolkit.BinaryReadToEnd(br, length, BINARY_BUFFER_LENGTH), false);
                    using (BinaryWriter bw = new BinaryWriter(outputStream, Encoding.UTF8, leaveOpen: true))
                    {
                        bw.Write(bytesPlainTextData);
                    }
                }
                catch (CryptographicException e)
                {
                    throw new WrongKeyException(e.ToString());
                }
            }

            return true;
        }

        public void Dispose()
        {
            ((IDisposable)csp).Dispose();
        }
    }
}
