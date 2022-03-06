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
            if (args.Length != 2)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  gvas-converter infile outfile");
                return;
            }

            var ext = Path.GetExtension(args[0]).ToLower();
            if (ext == ".json")
            {
                Console.WriteLine("Loading json...");
                Gvas data = JsonConvert.DeserializeObject<Gvas>(File.ReadAllText(args[0]), new GvasJsonConverter(), new ByteArrayToHexConverter());
                var stream = File.Open(args[1], FileMode.Create, FileAccess.Write);
                Console.WriteLine("Converting and saving file...");
                UESerializer.Write(stream, data);
            }
            else
            {
                Console.WriteLine("Parsing UE4 save file structure...");
                Gvas save;
                using (var stream = File.Open(args[0], FileMode.Open, FileAccess.Read, FileShare.Read))
                    save = UESerializer.Read(stream);

                Console.WriteLine("Converting to json...");
                var json = JsonConvert.SerializeObject(save, Formatting.Indented, new ByteArrayToHexConverter() );

                Console.WriteLine("Saving json...");
                using (var stream = File.Open(args[1], FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    writer.Write(json);
            }
            Console.WriteLine("Done.");
        }
    }
}
