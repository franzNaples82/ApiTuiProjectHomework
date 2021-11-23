
using CoreApiLibrary;
using CsvHelper;
using IronXL;
using Konsole;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ApiTuiProject
{
    public class Program
    {
        public static int Main(string[] args)
                   => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "TUIEndPoint")]
        public string TUIEndPoint { get; } = HelperUtility.baseUrl;

        [Option(Description = "WeatherEndPoint")]
        public string WeatherEndPoint { get; } = HelperUtility.BaseApiWeatherUrl;

        [Option(ShortName = "nCities")]
        public int CountCities { get; set; }
        public string currentDate { get; set; }
        public string tomorrowDate { get; set; }

        List<MapForecastCity> payloadForecastList = new List<MapForecastCity>();

        private void OnExecute()
        {
            DateTime now = DateTime.Now;
            DateTime tomorrow = now.AddDays(1);
            currentDate = now.Year.ToString() + "-" + now.Month.ToString() + "-" + now.Day.ToString();
            tomorrowDate = tomorrow.Year.ToString() + "-" + tomorrow.Month.ToString() + "-" + tomorrow.Day.ToString();
            var w = Window.OpenBox($"Step 1 | Development", 100, 10);

            #region TuiCityEndPoint
            var left = w.SplitLeft($"List Of Cities");
            Console.WriteLine($"Open EndPoint {TUIEndPoint} ");
            RepositoryTUIApi repositoryTUIApi = new RepositoryTUIApi();

            var result = repositoryTUIApi.GetResponseRequestCityEndPoint();
            dynamic objectRequestJson = null;
            foreach (var item in result)
            {
                Console.WriteLine($"{item.Key}");
                objectRequestJson = item.Value;
            }
            Console.WriteLine($" N. City Loaded {objectRequestJson.Count}");
            Console.WriteLine($" Call Method City Get List ");
            Console.WriteLine($" Show Progress Content");
            List<MapCity> cityList = new List<MapCity>();

            cityList = repositoryTUIApi.GetListPayloadCity(objectRequestJson);
            var pb1 = new ProgressBar(left, cityList.Count);
            for (int i = 0; i <= cityList.Count; i++)
            {
                ShowProgressBar(i, cityList.Count);
                pb1.Refresh(i, "List Item");
                Thread.Sleep(500);

            }
            Console.WriteLine($"\n");
            Console.WriteLine($"City List Content Processed");
            #endregion
            #region Weather
            var right = w.SplitRight($"City Get Forecast");
            Console.WriteLine($"Open EndPoint {WeatherEndPoint} ");
            Console.WriteLine($"Show Progress Content");
            dynamic objResultWeather = null;
            payloadForecastList = repositoryTUIApi.GetResponseListWeather();
            #region Write StOut
            const string fileName = "OutputTest.xlsx";
            const String fileDir = @"TestData";
            string FileCurrentFolder = Path.Combine(CurrentFolder, "TestData", fileName);
            string FileDestinationFolder = Path.Combine(fileDir, fileName);
            if (File.Exists(FileCurrentFolder))
                File.Delete(FileCurrentFolder);
            string FileCurrentStOutFolder = Path.Combine(CurrentFolder, "TestData", "stOut.txt");
            if (File.Exists(FileCurrentStOutFolder))
                File.Delete(FileCurrentStOutFolder);
            //WriteExcel(payloadForecastList, FileCurrentFolder);
            StringBuilder stringBuilder = new StringBuilder();
            IEnumerable<MapForecastCity> mapForecastCities = payloadForecastList.ToList();
            var pb2 = new ProgressBar(right, payloadForecastList.Count);
            foreach(var itemforecast in mapForecastCities)
            {
                Boolean bCheck = stringBuilder.ToString().Contains(itemforecast.name);
                if(!bCheck)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("[Processed City] " + itemforecast.name + "|"  
                        + mapForecastCities.Where(u => u.date == currentDate).Select(u => u.condition.ToString()).FirstOrDefault()
                        + "-" 
                        + mapForecastCities.Where(u => u.date == tomorrowDate).Select(u => u.condition.ToString()).FirstOrDefault());
                    ////WriteStConsoleOut(payloadForecastList, FileCurrentStOutFolder);
                    stringBuilder.AppendLine();
                }

            }
            for (int i = 0; i <= payloadForecastList.Count; i++)
            {
                ShowProgressBar(i, payloadForecastList.Count);
                pb2.Refresh(i, "Load " + payloadForecastList.Select(u => u.name));
                Thread.Sleep(500);
            }
            using (StreamWriter file = new System.IO.StreamWriter(FileCurrentStOutFolder))
            {
                file.WriteLine(stringBuilder.ToString()); // "sb" is the StringBuilder
            }
            #endregion
            Console.WriteLine($"File StOut Complete");
            #endregion

        }

        private void WriteStConsoleOut(List<MapForecastCity> payloadForecastList,String FileCurrentStOutFolder)
        {

            using (var fs = new FileStream(FileCurrentStOutFolder, FileMode.Create, FileAccess.Write))
            {
                IEnumerable<MapForecastCity> mapForecastCities = payloadForecastList.ToList();
                //AddText(fs, "Forecast City    | [ weather today]  - [weather tomorrow]");
                //AddText(fs, "\n");
                AddText(fs, "[Processed City] " + mapForecastCities.Select(u => u.name).FirstOrDefault());
                AddText(fs, "|");

                AddText(fs, mapForecastCities.Where(u => u.date == currentDate).Select(u => u.condition.ToString()).FirstOrDefault());
                AddText(fs, "-");
                AddText(fs, mapForecastCities.Where(u => u.date == tomorrowDate).Select(u => u.condition.ToString()).FirstOrDefault());
            }
        }
        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
        private static void WriteExcel(List<MapForecastCity> payloadForecastList,String FileCurrentFolder)
        {
            DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(payloadForecastList), (typeof(DataTable)));
            var memoryStream = new MemoryStream();

            using (var fs = new FileStream(FileCurrentFolder, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("ForecastByCity");

                List<String> columns = new List<string>();
                IRow row = excelSheet.CreateRow(0);
                int columnIndex = 0;

                foreach (DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);
                    row.CreateCell(columnIndex).SetCellValue(column.ColumnName);
                    columnIndex++;
                }

                int rowIndex = 1;
                foreach (DataRow dsrow in table.Rows)
                {
                    row = excelSheet.CreateRow(rowIndex);
                    int cellIndex = 0;
                    foreach (String col in columns)
                    {
                        row.CreateCell(cellIndex).SetCellValue(dsrow[col].ToString());
                        cellIndex++;
                    }

                    rowIndex++;
                }
                workbook.Write(fs);
            }
        }

        private static String CurrentFolder
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        private static void ShowProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("[");
            Console.CursorLeft = 32;
            Console.Write("]");
            Console.CursorLeft = 1;
            float oneunit = 30.0f / total;

            //draw progress
            int position = 1;
            for (int i = 0; i < oneunit * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw strip bar
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    ");

        }
    }
}
