using Microsoft.AspNetCore.Identity;

namespace Narendra4News.Domain.Entities;

// Extends ASP.NET Core Identity's IdentityUser. Identity already provides
// Id, Email, UserName, PasswordHash, etc. Role-based authorization is handled
// via IdentityRole ("Admin" / "User") rather than a custom Role entity, which
// avoids re-implementing password hashing / security stamp / lockout logic.
public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<Article> Articles { get; set; } = new List<Article>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
