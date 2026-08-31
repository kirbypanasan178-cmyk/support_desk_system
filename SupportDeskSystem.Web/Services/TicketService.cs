using Microsoft.EntityFrameworkCore;
using SupportDeskSystem.Web.Data;
using SupportDeskSystem.Web.Enums;
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

        // Get all tickets (paginated)
        public async Task<(List<Ticket> Tickets, int TotalCount)> GetAllTicketsAsync(
            string? search,
            TicketCategory? category,
            TicketPriority? priority,
            TicketStatus? status,
            int page = 1,
            int pageSize = 10
            )
        {
            var query = BuildFilteredQuery(search, category, priority, status);

            var totalCount = await query.CountAsync();

            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tickets, totalCount);
        }

        // Get status counts for the CURRENT filter, across ALL matching tickets (not just the current page)
        public async Task<(int Total, int Open, int InProgress, int Resolved)> GetTicketStatsAsync(
            string? search,
            TicketCategory? category,
            TicketPriority? priority,
            TicketStatus? status)
        {
            var query = BuildFilteredQuery(search, category, priority, status);

            var total = await query.CountAsync();
            var open = await query.CountAsync(t => t.Status == TicketStatus.Open);
            var inProgress = await query.CountAsync(t => t.Status == TicketStatus.InProgress);
            var resolved = await query.CountAsync(t => t.Status == TicketStatus.Resolved);

            return (total, open, inProgress, resolved);
        }

        private IQueryable<Ticket> BuildFilteredQuery(
            string? search,
            TicketCategory? category,
            TicketPriority? priority,
            TicketStatus? status)
        {
            var query = _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(normalizedSearch));
            }

            if (category.HasValue)
            {
                query = query.Where(t => t.Category == category.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            return query;
        }

        // Get ticket by ID
        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // Create ticket
        public async Task CreateTicketAsync(Ticket ticket, int userId)
        {
            ticket.CreatedById = userId;
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
        }

        // Update ticket
        public async Task UpdateTicketAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        // Delete ticket
        public async Task DeleteTicketAsync(int id)
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