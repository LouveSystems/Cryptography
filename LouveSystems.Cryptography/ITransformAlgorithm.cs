using System.IO;

namespace LouveSystems.Cryptography {
    public interface ITransformAlgorithm {

        bool Encode(Stream inputStream, Stream outputStream, int? length = null);

        bool Decode(Stream inputStream, Stream outputStream, int? length = null);
    }
}
