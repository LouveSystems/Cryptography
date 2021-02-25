using System;
using System.IO;
using System.Text;
using LouveSystems.Cryptography;

namespace LouveSystems.Cryptography.Tester
{
    class Program
    {
        readonly static string[] TEST_STRINGS = new string[]
        {
            "AAAA",
            "AAAB",
            "",
            "Hello world!",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam sit amet nunc elit. Vivamus egestas laoreet ex ac aliquet. Sed luctus posuere lacus, id consectetur urna fermentum in. Aenean finibus pretium sollicitudin. Praesent pulvinar felis ac enim blandit, eget ultricies magna dictum. Maecenas vitae lectus leo. Nulla ultrices luctus mollis. Proin mattis odio id efficitur fringilla. Vivamus non dapibus mauris. Nam vitae libero tellus. "
        };


        static void Main(string[] args)
        {
            Console.WriteLine("==================\n==\n== HASH ALGORITHMS\n==\n==================");
            foreach (var testString in TEST_STRINGS)
            {
                Console.WriteLine(testString);
                Console.WriteLine($"LSS\n => {Encoding.UTF8.GetBytes(testString).Hash(new HashAlgorithms.LSS())}");
                Console.WriteLine($"SHA256\n => {Encoding.UTF8.GetBytes(testString).Hash(new HashAlgorithms.SHA256())}");
                Console.WriteLine($"MD5\n => {Encoding.UTF8.GetBytes(testString).Hash(new HashAlgorithms.MD5())}");

                Console.WriteLine();
            }

            Console.WriteLine("==================\n==\n== TRANSFORM ALGORITHMS\n==\n==================");

            Chain[] chains = new Chain[]
            {
                 new Chain(){new PrependHash(new HashAlgorithms.MD5())},

                 new Chain(){new Compression()},

                 new Chain(){new Compression(), new AES("Hello")},

                 new Chain(){new Compression(), new AES("Hello"), new Compression(), new PrependHash(new HashAlgorithms.MD5())},
        };

            foreach (var chain in chains)
            {
                foreach (var testString in TEST_STRINGS)
                {
                    Console.WriteLine(testString);

                    MemoryStream output;
                    chain.Encode(new MemoryStream(Encoding.UTF8.GetBytes(testString)), output = new MemoryStream());

                    MemoryStream output2;
                    output.Seek(0, SeekOrigin.Begin);

                    chain.Decode(output, output2 = new MemoryStream());
                    bool success = Encoding.UTF8.GetString(output2.ToArray()) == testString;
                    Console.WriteLine($"{chain}: {(success ? "OK" : "ERROR")}");

                    if (!success)
                    {
                        Console.WriteLine(testString);
                        Console.WriteLine(Encoding.UTF8.GetString(output.ToArray()));
                        Console.WriteLine(Encoding.UTF8.GetString(output2.ToArray()));

                        throw new Exception();
                    }
                }

                Console.WriteLine("=====\n");
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
