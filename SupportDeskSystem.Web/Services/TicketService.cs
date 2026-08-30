using Microsoft.EntityFrameworkCore;
using SupportDeskSystem.Web.Data;
using SupportDeskSystem.Web.Models;

namespace SupportDeskSystem.Web.Services
{
    public class TicketService
    {
        private readonly AppDbContext _context;

        public TicketService(AppDbContext context)
        {
            _context = context;
        }

        // Get all tickets
        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                .OrderByDescending(t => t.Id)
                .ToListAsync();
        }

        // Get ticket by ID
        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }


        // Create ticket
        public async Task CreateAsync(Ticket ticket)
        {
            ticket.CreatedById = 2;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
        }

        // Update ticket
        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        // Delete ticket
        public async Task DeleteAsync(int id)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }


    }
}