using System.Text;

namespace Generator.Services
{
    internal class BigFileGenerator
    {
        private char[] buffer;
        private const int maxBufferSizeSetting = 1 * 1024 * 1024 * 100; // 100 мегабайт

        public void Generate(int fileSizeinMegabytes, string fileName, Dictionary<int, string> fileStrings)
        {
            double fileSizeinBytes = (double) fileSizeinMegabytes * 1024 * 1024;

            var rng = new Random();
            double currentFileSize = 0;
            int numOfStrings = fileStrings.Count;


            try
            {
                using (var fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
                {
                    Console.WriteLine($"Создан файл '{fileName}'");

                    var maxBufferSize = (int)Math.Min(fileSizeinBytes, maxBufferSizeSetting);
                    buffer = new char[maxBufferSize];
                    var span = buffer.AsSpan();

                    using (var writetext = new StreamWriter(fileStream, Encoding.ASCII))
                    {
                        var currentBufferSize = 0;
                        while (true)
                        {
                            var str = $"{rng.Next()}.{fileStrings[rng.Next(numOfStrings)]}";
                            var futureBufferSize = currentBufferSize + str.Length + 2;

                            if (futureBufferSize > maxBufferSize || currentFileSize + futureBufferSize > fileSizeinBytes)
                            {
                                writetext.Write(buffer, 0, currentBufferSize);
                                currentFileSize += currentBufferSize;
                                currentBufferSize = 0;
                            }

                            if (currentFileSize > fileSizeinBytes)
                                break;

                            foreach (var s in str)
                            {
                                span[currentBufferSize++] = s;
                            }
                            span[currentBufferSize++] = '\r';
                            span[currentBufferSize++] = '\n';

                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Не удалось создать файл '{fileName}'. Возможно он уже существует.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось создать файл '{fileName}'.");
            }


            Console.WriteLine($"Размер файла = '{currentFileSize}' байт");
        }
    }
}
