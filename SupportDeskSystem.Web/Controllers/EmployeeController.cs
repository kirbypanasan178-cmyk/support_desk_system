using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportDeskSystem.Web.Enums;
using SupportDeskSystem.Web.Models;
using SupportDeskSystem.Web.Services;
using System.Security.Claims;

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
        public async Task<IActionResult> Dashboard(
             string? search,
             TicketCategory? category,
             TicketPriority? priority,
             TicketStatus? status,
             int page = 1)
        {
            const int pageSize = 10;

            // Prevent invalid page numbers
            if (page < 1)
            {
                page = 1;
            }

            var result = await _ticketService.GetAllTicketsAsync(
                search,
                category,
                priority,
                status,
                page,
                pageSize
            );

            var tickets = result.Tickets;
            var totalCount = result.TotalCount;

            ViewBag.TotalTickets = totalCount;

            ViewBag.OpenTickets = tickets.Count(t =>
                t.Status == TicketStatus.Open);

            ViewBag.InProgressTickets = tickets.Count(t =>
                t.Status == TicketStatus.InProgress);

            ViewBag.ResolvedTickets = tickets.Count(t =>
                t.Status == TicketStatus.Resolved);

            // Pagination
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(
                (double)totalCount / pageSize
            );

            // Keep filter values for the view
            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Priority = priority;
            ViewBag.Status = status;

            return View(tickets);
        }

        // Ticket Details
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
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
          

            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            await _ticketService.CreateTicketAsync(ticket, int.Parse(userId));
         
            return RedirectToAction("Dashboard");
        }

        // GET: /Employee/Details/5
    }
}