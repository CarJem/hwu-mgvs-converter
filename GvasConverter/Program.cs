using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;
using GvasFormat;
using GvasFormat.Serialization;
using Newtonsoft.Json;
using GvasFormat.Utils;
using GvasFormat.Serializer;
using GvasFormat.Converters;

namespace GvasConverter
{
    class Program
    {



        [STAThread]
        static void Main(string[] args)
        {
            var results = ArgsPraser.Prase(args);
            if (results == null) return;

            switch (results.Operation)
            {
                case ArgsPraser.OperationType.JsonToSav:
                    Converter.JsonToSav(results.Input, results.Output);
                    break;
                case ArgsPraser.OperationType.SavToJson:
                    Converter.SavToJson(results.Input, results.Output);
                    break;
                case ArgsPraser.OperationType.ExportLiveryProject:
                    Converter.ExportSingleLiveryProject(results.Input, results.Output, results.ExtraArguments["LiveryName"]);
                    break;
                case ArgsPraser.OperationType.AccuracyTest:
                    Converter.AccuracyTest(results.Input, results.Output);
                    break;
            }
        }
    }
}
