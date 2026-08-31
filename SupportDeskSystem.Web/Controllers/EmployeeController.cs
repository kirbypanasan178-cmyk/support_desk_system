using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportDeskSystem.Web.Models;
using SupportDeskSystem.Web.Services;

namespace SupportDeskSystem.Web.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly TicketService _ticketService;

        public EmployeeController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: /Employee/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var tickets = await _ticketService.GetAllAsync();

            ViewBag.TotalTickets = tickets.Count;
            ViewBag.OpenTickets = tickets.Count(t => t.Status == Enums.TicketStatus.Open);
            ViewBag.InProgressTickets = tickets.Count(t => t.Status == Enums.TicketStatus.InProgress);
            ViewBag.ResolvedTickets = tickets.Count(t => t.Status == Enums.TicketStatus.Resolved);

            return View(tickets);
        }

        // GET: /Employee/CreateTicket
        [HttpGet]
        public IActionResult CreateTicket()
        {
            return View();
        }

        // POST: /Employee/CreateTicket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            Console.WriteLine("========== POST CREATE TICKET HIT ==========");
            Console.WriteLine($"Title: {ticket.Title}");
            Console.WriteLine($"Description: {ticket.Description}");
            Console.WriteLine($"Category: {ticket.Category}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("========== MODEL STATE INVALID ==========");

                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                return View(ticket);
            }

            await _ticketService.CreateAsync(ticket);

            Console.WriteLine("========== TICKET SAVED ==========");

            return RedirectToAction("Dashboard");
        }

        // GET: /Employee/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
    }
}