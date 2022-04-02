using GvasFormat.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GvasConverter
{
    public class ArgsPraser
    {
        public string Input;
        public string Output;
        public Dictionary<string, string> ExtraArguments = new Dictionary<string, string>();
        public OperationType Operation;
        public enum OperationType
        {
            SavToJson,
            JsonToSav,
            AccuracyTest,
            ExportLiveryProject
        }

        public class ArgumentNames
        {
            //public const string JsonToSav = "--json";
            //public const string SavToJson = "--sav";
            public const string AccuracyTest = "--test";
            public const string ExportLiveryProject = "--liveryProjectExport";
            public const string Options = "--option";
            public const string Developer = "--dev";
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: \r\n");

            Console.WriteLine($" gvas-converter [functions]");
            Console.WriteLine($" gvas-converter {ArgumentNames.Options}:SETTING1=VALUE1:SETTING2=VALUE2... [functions]\r\n");

            Console.WriteLine("Functions: \r\n");

            Console.WriteLine($" {ArgumentNames.ExportLiveryProject} [inFile] [projectName]");
            Console.WriteLine("  | export a single livery project from inFile \r\n");

            Console.WriteLine($" {ArgumentNames.AccuracyTest} [inFile] <outFile>");
            Console.WriteLine("  | perform sav to json to sav accuracy test (must be used on a .sav file) \r\n");

            Console.WriteLine($" [inFile] <outFile>");
            Console.WriteLine("  | convert inFile to outFile \r\n");
        }
        private static string[] DeveloperArgument(string[] args)
        {
            int i = 0;

            if (i == 0) Environment.CurrentDirectory = @"C:\Users\Cwall\AppData\Local\hotwheels\Saved\SaveGames";
            if (i == 1) Environment.CurrentDirectory = @"D:\UserData\Modding Workspaces\Hot Wheels Unleashed\Save\Research\Zolton";
            if (i == 2) Environment.CurrentDirectory = @"D:\UserData\Modding Workspaces\Hot Wheels Unleashed\Save\Research\jmsbd07";
            return args.Skip(1).ToArray();
        }

        private static string[] OptionsArgument(string[] args)
        {
            var options = args[0].Split(':').ToList();
            options.Remove(ArgumentNames.Options);

            for (int i = 0; i < options.Count; i++)
            {
                if (!options.Contains("=")) continue;

                var keyvaluepair = options[i].Split('=');

                if (keyvaluepair.Length != 2) continue;

                var key = keyvaluepair[0];
                var value = keyvaluepair[1];

                switch (key)
                {
                    case "LiveryProjectionPraseOption":
                        if (value == "Chunks") GvasSettings.LiveryProjectionPraseOption = GvasSettings.LiveryProjectionPraseMode.Chunks;
                        else if (value == "Layers") GvasSettings.LiveryProjectionPraseOption = GvasSettings.LiveryProjectionPraseMode.Layers;
                        break;
                }

            }
            return args.Skip(1).ToArray();
        }

        public static ArgsPraser Prase(string[] args)
        {
            if (args.Length >= 1 && args[0] == ArgumentNames.Developer) args = DeveloperArgument(args);
            if (args.Length >= 1 && args[0].StartsWith(ArgumentNames.Options)) args = OptionsArgument(args);

            if (args.Length == 0) return GetWithoutArgs();
            else
            {
                ArgsPraser result;
                var option = args[0];
                switch (option)
                {
                    case ArgumentNames.ExportLiveryProject:
                        result = PraseExportLiveryProject(args);
                        break;
                    case ArgumentNames.AccuracyTest:
                        result = PraseAccuracyTest(args);
                        break;
                    default:
                        result = PraseLegacy(args);
                        break;
                }


                if (result == null) PrintUsage();
                return result;
            }
        }


        private static ArgsPraser GetWithoutArgs()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Select Input File...",
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "All Files (*.sav;*.json)|*.sav;*.json",
                Multiselect = false

            };
            if (ofd.ShowDialog() != DialogResult.OK) return null;

            
            bool jsonToSave = (Path.GetExtension(ofd.FileName).ToLower() == ".json");
            string acceptedTypes = jsonToSave ? "Sav Files (*.sav)|*.sav" : "Json Files (*.json)|*.json";

            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Select Output File...",
                InitialDirectory = Environment.CurrentDirectory,
                Filter = acceptedTypes
            };
            if (sfd.ShowDialog() != DialogResult.OK) return null;

            ArgsPraser result = new ArgsPraser();
            result.Input = ofd.FileName;
            result.Output = sfd.FileName;
            result.Operation = jsonToSave ? OperationType.JsonToSav : OperationType.SavToJson;
            return result;
        }

        private static ArgsPraser PraseExportLiveryProject(string[] args)
        {
            if (args.Length < 3 && args.Length >= 4) return null;

            var inFile = args[1];
            var ext = Path.GetExtension(inFile).ToLower();
            if (ext != ".sav") return null;

            ArgsPraser result = new ArgsPraser();
            result.Input = inFile;

            string liveryName = args[2];
            result.ExtraArguments.Add("LiveryName", liveryName);
            result.Output = Path.Combine(Path.GetDirectoryName(inFile), liveryName + ".json");
            result.Operation = OperationType.ExportLiveryProject;

            return result;
        }
        private static ArgsPraser PraseAccuracyTest(string[] args)
        {
            if (args.Length < 2 && args.Length > 3) return null;

            var inFile = args[1];
            var ext = Path.GetExtension(inFile).ToLower();
            if (ext != ".sav") return null;

            ArgsPraser result = new ArgsPraser();
            result.Input = inFile;
            if (args.Length == 3) result.Output = args[2];
            result.Operation = OperationType.AccuracyTest;
            return result;
        }
        private static ArgsPraser PraseLegacy(string[] args)
        {
            if (args.Length < 1 && args.Length >= 3) return null;

            var inFile = args[0];
            var outFile = args.Length == 1 ? string.Empty : args[1];
            var ext = Path.GetExtension(inFile).ToLower();
            OperationType mode;

            if (args.Length == 1)
            {
                var fileName = Path.GetFileNameWithoutExtension(inFile);
                if (ext == ".json") outFile = fileName + ".sav";
                else outFile = fileName + ".json";
            }

            if (ext == ".json") mode = OperationType.JsonToSav;
            else mode = OperationType.SavToJson;

            ArgsPraser result = new ArgsPraser();
            result.Input = inFile;
            result.Output = outFile;
            result.Operation = mode;
            return result;
        }

    }

}
