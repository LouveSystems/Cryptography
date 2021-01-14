using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace LouveSystems.Cryptography
{
    public class PrependHash : ITransformAlgorithm
    {
        const ushort BINARY_BUFFER_LENGTH = 1024;

        public string ReadHash { private set; get; } = string.Empty;

        public string ComputedHash { private set; get; } = string.Empty;

        private IHashAlgorithm hashAlgorithm;

        public class HashMismatchException : Exception { public HashMismatchException(string details) : base(details) { } }

        public PrependHash(IHashAlgorithm algorithm)
        {
            this.hashAlgorithm = algorithm;
        }

        public bool Encode(Stream inputStream, Stream outputStream, int? length)
        {
            using (var br = new BinaryReader(inputStream))
            {
                byte[] buffer = Toolkit.BinaryReadToEnd(br, length, BINARY_BUFFER_LENGTH);

                using (var bw = new BinaryWriter(outputStream, Encoding.UTF8, leaveOpen: true))
                {
                    // We are reencoding the hash as bytes here to avoid the artificial 1-byte padding set by the string size marker
                    ComputedHash = buffer.Hash(hashAlgorithm);
                    bw.Write(Encoding.UTF8.GetBytes(ComputedHash));
                    bw.Write(buffer);
                }
            }

            return true;
        }

        public bool Decode(Stream inputStream, Stream outputStream, int? length)
        {
            using (var br = new BinaryReader(inputStream))
            {
                var buffer = br.ReadBytes(hashAlgorithm.Length);
                ReadHash = Encoding.UTF8.GetString(buffer);

                buffer = Toolkit.BinaryReadToEnd(br, length - hashAlgorithm.Length, BINARY_BUFFER_LENGTH);

                using (var bw = new BinaryWriter(outputStream, Encoding.UTF8, leaveOpen: true))
                {
                    // We are reencoding the hash as bytes here to avoid the artificial 1-byte padding set by the string size marker
                    bw.Write(buffer);
                }
            }

            return true;
        }
    }
}
