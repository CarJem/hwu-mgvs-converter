using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;
using GvasConverter.Converters;
using GvasFormat;
using GvasFormat.Serialization;
using Newtonsoft.Json;
using GvasFormat.Utils;
using GvasFormat.Serializer;

namespace GvasConverter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var results = GetFiles(args);
            if (results == null) return;

            var inFile = results.Item1;
            var outFile = results.Item2;
            var jsonToSave = results.Item3;

            if (jsonToSave)
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
        static Tuple<string, string, bool> GetFiles(string[] args)
        {
            if (args.Length == 0) return GetFilesByUI();
            else return GetFilesFromArguments(args);
        }
        static Tuple<string, string, bool> GetFilesFromArguments(string[] args)
        {
            if (args.Length < 1 || args.Length >= 3)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  gvas-converter infile <outfile>");
                return null;
            }

            var inFile = args[0];
            var outFile = args.Length == 1 ? string.Empty : args[1];

            var ext = Path.GetExtension(inFile).ToLower();
            bool jsonToSave = false;

            if (args.Length == 1)
            {
                var fileName = Path.GetFileNameWithoutExtension(inFile);
                if (ext == ".json") outFile = fileName + ".sav";
                else outFile = fileName + ".json";
            }

            if (ext == ".json") jsonToSave = true;
            else jsonToSave = false;

            return new Tuple<string, string, bool>(inFile, outFile, jsonToSave);
        }
        static Tuple<string, string, bool> GetFilesByUI()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Select Input File...",
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "sav files (*.sav)|*.sav|json files (*.json)|*.json",
                Multiselect = false
                
            };
            if (ofd.ShowDialog() != DialogResult.OK) return null;
            
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Select Output File...",
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "sav files (*.sav)|*.sav|json files (*.json)|*.json"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return null;

            var inFile = ofd.FileName;
            var outFile = sfd.FileName;
            bool jsonToSave = (Path.GetExtension(inFile).ToLower() == ".json");

            return new Tuple<string, string, bool>(inFile, outFile, jsonToSave);
        }
    }
}
