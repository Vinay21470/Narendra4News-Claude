namespace Narendra4News.Domain.Enums;

// Mirrors ASP.NET Core Identity role names. Kept as constants (not just enum)
// because Identity roles are string-based.
public static class UserRoles
{
    public const string Admin = "Admin";
    public const string User = "User";
}
