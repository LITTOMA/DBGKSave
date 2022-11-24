using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using DBGKLib.Compression;

namespace DBGKSave.Cli;

class Program
{
    static void Main(string[] args)
    {
        // Usage: DBGKSave.Cli.exe <-d|-c> <input file> <output file>
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: DBGKSave.Cli.exe <-d|-c> <input file> <output file>");
            return;
        }

        if (args[0] == "-d")
        {
            // Decompress
            DecompressSaveDataFile(args[1], args[2]);
        }
        else if (args[0] == "-c")
        {
            // Compress
            CompressSaveDataFile(args[1], args[2]);
        }
        else
        {
            Console.WriteLine("Usage: DBGKSave.Cli.exe <-d|-c> <input file> <output file>");
            return;
        }

        Console.WriteLine("Done!");
    }

    private static void DecompressSaveDataFile(string input, string output)
    {
        using var fs = File.OpenRead(input);
        using var reader = new BinaryReader(fs);
        var magic = reader.ReadUInt32();
        var dataSize = reader.ReadUInt32();
        if (magic != 1 || dataSize > fs.Length)
        {
            throw new InvalidDataException();
        }

        var data = reader.ReadBytes((int)dataSize);
        var decompressed = Zstb.Decompress(data);
        File.WriteAllBytes(output, decompressed);
    }

    private static void CompressSaveDataFile(string input, string output)
    {
        var text = File.ReadAllText(input);
        var minified = MinifyJson(text);
        var bytes = Encoding.UTF8.GetBytes(minified);
        var compressed = Zstb.Compress(bytes);
        using var fs = File.OpenWrite(output);
        using var writer = new BinaryWriter(fs);
        writer.Write(1u);
        writer.Write((uint)compressed.Length);
        writer.Write(compressed);
    }

    private static string MinifyJson(string json)
    {
        var minified = Regex.Replace(json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
        return minified;
    }
}
