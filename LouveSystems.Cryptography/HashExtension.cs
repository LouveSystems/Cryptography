using System;
namespace LouveSystems.Cryptography
{
    public static class HashExtension
    {
        public static string Hash(this byte[] rawData, IHashAlgorithm hashAlgorithm)
        {
            return hashAlgorithm.Hash(rawData);
        }
    }
}
