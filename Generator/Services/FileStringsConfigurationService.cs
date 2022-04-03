namespace Generator.Services
{
    internal class FileStringsConfigurationService
    {
        public async Task<Dictionary<int, string>> GetFileStringsConfigurationAsync(string filePath)
        {
            var result = new Dictionary<int, string>();
            var i = 0;

            try
            {
                using (var sr = File.OpenText(filePath))
                {
                    string s = "";
                    while ((s = await sr.ReadLineAsync()) != null)
                    {

                        result.Add(i, s);
                        i++;
                    }
                }

                return result;
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception($"Файл {ex.FileName} не найден. Он должен находиться в папке с программой");
            }
            catch (Exception)
            {
                throw new Exception($"Не удалось прочитать файл со списком строк: {filePath}");
            }
        }
    }
}
