namespace TicketSales.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> ReadFileAsync(string path);
    }
}
