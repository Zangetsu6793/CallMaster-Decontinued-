using CallMaster.Data;
using CallMaster.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CallMaster.Services
{
    public interface IAuthenticationServices
    {
        Task<(bool, string)> Login(string username, string password, string? ProductKey, HttpContext context);
        Task<(bool, string)> RegisterAccount(string username,string password,string? productKey,HttpContext context,bool autoLogin = false);
        Task<(bool, string)> UpdateRegisterAccountInfo(string email, string firstName, string surname, string phoneNumber, HttpContext context);
        Task<(bool, string)> RegisterA(User Account);
    }


    public class AuthenticationService : IAuthenticationServices
    {
        private readonly CallMaster.Models.Settings _settings;
        private readonly CallMasterDbContext _context;

        public AuthenticationService(CallMaster.Models.Settings settings, CallMasterDbContext context)
        {
            _settings = settings;
            _context = context;
        }

        public async Task<(bool, string)> RegisterAccount(string username, string password, string? productKey, HttpContext context, bool autoLogin = false)
        {
            if (string.IsNullOrEmpty(productKey))
                return (false, "A license key is required to create an account.");

            // Check if the license key exists
            var license = await _context.LicenceKeys.FirstOrDefaultAsync(l => l.Key == productKey);

            if (license == null)
                return (false, "Invalid license key.");

            if (license.Status != "Active" || license.ExpiresAt < DateTime.UtcNow)
                return (false, "This license key is expired or revoked.");

            if (license.UsuableAcounts <= 0)
                return (false, "This license key has reached its activation limit.");

            var newUser = new User
            {
                Email = username,
                PasswordHash = password,
                Role = license.Role,
                Status = "Active",
                LicenceKeyId = license.Id // Assigning license to the user
            };

            newUser.ProvideSaltAndHash();

            _context.Users.Add(newUser);

            // Reduce available activations
            license.UsuableAcounts -= 1;

            await _context.SaveChangesAsync();

            if (autoLogin)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, newUser.Role) // Include role claim
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await context.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7)
                    });
            }

            return (true, "AccountRegistered");
        }


        public async Task<(bool, string)> Login(string username, string password, string? productKey, HttpContext context)
        {
            var user = await _context.Users.Include(u => u.LicenceKey).FirstOrDefaultAsync(u => u.Email == username);

            if (user == null)
                return (false, "NoAccount");
            var hash = AuthenticationHelpers.ComputeHash(password, user.Salt);
            if (user.PasswordHash != hash)
                return (false, "InvalidCred");

            if (user.Status == "Inactive")
                return (false, "InactiveAccount");

            // Check if the license is still valid
            if (user.LicenceKey == null || user.LicenceKey.Status != "Active" || user.LicenceKey.ExpiresAt < DateTime.UtcNow)
                return (false, "LicenseInvalid");

            if (user.Role == "Admin")
                return (true, "AdminLoggedIn");
            else if (user.Role == "Agent")
                return (true, "AgentLoggedIn");
            else if (user.Role == "Manager")
                return (true, "ManagerLoggedIn");

            return (false, "InvalidRole");
        }


        public async Task<(bool, string)> Logout(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return (true, "LoggedOut");
        }

        public async Task<(bool,string)> UpdateRegisterAccountInfo(string email, string firstName, string surname, string phoneNumber, HttpContext context)
        {
            // Find the user in the database by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return (false, "User Not Found");

            // Update the user's details
            user.pName = firstName;
            user.Surname = surname;
            user.PersonalPhoneNumber = phoneNumber;

            // Save the changes to the database
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return (true, "AccountUpdated");
            // Redirect to the dashboard after successful update
        }
        public async Task<(bool, string)> RegisterA(User account)
        {
            try
            {
                account.ProvideSaltAndHash();
                _context.Users.Add(account);
                await _context.SaveChangesAsync(); // This generates the ID
                return (true, "Account registered successfully");
            }
            catch (DbUpdateException ex)
            {
                return (false, $"Database error: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
        private int GenerateUniqueManagerId()
        {
            // This can be customized based on how you want to generate unique IDs
            return new Random().Next(1000, 9999);  // Generates a random number between 1000 and 9999
        }
    }
}


    public static class AuthenticationHelpers
    {
        public static void ProvideSaltAndHash(this User user)
        {
            var salt = GenerateSalt();
            user.Salt = Convert.ToBase64String(salt);
            user.PasswordHash = ComputeHash(user.PasswordHash, user.Salt);
        }

        private static byte[] GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var salt = new byte[24];
            rng.GetBytes(salt);
            return salt;
        }

        public static string ComputeHash(string password, string saltString)
        {
            var salt = Convert.FromBase64String(saltString);

            using var hashGenerator = new Rfc2898DeriveBytes(password, salt);
            hashGenerator.IterationCount = 10101;
            var bytes = hashGenerator.GetBytes(24);
            return Convert.ToBase64String(bytes);
        }
    }
