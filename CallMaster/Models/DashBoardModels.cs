using CallMaster.Models.CallMaster.Models;

namespace CallMaster.Models
{
    public class AdminDashboardModel
    {
        public string AdminName { get; set; }
        public int TotalAgents { get; set; }
        public int TotalManagers { get; set; }
        public int TotalActiveCampaigns { get; set; }
        public int DailyCalls { get; set; }
        public float DailyIncreasedCallsPersent { get; set; }


    }

    public class AdminSharedView
    {
        public AdminDashboardModel DashboardData { get; set; }
        public IEnumerable<UserData> UsersData { get; set; }
        public IEnumerable<LicenceKeys> Licenses { get; set; }
        public IEnumerable<Campaign> Campaigns { get; set; } // Add this
        public IEnumerable<Team> Teams { get; set; }

    }

    public class AgentDashboardModel
    {
    }
    public class ManagerDashboardModel
    {
        public string ManagerName { get; set; }
        public List<AgentViewModel> ManagedAgents { get; set; }
        public List<CampaignPerformance> CampaignPerformances { get; set; } // New
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchName { get; set; }
        public IEnumerable<LicenceKeys> Licenses { get; set; } // Add this
    }

    public class CampaignPerformance
    {
        public string CampaignName { get; set; }
        public CampaignType Type { get; set; }
        public string Status { get; set; }

        // Common metrics
        public int TotalCalls { get; set; }
        public int CompletedCalls { get; set; }

        // Type-specific metrics
        public decimal? AverageCallsPerHour { get; set; } // For PowerDialer
        public decimal? SurveyCompletionRate { get; set; } // For SurveyFeedback
        public decimal? AverageSurveyDuration { get; set; }

        public string ConfigurationSummary { get; set; }
        public int TotalCallbacks { get; set; }
        public decimal CallbackConversionRate { get; set; }
        public int AverageCallbackAttempts { get; set; }
        public int PendingCalls { get; set; }
    }

    public class AgentViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public VoIPLoginDetails VoIPDetails { get; set; }
        public int AssignedCampaignsCount { get; set; } //
        public string TeamName { get; set; }
    }


    public class UserData
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime LastLogin { get; set; }
        public string Status { get; set; }
        public string ManagerName { get; set; }
        public int? TeamId { get; set; } // Add this
    }

}
