using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LouveSystems.Cryptography
{
    public static class Toolkit
    {
        public static byte[] BinaryReadToEnd( System.IO.BinaryReader binaryReader, int? length=null, ushort buffSize = 2048)
        {
            byte[] buffer;
            if (length.HasValue)
            {
                buffer = binaryReader.ReadBytes(length.Value);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] bufferBuffer = new byte[buffSize];
                    int count;
                    while ((count = binaryReader.Read(bufferBuffer, 0, bufferBuffer.Length)) != 0)
                        ms.Write(bufferBuffer, 0, count);
                    buffer = ms.ToArray();
                }
            }

            return buffer;
        }
    }
}
