using TicketSales.Services.Interfaces;

namespace TicketSales.Services
{
    public class FileService : IFileService
    {
        public async Task<string> ReadFileAsync(string path)
        {
            using StreamReader reader = new StreamReader(path);
            var body = await reader.ReadToEndAsync();
            return body;
        }
    }
}
