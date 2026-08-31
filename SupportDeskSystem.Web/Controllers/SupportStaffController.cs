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
        public async Task<IActionResult> Dashboard()
        {
            var tickets = await _ticketService.GetAllAsync();

            ViewBag.TotalTickets = tickets.Count;
            ViewBag.OpenTickets = tickets.Count(t => t.Status == TicketStatus.Open);
            ViewBag.InProgressTickets = tickets.Count(t => t.Status == TicketStatus.InProgress);
            ViewBag.ResolvedTickets = tickets.Count(t => t.Status == TicketStatus.Resolved);

            return View(tickets);
        }

        // Ticket Details
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);

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
            var ticket = await _ticketService.GetByIdAsync(id);

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

            var existingTicket = await _ticketService.GetByIdAsync(ticket.Id);

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

            await _ticketService.UpdateAsync(existingTicket);

            return RedirectToAction(nameof(Dashboard));
        }

        // Delete Ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _ticketService.DeleteAsync(id);

            return RedirectToAction(nameof(Dashboard));
        }
    }
}