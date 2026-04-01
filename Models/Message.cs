public class Message
{
    public long Id { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; }
    public User User { get; set; }

    public Guid ChannelId { get; set; }
    public Channel Channel { get; set; }
}