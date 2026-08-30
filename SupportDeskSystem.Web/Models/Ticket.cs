using SupportDeskSystem.Web.Enums;

namespace SupportDeskSystem.Web.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public TicketCategory Category { get; set; }

        public TicketPriority Priority { get; set; } = TicketPriority.Unspecified;

        public TicketStatus Status { get; set; } = TicketStatus.Open;

        public int CreatedById { get; set; }
        public User? CreatedBy { get; set; } = null!;

        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<TicketComment> Comments { get; set; }
            = new List<TicketComment>();
    }
}