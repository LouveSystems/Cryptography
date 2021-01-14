using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LouveSystems.Cryptography.HashAlgorithms
{
    // Louve systems simple
    public class LSS : IHashAlgorithm
    {
        public byte Length => 8;

        public string Hash(byte[] rawData)
        {
            ulong hash = 0x1234567887654321;

            for (int i = 0; i < rawData.Length; i++)
            {
                hash = (ulong)(rawData[i] * (i%10)) + (hash << 16) - hash;
            }

            return hash.ToString("X16");
        }
    }
}
