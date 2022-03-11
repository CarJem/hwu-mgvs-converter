using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GvasConverter
{
    public class ArgsPraser
    {
        public string Input;
        public string Output;
        public OperationType Operation;
        public enum OperationType
        {
            SavToJson,
            JsonToSav,
            AccuracyTest
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  gvas-converter inFile <outFile> : convert inFile to outFile");
            Console.WriteLine("  gvas-converter --test inFile    : perform sav to json to sav accuracy test (must be used on a .sav file)");
        }

        public static ArgsPraser Prase(string[] args)
        {
            if (args.Length == 0) return GetWithoutArgs();
            else
            {
                ArgsPraser result;
                if (args.Length == 2 && args[0] == "--test") result = PraseAccuracyTest(args);
                else if (args.Length >= 1 && args.Length < 3) result = PraseClassic(args);
                else result = null;


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
        private static ArgsPraser PraseClassic(string[] args)
        {
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
        private static ArgsPraser PraseAccuracyTest(string[] args)
        {
            var inFile = args[1];
            var ext = Path.GetExtension(inFile).ToLower();
            if (ext != ".sav") return null;

            ArgsPraser result = new ArgsPraser();
            result.Input = inFile;
            result.Operation = OperationType.AccuracyTest;
            return result;
        }

    }

}
