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

        static string[] DevSetup(string[] args)
        {
            int i = 0;

            if (i == 0) Environment.CurrentDirectory = @"C:\Users\Cwall\AppData\Local\hotwheels\Saved\SaveGames";
            if (i == 1) Environment.CurrentDirectory = @"D:\UserData\Modding Workspaces\Hot Wheels Unleashed\Save\Research\Zolton";
            if (i == 2) Environment.CurrentDirectory = @"D:\UserData\Modding Workspaces\Hot Wheels Unleashed\Save\Research\jmsbd07";
            return args.Skip(1).ToArray();
        }


        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length >= 1 && args[0] == "--dev") args = DevSetup(args);

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
                case ArgsPraser.OperationType.AccuracyTest:
                    Converter.AccuracyTest(results.Input, results.Output);
                    break;
            }
        }
    }
}
