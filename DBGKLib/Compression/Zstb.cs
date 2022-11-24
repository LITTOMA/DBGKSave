using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using DBGKLib.Extensions;

namespace DBGKLib.Compression
{
    public class Zstb
    {
        public Stream Decompress(Stream stream)
        {
            return new MemoryStream(Decompress(stream.ToArray()));
        }

        public Stream Compress(Stream stream)
        {
            return new MemoryStream(Compress(stream.ToArray()));
        }

        public static byte[] Decompress(byte[] b)
        {
            using (var decompressor = new ZstdSharp.Decompressor())
            {
                return decompressor.Unwrap(b).ToArray();
            }
        }
        public static byte[] Decompress(byte[] b, int MaxDecompressedSize)
        {
            using (var decompressor = new ZstdSharp.Decompressor())
            {
                return decompressor.Unwrap(b, MaxDecompressedSize).ToArray();
            }
        }
        public static byte[] Compress(byte[] b)
        {
            using (var compressor = new ZstdSharp.Compressor(22))
            {
                return compressor.Wrap(b).ToArray();
            }
        }
    }
}