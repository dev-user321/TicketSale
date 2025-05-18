using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TicketSales.Data;
using TicketSales.Models;
using TicketSales.Services.Interfaces;
using TicketSales.ViewModels;

namespace TicketSales.Controllers
{

    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public HomeController(AppDbContext context,IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _context.Categories.Where(m => !m.SoftDelete);
            IEnumerable<Event> events = _context.Events.Where(m => !m.SoftDelete).Include(m=>m.Tickets);
            HomeVm vm = new HomeVm()
            {
                Events = events,
                Categories = categories
            };
            return View(vm);
        }
        public IActionResult Detail(int id)
        {
            var evt = _context.Events
                                 .Where(m => !m.SoftDelete && m.Id == id)
                                 .FirstOrDefault();
            IEnumerable<Ticket> tickets = _context.Tickets.Where(m => !m.SoftDelete && m.EventId == id);
            IEnumerable<Event> loadEvents = _context.Events.Where(m => !m.SoftDelete && m.Id != id && m.CategoryId == evt.CategoryId).Take(4).ToList();

            var homeDetailVm = new HomeDetailVm()
            {
                Event = evt,
                Tickets = tickets,
                LoadEvents = loadEvents
            };

           

            return View(homeDetailVm);
        }
        [HttpGet]
        [Authorize]
        public IActionResult BuyTicket(int id)
        {
            var tickets = _context.Tickets
                .Where(t => !t.SoftDelete && t.EventId == id)
                .ToList();

            var ev = _context.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null || tickets == null) return NotFound();

            var vm = new HomeDetailVm
            {
                Event = ev,
                Tickets = tickets
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BuyTicket(BuyTicketVm model)
        {
            if (model.SelectedSeats == null || !model.SelectedSeats.Any())
            {
                return RedirectToAction("Detail", new { id = model.EventId });
            }

            var username = User.Identity.Name; 
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            var eventEntity = await _context.Events.FindAsync(model.EventId);
            if (eventEntity == null) return NotFound();


            List<TicketOrder> purchasedTickets = new List<TicketOrder>();
            foreach (var seat in model.SelectedSeats)
            {
                var parts = seat.Split('-');
                var type = parts[0];
                var number = parts[1];

                var ticketData = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.SeatNumber.ToLower().Replace(" ", "") == type && t.EventId == model.EventId);

                if (ticketData == null || ticketData.TotalCount <= 0)
                    continue;

                ticketData.TotalCount--;

                var ticketOrder = new TicketOrder
                {
                    UserId = user.Id, 
                    EventId = model.EventId,
                    SeatInfo = seat,
                    Price = ticketData.Price,
                    PurchaseDate = DateTime.Now
                };

                _context.TicketOrders.Add(ticketOrder);
                purchasedTickets.Add(ticketOrder);
            }

            await _context.SaveChangesAsync();
            var emailBody = GenerateTicketEmailHtml(eventEntity, purchasedTickets, user);
            _emailService.Send(user.Email, "Sizin EventTix Biletləriniz", emailBody, "no-reply@eventtix.az");


            return RedirectToAction("MyTickets");
        }

        [Authorize]
        public async Task<IActionResult> MyTickets()
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            var myTickets = await _context.TicketOrders
                .Include(t => t.Event)
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            return View(myTickets);
        }

        private string GenerateTicketEmailHtml(Event eventEntity, List<TicketOrder> tickets, AppUser user)
        {
            var sb = new System.Text.StringBuilder();

            sb.Append($@"
    <html>
    <head>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f4f4f7;
                color: #333;
                padding: 20px;
            }}
            .container {{
                background-color: #fff;
                border-radius: 8px;
                padding: 20px;
                max-width: 600px;
                margin: auto;
                box-shadow: 0 0 10px rgba(0,0,0,0.1);
            }}
            h2 {{
                color: #3a0ca3;
            }}
            table {{
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
            }}
            th, td {{
                border: 1px solid #ddd;
                padding: 12px;
                text-align: left;
            }}
            th {{
                background-color: #3a0ca3;
                color: white;
            }}
            .footer {{
                margin-top: 30px;
                font-size: 0.9em;
                color: #666;
                text-align: center;
            }}
            .btn {{
                display: inline-block;
                padding: 10px 20px;
                margin-top: 20px;
                background-color: #f72585;
                color: white;
                text-decoration: none;
                border-radius: 5px;
                font-weight: bold;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <h2>Hörmetli {user.Username},</h2>
            <p>Siz ugurla asagıdakı tedbir ucun bilet satin aldiniz:</p>

            <h3>{eventEntity.Title}</h3>
            <p><strong>Tarix:</strong> {eventEntity.CreatedDate.ToString("dd.MM.yyyy HH:mm")}</p>
            <p><strong>Yer:</strong> {eventEntity.Location}</p>

            <table>
                <thead>
                    <tr>
                        <th>Yer</th>
                        <th>Qiymət</th>
                        <th>Alış Tarixi</th>
                    </tr>
                </thead>
                <tbody>");

            foreach (var ticket in tickets)
            {
                sb.Append($@"
                    <tr>
                        <td>{ticket.SeatInfo}</td>
                        <td>{ticket.Price} AZN</td>
                        <td>{ticket.PurchaseDate.ToString("dd.MM.yyyy HH:mm")}</td>
                    </tr>");
            }

            sb.Append(@"
                </tbody>
            </table>

            <p>EventTix-e gösterdiyiniz etimada gorə tesekkur edirik!</p>

            <div class='footer'>
                <p>Bu email avtomatik göndərilir. Zəhmət olmasa cavab verməyin.</p>
                <a href='https://eventtix.az' class='btn'>Sayta Giriş</a>
            </div>
        </div>
    </body>
    </html>");

            return sb.ToString();
        }


    }
}
