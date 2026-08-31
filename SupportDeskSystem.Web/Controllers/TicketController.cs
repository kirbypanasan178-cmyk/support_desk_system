using Microsoft.AspNetCore.Mvc;
using SupportDeskSystem.Web.Models;
using SupportDeskSystem.Web.Services;
using System.Security.Claims;

namespace SupportDeskSystem.Web.Controllers
{
    public class TicketController : Controller
    {
        private readonly TicketService _ticketService;

        public TicketController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: /Ticket
        public async Task<IActionResult> Index()
        {
            var tickets = await _ticketService.GetAllAsync();

            return View(tickets);
        }

        // GET: /Ticket/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        // GET: /Ticket/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (!ModelState.IsValid)
                return View(ticket);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            await _ticketService.CreateAsync(ticket, int.Parse(userId));

            return RedirectToAction(nameof(Index));
        }

        // GET: /Ticket/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        // POST: /Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ticket ticket)
        {
            if (id != ticket.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(ticket);

            var existingTicket = await _ticketService.GetByIdAsync(id);

            if (existingTicket == null)
                return NotFound();

            await _ticketService.UpdateAsync(ticket);

            return RedirectToAction(nameof(Index));
        }

        // GET: /Ticket/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        // POST: /Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _ticketService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}