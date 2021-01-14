using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LouveSystems.Cryptography
{
    public interface IHashAlgorithm
    {
        byte Length { get; }

        string Hash(byte[] data);
    }
}
