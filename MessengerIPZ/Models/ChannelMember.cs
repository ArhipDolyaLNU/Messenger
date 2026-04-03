public class ChannelMember
{
    public string UserId { get; set; }
    public User User { get; set; }

    public Guid ChannelId { get; set; }
    public Channel Channel { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}