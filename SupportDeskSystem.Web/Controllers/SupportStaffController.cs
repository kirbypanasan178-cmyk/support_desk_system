using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SupportDeskSystem.Web.Data;
using SupportDeskSystem.Web.Enums;
using SupportDeskSystem.Web.Models;
using SupportDeskSystem.Web.Services;

namespace SupportDeskSystem.Web.Controllers
{
    [Authorize(Roles = "SupportStaff, Admin")]
    public class SupportStaffController : Controller
    {
        private readonly TicketService _ticketService;
        private readonly AppDbContext _context;

        public SupportStaffController(
            TicketService ticketService,
            AppDbContext context)
        {
            _ticketService = ticketService;
            _context = context;
        }

        // Support Staff Dashboard
        public async Task<IActionResult> Dashboard(
    string? search,
    TicketCategory? category,
    TicketPriority? priority,
    TicketStatus? status,
    int page = 1)
        {
            const int pageSize = 10;

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

            // Stats now reflect the FULL filtered set, not just the current page
            var stats = await _ticketService.GetTicketStatsAsync(search, category, priority, status);

            ViewBag.TotalTickets = stats.Total;
            ViewBag.OpenTickets = stats.Open;
            ViewBag.InProgressTickets = stats.InProgress;
            ViewBag.ResolvedTickets = stats.Resolved;

            // Pagination
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

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

        // Show Edit Ticket Page
        [HttpGet]
        public async Task<IActionResult> EditTicket(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            var supportStaff = _context.Users
                .Where(u => u.Role == UserRole.SupportStaff)
                .OrderBy(u => u.FullName)
                .ToList();

            ViewBag.SupportStaff = new SelectList(
                supportStaff,
                "Id",
                "FullName",
                ticket.AssignedToId
            );

            return View(ticket);
        }

        // Save Edited Ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTicket(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                var supportStaff = _context.Users
                    .Where(u => u.Role == UserRole.SupportStaff)
                    .OrderBy(u => u.FullName)
                    .ToList();

                ViewBag.SupportStaff = new SelectList(
                    supportStaff,
                    "Id",
                    "FullName",
                    ticket.AssignedToId
                );

                return View(ticket);
            }

            var existingTicket =
                await _ticketService.GetTicketByIdAsync(ticket.Id);

            if (existingTicket == null)
            {
                return NotFound();
            }

            existingTicket.Title = ticket.Title;
            existingTicket.Description = ticket.Description;
            existingTicket.Category = ticket.Category;
            existingTicket.Priority = ticket.Priority;
            existingTicket.Status = ticket.Status;
            existingTicket.AssignedToId = ticket.AssignedToId;

            await _ticketService.UpdateTicketAsync(existingTicket);

            return RedirectToAction(nameof(Dashboard));
        }

        // Delete Ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _ticketService.DeleteTicketAsync(id);

            return RedirectToAction(nameof(Dashboard));
        }
    }
}