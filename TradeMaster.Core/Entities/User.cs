using System.ComponentModel.DataAnnotations;

namespace TradeMaster.Core.Entities
{
    /// <summary>
    /// Represents a user account in the system.
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = UserRoles.Cashier;

        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(200)]
        [EmailAddress]
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastLoginDate { get; set; }
    }

    /// <summary>
    /// Predefined user roles.
    /// </summary>
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Cashier = "Cashier";

        public static readonly string[] AllRoles = { Admin, Manager, Cashier };
    }
}
