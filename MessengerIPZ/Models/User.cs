using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string? AvatarUrl { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }

    public ICollection<ChannelMember> SubscribedChannels { get; set; }
}