using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LouveSystems.Cryptography
{
    public class Chain : List<ITransformAlgorithm>
    {
        public class CorruptSaveGameException : Exception { public CorruptSaveGameException(string details) : base(details) { } }

        public void Encode(Stream inputStream, Stream outputStream)
        {
            MemoryStream originMs = new MemoryStream();
            MemoryStream destinationMs;

            inputStream.CopyTo(originMs);

            foreach (ITransformAlgorithm transform in this)
            {
                originMs.Seek(0, SeekOrigin.Begin);
                destinationMs = new MemoryStream();

                transform.Encode(originMs, destinationMs);

                originMs.Dispose();
                originMs = destinationMs;
            }

            originMs.Seek(0, SeekOrigin.Begin);
            originMs.CopyTo(outputStream);

            originMs.Dispose();
        }

        public void Decode(Stream inputStream, Stream outputStream)
        {
            MemoryStream originMs = new MemoryStream();
            MemoryStream destinationMs;

            inputStream.CopyTo(originMs);

            var reverseOrder = ((IEnumerable<ITransformAlgorithm>)this).Reverse();

            foreach (ITransformAlgorithm transform in reverseOrder)
            {
                originMs.Position = 0;
                destinationMs = new MemoryStream();

                transform.Decode(originMs, destinationMs);

                originMs.Dispose();
                originMs = destinationMs;
            }

            originMs.Seek(0, SeekOrigin.Begin);
            originMs.CopyTo(outputStream);

            originMs.Dispose();
        }

        public override string ToString()
        {
            return string.Join( "=> ", this.Select(o => o.GetType().ToString()));
        }
    }
}
