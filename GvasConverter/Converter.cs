using GvasFormat.Converters;
using GvasFormat.Serialization;
using GvasFormat.Serialization.UETypes;
using GvasFormat.Serialization.HotWheels;
using GvasFormat.Serializer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GvasConverter
{
    public static class Converter
    {

        public static void AccuracyTest(string inFile, string outFile = null)
        {
            string fileName = Path.GetFileNameWithoutExtension(inFile);
            string jsonFile = fileName + ".json";
            if (outFile == null) outFile = fileName + ".test.sav";
            SavToJson(inFile, jsonFile);
            JsonToSav(jsonFile, outFile);
        }

        public static void JsonToSav(string inFile, string outFile)
        {
            Console.WriteLine("Loading json...");
            Gvas data = JsonConvert.DeserializeObject<Gvas>(File.ReadAllText(inFile), new GvasJsonConverter(), new ByteArrayToHexConverter());
            var stream = File.Open(outFile, FileMode.Create, FileAccess.Write);
            Console.WriteLine("Converting and saving file...");
            UESerializer.Write(stream, data);
            Console.WriteLine("Done.");
        }
        public static void ExportSingleLiveryProject(string inFile, string outFile, string liveryName)
        {
            Console.WriteLine("Parsing UE4 save file structure...");
            Gvas save;
            using (var stream = File.Open(inFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                save = UESerializer.Read(stream);

            if (!save.Properties.Exists(x => x.Name == "LiveryProjects" && x.Type == "ArrayProperty")) 
                throw new Exception("Does not contain any LiveryProjects");

            var projects = save.Properties.ToList().FirstOrDefault(x => x.Name == "LiveryProjects" && x.Type == "ArrayProperty") as UEArrayProperty;

            if (projects.ItemType != "StructProperty" && !projects.Headers.ContainsKey("StructureType"))
                throw new Exception("Invalid Format");
            if (projects.Headers["StructureType"].Value.ToString() != "VehicleEditorProject")
                throw new Exception("Invalid Format");

            var castedProjects = projects.Items.Cast<VehicleEditorProject>().ToList();

            if (!castedProjects.Exists(x => x.LiveryName == liveryName))
                throw new Exception("Livery Project Not Found");

            var project = castedProjects.FirstOrDefault(x => x.LiveryName == liveryName);

            Console.WriteLine("Converting to json...");
            var json = JsonConvert.SerializeObject(project, Formatting.Indented, new ByteArrayToHexConverter());

            Console.WriteLine("Saving json...");
            using (var stream = File.Open(outFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                writer.Write(json);
            Console.WriteLine("Done.");

        }

        public static void SavToJson(string inFile, string outFile)
        {
            Console.WriteLine("Parsing UE4 save file structure...");
            Gvas save;
            using (var stream = File.Open(inFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                save = UESerializer.Read(stream);

            Console.WriteLine("Converting to json...");
            var json = JsonConvert.SerializeObject(save, Formatting.Indented, new ByteArrayToHexConverter());

            Console.WriteLine("Saving json...");
            using (var stream = File.Open(outFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                writer.Write(json);
            Console.WriteLine("Done.");
        }
    }
}
