using CallMaster.Data;
using CallMaster.Models;
using CallMaster.Models.CallMaster.Models;
using CallMaster.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging;
using System.Linq;

namespace CallMaster.Controllers
{
    public class CallerController : Controller
    {
        private readonly CallMasterDbContext _context;
        private readonly CallMaster.Services.IAuthenticationServices _authenticationServices;

        public CallerController(CallMasterDbContext context, CallMaster.Services.IAuthenticationServices authenticationServices)
        {
            _context = context;
            _authenticationServices = authenticationServices;
            // Replace all instances of:
            
        }

        // Dashboard for Agents
        [Authorize(Roles = "Agent")]
        public IActionResult AgentDashboard()
        {
            var UserInfo = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            UserInfo.Salt = null; // Remove the salt from the view
            UserInfo.PasswordHash = null; // Remove the password hash from the view
            return View(UserInfo);
        }

        // Dashboard for Admins and Managers
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard(int page = 1, string searchName = null, string roleFilter = null, string statusFilter = null)
        {
            var UserInfo = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);

            // Pagination settings
            int pageSize = 10; // Number of users to display per page
            int skipCount = (page - 1) * pageSize; // Number of users to skip

            // Base query
            var query = _context.Users.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(u => (u.pName + " " + u.Surname).Contains(searchName));
            }

            if (!string.IsNullOrEmpty(roleFilter))
            {
                query = query.Where(u => u.Role == roleFilter);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(u => u.Status == statusFilter);
            }


            // Fetch Manager's name if the user is an Agent or Manager
            var manager = _context.Users.FirstOrDefault(u => u.Id == UserInfo.ManagerId); // Assuming there's a ManagerId field
            string managerName = manager?.pName + " " + manager?.Surname;

            // Fetch the number of calls for today and yesterday
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            var dailyCalls = 0; // _context.CallLogs.Count(c => c.Date >= today && c.Date < today.AddDays(1));
            var yesterdayCalls = 0; // _context.CallLogs.Count(c => c.Date >= yesterday && c.Date < today);

            // Fetch all campaigns
            var campaigns = _context.Campaigns
        .Include(c => c.AssignedAgents)
        .Include(c => c.PhoneNumbers)
        .ToList();

            // Calculate percentage increase in calls
            float dailyIncreasePercentage = 0.0f;
            if (yesterdayCalls > 0)
            {
                dailyIncreasePercentage = ((float)(dailyCalls - yesterdayCalls) / yesterdayCalls) * 100;
            }

            // Prepare the data for the shared view
            var SharedView = new AdminSharedView
            {
                DashboardData = new AdminDashboardModel
                {
                    AdminName = UserInfo.pName + " " + UserInfo.Surname,
                    TotalAgents = _context.Users.Count(c => c.Role == "Agent"),
                    TotalManagers = _context.Users.Count(c => c.Role == "Manager"),
                    TotalActiveCampaigns = _context.Campaigns.Count(c => c.Status == Models.CallMaster.Models.CampaignStatus.Active),
                    DailyCalls = dailyCalls,
                    DailyIncreasedCallsPersent = dailyIncreasePercentage,

                },
                UsersData = new List<UserData>(),
                Licenses = _context.LicenceKeys.ToList(),
                Campaigns = campaigns,
                Teams = _context.Teams.Include(t => t.Users).ToList()
            };

            // Retrieve the users for the current page (pagination)
            var allUsers = query
                .OrderBy(u => u.pName)
                .Skip(skipCount)
                .Take(pageSize)
                .ToList();

            foreach (var user in allUsers)
            {
                var userManager = _context.Users.FirstOrDefault(u => u.Id == user.ManagerId);
                string userManagerName = userManager?.pName + " " + userManager?.Surname;
                var userTeam = _context.Teams.FirstOrDefault(t => t.Id == user.TeamId);
                // Create UserData for each user
                var userData = new UserData
                {
                    Id = user.Id,
                    FullName = user.pName + " " + user.Surname,
                    Role = user.Role,
                    LastLogin = user.LastLogin,
                    Status = user.Status,
                    ManagerName = userManagerName,
                    TeamId = user.TeamId
                };

                // Add to the UsersData collection
                ((List<UserData>)SharedView.UsersData).Add(userData);
            }

            // Calculate total pages for pagination
            int totalUsers = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            // Store filter parameters in ViewBag for pagination
            ViewBag.SearchName = searchName;
            ViewBag.RoleFilter = roleFilter;
            ViewBag.StatusFilter = statusFilter;

            // Add pagination info
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Return the view with the populated model
            return View(SharedView);
        }




        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ToggleUserStatus(int userId, string status, int page, string searchName, string roleFilter, string statusFilter)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Status = status; // Set user status to Active or Inactive
                _context.SaveChanges();
            }
            if (page < 1) page = 1;

            // Construct the URL with query parameters and hash fragment
            var url = Url.Action("AdminDashboard", new
            {
                page = page,
                searchName = searchName,
                roleFilter = roleFilter,
                statusFilter = statusFilter
            }) + "#userManagement";

            return Redirect(url);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Filter(
    int userId,
    string fullName,
    string email,
    string role,
    int? teamId, // Add teamId parameter
    int page,
    string searchName,
    string roleFilter,
    string statusFilter)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                // Check if email is being changed
                if (user.Email != email)
                {
                    // Verify new email doesn't exist
                    var emailExists = _context.Users.Any(u => u.Email == email);
                    if (emailExists)
                    {
                        TempData["Error"] = "Email already belongs to another user";
                        return RedirectToAction("AdminDashboard", new { page, searchName, roleFilter, statusFilter });
                    }
                }

                var nameParts = fullName.Split(' ');
                user.pName = nameParts[0];
                user.Surname = nameParts.Length > 1 ? nameParts[1] : "";
                user.Email = email;
                user.Role = role;
                user.TeamId = teamId; // Add team assignment

                _context.SaveChanges();
            }

            return RedirectToAction("AdminDashboard", new
            {
                page,
                searchName,
                roleFilter,
                statusFilter
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult GenerateLicense(string role, DateTime expiresAt, int usableAccounts)
        {
            var license = new LicenceKeys
            {
                Key = GenerateLicenseKey(),
                Role = role,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                UsuableAcounts = usableAccounts
            };
            _context.LicenceKeys.Add(license);
            _context.SaveChanges();
            return RedirectToAction("AdminDashboard");
        }

        private string GenerateLicenseKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 16)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(
    string firstName,
    string surname,
    string email,
    string password,
    string phoneNumber,
    string licenseKey,
    int teamId,
    int page = 1,
    string searchName = null,
    string roleFilter = null,
    string statusFilter = null
    )
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                TempData["Error"] = "Email already registered!";
                return Redirect(Url.Action("AdminDashboard") + "#userManagement");
            }

            var license = await _context.LicenceKeys
                .FirstOrDefaultAsync(l => l.Key == licenseKey && l.Status == "Active");

            if (license == null || license.UsuableAcounts <= 0)
            {
                TempData["Error"] = "Invalid or exhausted license key.";
                return Redirect(Url.Action("AdminDashboard") + "#userManagement");
            }

            var (success, message) = await _authenticationServices.RegisterAccount(
                email,
                password,
                licenseKey,
                HttpContext
            );

            if (!success)
            {
                TempData["Error"] = message;
                return Redirect(Url.Action("AdminDashboard") + "#userManagement");
            }

            // Update user details
            var newUser = await _context.Users.FirstAsync(u => u.Email == email);
            newUser.TeamId = teamId;
            newUser.pName = firstName;
            newUser.Surname = surname;
            newUser.PersonalPhoneNumber = phoneNumber;
            newUser.Status = "Active";  // Set default status

            // Decrement license count
            license.UsuableAcounts--;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User created with role: {license.Role}";
            return Redirect(Url.Action("AdminDashboard", new
            {
                page = page,
                searchName = searchName,
                roleFilter = roleFilter,
                statusFilter = statusFilter
            }) + "#userManagement");
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateTeam(string teamName)
        {
            try
            {
                _context.Teams.Add(new Team { Name = teamName });
                _context.SaveChanges();
                TempData["Success"] = "Team created successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating team: {ex.Message}";
            }
            return RedirectToAction("AdminDashboard", new { hash = "teamManagement" });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteTeam(int teamId)
        {
            try
            {
                var team = _context.Teams
                    .Include(t => t.Users)
                    .FirstOrDefault(t => t.Id == teamId);

                if (team?.Users.Any() == true)
                {
                    TempData["Error"] = "Cannot delete team with members";
                    return RedirectToAction("AdminDashboard", new { hash = "teamManagement" });
                }

                _context.Teams.Remove(team);
                _context.SaveChanges();
                TempData["Success"] = "Team deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting team: {ex.Message}";
            }
            return RedirectToAction("AdminDashboard", new { hash = "teamManagement" });
        }



        [Authorize(Roles = "Manager")]
        public IActionResult ManagerDashboard(int page = 1, string searchName = null)
        {
            var currentManager = _context.Users
                .Include(u => u.Team)
                .FirstOrDefault(u => u.Email == User.Identity.Name);

            // Pagination settings
            int pageSize = 10;
            int skipCount = (page - 1) * pageSize;

            var query = _context.Users
        .Include(u => u.VoIPDetails)
        .Include(u => u.Team) // Load agent's team
        .Where(u => u.Role == "Agent" && u.TeamId == currentManager.TeamId);

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(u => (u.pName + " " + u.Surname).Contains(searchName));
            }

            var managedAgents = query
         .OrderBy(u => u.pName)
         .Skip(skipCount)
         .Take(pageSize)
         .ToList();

            // Get campaigns assigned to managed agents
            var managedAgentIds = managedAgents.Select(a => a.Id).ToList();
            var campaigns = _context.Campaigns
                .Include(c => c.AssignedAgents)
                .Include(c => c.PhoneNumbers)
                .Where(c => c.AssignedAgents.Any(a => managedAgentIds.Contains(a.Id)))
                .ToList();

            // Calculate campaign performance
            var campaignPerformances = campaigns.Select(c => new CampaignPerformance
            {
                CampaignName = c.Name,
                TotalCalls = c.PhoneNumbers.Count,
                CompletedCalls = c.PhoneNumbers.Count(p => p.Outcome == CallOutcome.Completed),
                PendingCalls = c.PhoneNumbers.Count(p => p.Outcome == CallOutcome.Pending),
                Status = c.Status.ToString()
            }).ToList();

            // Build AgentViewModels with assigned campaign count
            var agentViewModels = managedAgents.Select(a => new AgentViewModel
            {
                Id = a.Id,
                FullName = $"{a.pName} {a.Surname}",
                Email = a.Email,
                Status = a.Status ?? "Unknown",
                VoIPDetails = a.VoIPDetails ?? new VoIPLoginDetails(),
                AssignedCampaignsCount = _context.Campaigns
                    .Count(c => c.AssignedAgents.Any(ag => ag.Id == a.Id)),
                TeamName = a.Team?.Name ?? "No Team"
            }).ToList();

            var model = new ManagerDashboardModel
            {
                ManagerName = $"{currentManager.pName} {currentManager.Surname}",
                ManagedAgents = agentViewModels,
                CampaignPerformances = campaignPerformances,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)query.Count() / pageSize),
                SearchName = searchName,
                Licenses = _context.LicenceKeys
            .Where(l => l.Role == "Agent" &&
                      l.Status == "Active" &&
                      l.UsuableAcounts > 0)
            .ToList()
            };

            return View(model);
        }

        private User GetCurrentManager()
        {
            return _context.Users
                .FirstOrDefault(u => u.Email == User.Identity.Name);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult UpdateAgent(
    int agentId,
    string fullName,
    string email,
    string inboundNumber,
    string extension,
    string userName,
    string password,
    string principal,
    string received)
        {
            var currentManager = _context.Users
                .FirstOrDefault(u => u.Email == User.Identity.Name);
            var agent = _context.Users
                .Include(u => u.VoIPDetails)
                .FirstOrDefault(u => u.Id == agentId);

            if (agent == null || agent.ManagerId != currentManager?.Id) return Forbid();

            // Update basic info
            var nameParts = fullName.Split(' ');
            agent.pName = nameParts[0];
            agent.Surname = nameParts.Length > 1 ? nameParts[1] : "";
            agent.Email = email;

            // Handle VoIP details
            if (!string.IsNullOrWhiteSpace(inboundNumber) || !string.IsNullOrWhiteSpace(extension))
            {
                var existingVoIP = _context.VoIPLoginDetails
                    .FirstOrDefault(v =>
                        (v.InboundNumber == inboundNumber || v.Extension == extension) &&
                        v.UserId != agentId);

                if (existingVoIP != null)
                {
                    ModelState.AddModelError("", "VoIP number or extension already in use");
                    return RedirectToAction("ManagerDashboard");
                }

                agent.VoIPDetails ??= new VoIPLoginDetails();
                agent.VoIPDetails.InboundNumber = inboundNumber;
                agent.VoIPDetails.Extension = extension;
                agent.VoIPDetails.UserName = userName;
                agent.VoIPDetails.Password = password;
                agent.VoIPDetails.Agent = $"{agent.pName} {agent.Surname}";
                agent.VoIPDetails.Principal = principal;
                agent.VoIPDetails.Received = received;
                agent.VoIPDetails.UserId = agent.Id;
            }
            else if (agent.VoIPDetails != null)
            {
                _context.VoIPLoginDetails.Remove(agent.VoIPDetails);
                agent.VoIPDetails = null;
            }

            _context.SaveChanges();
            return RedirectToAction("ManagerDashboard");
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateAgent(
    string firstName,
    string surname,
    string email,
    string Apassword,
    string phoneNumber,
    string inboundNumber,
    string extension,
    string userName,
    string password,
    string principal,
    string received,
    string licenseKey)
        {
            try
            {
                var currentManager = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                if (currentManager == null) return Forbid();

                // Validate Agent license
                var license = await _context.LicenceKeys
                    .FirstOrDefaultAsync(l => l.Key == licenseKey && l.Role == "Agent");
                if (license == null || license.UsuableAcounts <= 0)
                {
                    TempData["Error"] = "Invalid Agent license.";
                    return RedirectToAction("ManagerDashboard");
                }

                // Validate VoIP uniqueness
                if (!string.IsNullOrWhiteSpace(inboundNumber) || !string.IsNullOrWhiteSpace(extension))
                {
                    bool existing = await _context.VoIPLoginDetails
                        .AnyAsync(v => v.InboundNumber == inboundNumber || v.Extension == extension);
                    if (existing)
                    {
                        TempData["Error"] = "VoIP number or extension already in use";
                        return RedirectToAction("ManagerDashboard");
                    }
                }

                // Create the agent
                var agent = new User
                {
                    pName = firstName,
                    Surname = surname,
                    Email = email,
                    PasswordHash = Apassword,
                    PersonalPhoneNumber = phoneNumber,
                    Role = "Agent",
                    ManagerId = currentManager.Id,
                    Status = "Active",
                    LastLogin = DateTime.Now,
                    LicenceKeyId = license.Id, // Assign the license
                    TeamId = currentManager.TeamId
                };

                // Register the agent (this will hash the password)
                var (registrationSuccess, registrationMessage) = await _authenticationServices.RegisterA(agent);
                if (!registrationSuccess)
                {
                    TempData["Error"] = registrationMessage;
                    return RedirectToAction("ManagerDashboard");
                }

                // Add VoIP details if provided
                if (!string.IsNullOrWhiteSpace(inboundNumber) || !string.IsNullOrWhiteSpace(extension))
                {
                    var voipDetails = new VoIPLoginDetails
                    {
                        InboundNumber = inboundNumber,
                        Extension = extension,
                        UserName = userName,
                        Password = password,
                        Agent = $"{firstName} {surname}",
                        Principal = principal,
                        Received = received,
                        UserId = agent.Id
                    };
                    _context.VoIPLoginDetails.Add(voipDetails);
                }

                // Decrement license and save changes
                license.UsuableAcounts--;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Agent created successfully";
                return RedirectToAction("ManagerDashboard");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating agent: {ex.Message}";
                return RedirectToAction("ManagerDashboard");
            }
        }




        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult ToggleAgentStatus(int agentId, string status, int page, string searchName)
        {
            var currentManager = _context.Users
                .Include(u => u.Team)
                .FirstOrDefault(u => u.Email == User.Identity.Name);

            // Change this check
            var agent = _context.Users
                .FirstOrDefault(u => u.Id == agentId && u.TeamId == currentManager.TeamId); // Team-based check

            if (agent != null)
            {
                agent.Status = status;
                _context.SaveChanges();
            }

            return RedirectToAction("ManagerDashboard", new { page, searchName });
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult CreateCampaign(
    [FromForm] string campaignType,
    [FromForm] string name)
        {
            try
            {
                var manager = GetCurrentManager();

                var campaign = new Campaign
                {
                    Name = name,
                    Type = Enum.Parse<CampaignType>(campaignType),
                    Status = CampaignStatus.Draft,
                    AssignedAgents = new List<User>()
                };

                _context.Campaigns.Add(campaign);
                _context.SaveChanges();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult AssignAgentsToCampaign(int campaignId, [FromBody] List<int> agentIds)
        {
            var campaign = _context.Campaigns
                .Include(c => c.AssignedAgents)
                .FirstOrDefault(c => c.Id == campaignId);

            var agents = _context.Users
                .Where(u => agentIds.Contains(u.Id) && u.TeamId == GetCurrentManager().TeamId)
                .ToList();

            campaign.AssignedAgents.Clear();
            campaign.AssignedAgents = agents;

            _context.SaveChanges();
            return Ok();
        }

//        [HttpPost]
//        [Authorize(Roles = "Manager")]
//        [HttpPost]
//[Authorize(Roles = "Manager")]
//public IActionResult GenerateCallbackCampaign(int campaignId)
//{
//    var campaign = _context.Campaigns
//        .Include(c => c.PhoneNumbers)
//        .FirstOrDefault(c => c.Id == campaignId);

//    if (campaign?.Type != CampaignType.Callback)
//        return BadRequest("Invalid campaign type");

//    // Deserialize using Newtonsoft.Json
//    var config = JsonConvert.DeserializeObject<CallbackCampaignConfig>(campaign.Configuration);
    
//    if (config == null)
//        return BadRequest("Invalid campaign configuration");

//    var callbackNumbers = _context.CampaignPhoneNumbers
//        .Where(p => p.Outcome == CallOutcome.Callback &&
//                   p.CampaignId != campaignId &&
//                   p.CreatedAt >= DateTime.UtcNow.AddDays(-config.DaysToLookBack))
//        .OrderBy(p => config.PrioritizeOlderCallbacks ? p.CreatedAt : Guid.NewGuid())
//        .Take(1000)
//        .ToList();

//    campaign.PhoneNumbers.Clear();
//    campaign.PhoneNumbers.AddRange(callbackNumbers);
//    campaign.LastGenerated = DateTime.UtcNow;

//    _context.SaveChanges();

//    return Ok(new { count = callbackNumbers.Count });
//}

    }
}