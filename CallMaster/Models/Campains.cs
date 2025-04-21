namespace CallMaster.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace CallMaster.Models
    {
        public class Campaign
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public string Name { get; set; }

            public CampaignType Type { get; set; }

            // Common properties
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? LastGenerated { get; set; }
            public CampaignStatus Status { get; set; } = CampaignStatus.Active;
            public DateTime? CompletionDate { get; set; }

            // Type-specific properties (nullable)
            // For Callback Campaigns
            public int? DaysToLookBack { get; set; }
            public decimal? RequiredCallbackRate { get; set; }
            public bool? PrioritizeOlderCallbacks { get; set; }

            // For PowerDialer Campaigns
            public int? MinCallsPerHour { get; set; }
            public bool? AutoDialEnabled { get; set; }
            public int? MaxAttemptsPerNumber { get; set; }

            // Relationships
            public virtual ICollection<CampaignPhoneNumber> PhoneNumbers { get; set; } = new List<CampaignPhoneNumber>();
            public virtual ICollection<User> AssignedAgents { get; set; } = new List<User>();
            public string Configuration { get; set; }
        }

        public enum CampaignType
        {
            Normal,
            PowerDialer,
            Callback
        }

        // Add to CampaignStatus enum
        public enum CampaignStatus
        {
            Draft,
            Active,
            Paused,
            Completed
        }
        public class CampaignPhoneNumber
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [Phone]
            public string PhoneNumber { get; set; }

            // Outcome per number
            public CallOutcome Outcome { get; set; } = CallOutcome.Pending;

            // Foreign key to the Campaign
            public int CampaignId { get; set; }
            public virtual Campaign Campaign { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Add this
        }

        // Enum for Call Outcomes
        public enum CallOutcome
        {
            Pending,
            WrongPartyContact,
            DebtUnknownToDebtor,
            Callback,
            Completed,
            NoAnswer
        }

        public class NormalCampaignConfig
        {
            public bool RequireClientValidation { get; set; }
            public bool TrackCustomerSatisfaction { get; set; }
        }

        public class PowerDialerConfig
        {
            [Range(1, 100)]
            public int MinCallsPerHour { get; set; }
            public bool AutoDialEnabled { get; set; }
            public int MaxAttemptsPerNumber { get; set; } = 3;
        }

        public class CallbackCampaignConfig
        {
            [Range(1, 7)]
            public int DaysToLookBack { get; set; } = 1;

            [Range(1, 100)]
            public decimal RequiredCallbackRate { get; set; } = 80;

            public bool PrioritizeOlderCallbacks { get; set; } = true;
        }
    }

}
