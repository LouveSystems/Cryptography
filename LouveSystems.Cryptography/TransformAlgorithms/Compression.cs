using System.IO;
using System.IO.Compression;

namespace LouveSystems.Cryptography
{
    public class Compression : ITransformAlgorithm
    {
        public bool Encode(Stream inputStream, Stream outputStream, int? length)
        {
            using (GZipStream compressionStream = new GZipStream(outputStream, CompressionLevel.Fastest, leaveOpen: true))
            {
                inputStream.CopyTo(compressionStream);
            }

            return true;
        }

        public bool Decode(Stream inputStream, Stream outputStream, int? length)
        {
            using (GZipStream compressionStream = new GZipStream(inputStream, CompressionMode.Decompress, leaveOpen:true))
            {
                compressionStream.CopyTo(outputStream);
            }

            return true;
        }
    }
}
