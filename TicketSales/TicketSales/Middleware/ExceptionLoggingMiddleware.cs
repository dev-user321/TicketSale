using TicketSales.Data;
using TicketSales.Models;

namespace TicketSales.Middleware
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AppDbContext dbContext)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var log = new ExceptionLog
                {
                    Path = context.Request.Path,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                dbContext.ExceptionLogs.Add(log);
                await dbContext.SaveChangesAsync();

                throw;
            }
        }
    }
}
