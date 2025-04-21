using System.ComponentModel.DataAnnotations;

namespace CallMaster.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? pName { get; set; }
        public string? Surname { get; set; }
        public string? PersonalPhoneNumber { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }

        // Role (Admin, Manager, Agent)
        public string Role { get; set; }

        // ManagerId for Agents
        public int? ManagerId { get; set; }
        public virtual User Manager { get; set; }

        public virtual VoIPLoginDetails VoIPDetails { get; set; }
        public DateTime LastLogin { get; set; }
        public string Status { get; set; }

        // Link to a License Key
        public int? LicenceKeyId { get; set; }
        public virtual LicenceKeys LicenceKey { get; set; }  // Navigation Property
        public int? TeamId { get; set; }
        public virtual Team Team { get; set; }
    }


    public class  VoIPLoginDetails
    {
        [Key]
        [Required(ErrorMessage = "Inbound Number is required")]
        public string InboundNumber { get; set; }

        [Required(ErrorMessage = "Extension is required")]
        public string Extension { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Agent { get; set; }
        public string Principal { get; set; }
        public string Received { get; set; }

        // Foreign key to the User who owns these VoIP details
        public int? UserId { get; set; }

        // Navigation property to the related User
        public virtual User User { get; set; } // One-to-one navigation back to the User
    }

    public class LicenceKeys
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Role { get; set; } // Added property
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UsuableAcounts { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    // Add to Models/User.cs
    public class Team
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }


}

