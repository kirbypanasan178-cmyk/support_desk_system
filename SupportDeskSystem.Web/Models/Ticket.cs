using SupportDeskSystem.Web.Enums;

namespace SupportDeskSystem.Web.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public TicketPriority Priority { get; set; }

        public TicketStatus Status { get; set; }

        public TicketCategory Category { get; set; }

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // A ticket can have multiple comments
        public ICollection<TicketComment> Comments { get; set; }
            = new List<TicketComment>();
    }
}