using System;

namespace LouveSystems.Cryptography
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class AES : IDisposable, ITransformAlgorithm {
        const ushort ITERATIONS = 16; // Increase to make bruteforce attacks harder, but will make the encryption slower
        const ushort READ_BUFFER_LENGTH = 1024; // Increase this number will increase memory use

        readonly byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        readonly byte[] passKey;
        readonly RijndaelManaged aes;

        public class AESCryptographyOperationException : Exception { public AESCryptographyOperationException(string reason) : base(reason) { } }

        public AES(string passKey) : this(Encoding.UTF8.GetBytes(passKey)) { }

        public AES(string passKey, string salt) : this(Encoding.UTF8.GetBytes(passKey), Encoding.ASCII.GetBytes(salt.Substring(0,8))) { }

        public AES(byte[] passKey, byte[] saltBytes = null)
        {
            this.passKey = passKey;

            this.saltBytes = saltBytes ?? this.saltBytes;

            var key = new Rfc2898DeriveBytes(this.passKey, this.saltBytes, ITERATIONS);

            aes = new RijndaelManaged();
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Padding = PaddingMode.PKCS7;

            // Very important! Prevents watermark attacks & blazing fast. Do not use another CipherMode.
            aes.Mode = CipherMode.CFB;
        }

        public byte[] Encode(string input)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                using (var outputMs = new MemoryStream())
                {
                    if (Encode(ms, outputMs, ms.ToArray().Length))
                    {
                        return outputMs.ToArray();
                    }

                    throw new AESCryptographyOperationException($"An error occured while encrypting {input}");
                }

            }
        }

        public string Decode(byte[] input)
        {
            using (var ms = new MemoryStream(input))
            {
                using (var outputMs = new MemoryStream())
                {
                    try {
                        Decode(ms, outputMs, ms.ToArray().Length);
                        return Encoding.UTF8.GetString(outputMs.ToArray());
                    }
                    catch (System.Exception e){
                        throw new AESCryptographyOperationException($"An error occured while decrypting {input}\n{e}");
                    }
                }

            }
        }

        public bool Encode(Stream inputStream, Stream outputStream, int? length = null)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(inputStream)) {
#if NET471
                    MemoryStream net471MS;
                    using (net471MS = new MemoryStream()){
                        using (var cs = new CryptoStream(net471MS, aes.CreateEncryptor(), CryptoStreamMode.Write))
#else
                    using (var cs = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write, leaveOpen: true))
#endif
                    {
                        if (length.HasValue)
                        {
                            cs.Write(reader.ReadBytes(length.Value), 0, length.Value);
                        }
                        else
                        {
                            while (true)
                            {
                                var buffer = reader.ReadBytes(READ_BUFFER_LENGTH);
                                cs.Write(buffer, 0, buffer.Length);
                                if (buffer.Length < READ_BUFFER_LENGTH)
                                {
                                    // We're done here!
                                    break;
                                }
                            }
                        }
                    }
#if NET471
                        // Reopening stream to read...
                        using (var net471MS2 = new MemoryStream(net471MS.ToArray())){
                            net471MS2.CopyTo(outputStream);
                        }
                    }
#endif
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error while encrypting");
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        public bool Decode(Stream inputStream, Stream outputStream, int? length = null)
        {
            using (BinaryWriter writer = new BinaryWriter(outputStream, Encoding.UTF8, leaveOpen:true))
            {
////#if NET471
////                Stream originalInputStream = inputStream;
////                inputStream = new MemoryStream();
////                originalInputStream.CopyTo(inputStream);
////#endif
                using (var cs = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))

                {
                    if (length.HasValue)
                    {
                        var buffer = new byte[length.Value];
                        cs.Read(buffer, 0, length.Value);
                        writer.Write(buffer);
                    }
                    else
                    {
                        while (true)
                        {
                            var buffer = new byte[READ_BUFFER_LENGTH];
                            var read = cs.Read(buffer, 0, READ_BUFFER_LENGTH);
                            writer.Write(buffer, 0, read);
                            if (read < READ_BUFFER_LENGTH)
                            {
                                // We're done here!
                                break;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public void Dispose()
        {
            ((IDisposable)aes).Dispose();
        }
    }
}
