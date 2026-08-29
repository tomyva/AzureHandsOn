namespace AzureHandsOn.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string CustomerName { get; set; } = "";
        public string Subject { get; set; } = "";
        public string? Description { get; set; }
        public string Priority { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public string? AttachmentFileName { get; set; }
        public string? AttachmentBlobName { get; set; }
    }
}
