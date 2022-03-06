using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using GvasFormat;
using GvasFormat.Serialization;
using Newtonsoft.Json;

namespace GvasConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length >= 3)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  gvas-converter infile <outfile>");
                return;
            }

            var inFile = args[0];
            var outFile = args.Length == 1 ? string.Empty : args[1];

            var ext = Path.GetExtension(inFile).ToLower();

            if (args.Length == 1)
            {
                var fileName = Path.GetFileNameWithoutExtension(inFile);
                if (ext == ".json") outFile = fileName + ".json";
                else outFile = fileName + ".sav";
            }

            if (ext == ".json")
            {
                Console.WriteLine("Loading json...");
                Gvas data = JsonConvert.DeserializeObject<Gvas>(File.ReadAllText(inFile), new GvasJsonConverter(), new ByteArrayToHexConverter());
                var stream = File.Open(outFile, FileMode.Create, FileAccess.Write);
                Console.WriteLine("Converting and saving file...");
                UESerializer.Write(stream, data);
            }
            else
            {
                Console.WriteLine("Parsing UE4 save file structure...");
                Gvas save;
                using (var stream = File.Open(inFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    save = UESerializer.Read(stream);

                Console.WriteLine("Converting to json...");
                var json = JsonConvert.SerializeObject(save, Formatting.Indented, new ByteArrayToHexConverter() );

                Console.WriteLine("Saving json...");
                using (var stream = File.Open(outFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    writer.Write(json);
            }
            Console.WriteLine("Done.");
        }
    }
}
