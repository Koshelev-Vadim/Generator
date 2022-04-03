using Generator.Services;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Generator
{
    class Program
    {
        private static string FileStringsName = "FileStrings.txt";

        private class Arguments
        {
            public int FileSizeInMegabites { get; set; }
            public string FileName { get; set; }
        }

        private static string GetFileName(int fileSize)
        {
            var fileName = $"{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}_{fileSize}_Mb.txt";
            return fileName;
        }

        private static Arguments ParseArguments(string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
           {
               { "-sizeInMegabytes", "size" },
               { "-fileName", "fileName" }
           };
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args, switchMappings);
            var config = builder.Build();


            if (config["size"] == null || !int.TryParse(config["size"], out int fileSize))
            {
                Console.WriteLine("Параметр '-sizeInMegabytes' не установлен. Используется значение по умолчанию (10 Мб)");
                fileSize = 10;
            }

            string fileName = config["fileName"];
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = GetFileName(fileSize);
                Console.WriteLine($"Параметр '-fileName' не установлен. Используется значение по умолчанию ({fileName})");
                
            }

            return new Arguments { FileSizeInMegabites = fileSize, FileName = fileName };
        }

        static async Task<int> Main(string[] args)
        {
            var arguments = ParseArguments(args);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Console.WriteLine($"Запрошено создание файла размером {arguments.FileSizeInMegabites} Мб");

            var fileStringsConfigurationService = new FileStringsConfigurationService();
            var fileStrings = await fileStringsConfigurationService.GetFileStringsConfigurationAsync(FileStringsName);

            var generator = new BigFileGenerator();
            generator.Generate(arguments.FileSizeInMegabites, arguments.FileName, fileStrings);

            stopWatch.Stop();

            var seconds = (double)(stopWatch.ElapsedMilliseconds / 1000);
            Console.WriteLine($"Генерация файла завершена за {seconds.ToString("F")} секунд" );

            return 0;
        }


    }
}