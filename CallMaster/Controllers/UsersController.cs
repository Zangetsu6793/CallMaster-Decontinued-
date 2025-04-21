using CallMaster.Data;
using CallMaster.Models;
using CallMaster.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Diagnostics;

public class UsersController : Controller
{
    private readonly CallMasterDbContext _context;
    private readonly IAuthenticationServices _authenticationService;

    public UsersController(CallMasterDbContext context, IAuthenticationServices authenticationService)
    {
        _context = context;
        _authenticationService = authenticationService;
    }

    // GET: Login Page
    public IActionResult Login()
    {
        return View();
        if (User.Identity.IsAuthenticated)  // Check if the user is already authenticated
        {
            foreach(var v in User.Claims)
            {
                Debug.WriteLine(v.Type + " : " + v.Value);
            }
            if (User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
            {
                return RedirectToAction("AdminDashboard", "Caller");
            }
            else if (User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Agent"))
            {
                return RedirectToAction("AgentDashboard", "Caller");
            }
            else if(User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Manager"))
            {
                RedirectToAction("ManagerDashboard", "Caller");
            }
            else
            {
                return View(); // Redirect to the Login if authenticated
            }
            
        }
        return View();
    }

    // POST: Handle Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string? licenseKey)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "Email and Password are required.";
            return View();
        }

        var (success, reason) = await _authenticationService.Login(email, password, licenseKey, HttpContext);

        if (!success && (reason == "NoAccount" || reason == "LicenseInvalid"))
        {
            if (string.IsNullOrEmpty(licenseKey))
            {
                ViewBag.RequireLicense = true;
                ViewBag.Error = "No account found. Please enter a license key to register.";
                return View();
            }

            await _authenticationService.RegisterAccount(email, password, licenseKey, HttpContext, true);

            // Set flag to show the second form
            ViewBag.ShowSecondForm = true;

            // Pass user model to the view for completing registration
            var newUser = new User { Email = email };
            return View("Login", newUser); // Use the same Login view to show the second form
        }

        if (!success && reason == "InvalidCred")
        {
            ViewBag.Error = "Invalid credentials. Please try again.";
            return View();
        }

        if (!success && reason == "InactiveAccount")
        {
            ViewBag.Error = "This Account is disabled.";
            return View();
        }

        if (success)
        {
            // Define the user claims
            var loginClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email)
        };

            // Add role to the claims based on the login reason
            string userRole = string.Empty;

            if (reason == "AdminLoggedIn")
            {
                userRole = "Admin";
            }
            else if (reason == "ManagerLoggedIn")
            {
                userRole = "Manager";
            }
            else if (reason == "AgentLoggedIn")
            {
                userRole = "Agent";
            }

            if (!string.IsNullOrEmpty(userRole))
            {
                loginClaims.Add(new Claim(ClaimTypes.Role, userRole));

                // Create the identity and principal for the user
                var loginIdentity = new ClaimsIdentity(loginClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var loginPrincipal = new ClaimsPrincipal(loginIdentity);

                // Sign in the user with a persistent cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, loginPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,  // Keep the user logged in even after closing the browser
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)  // Set cookie expiration to 7 days
                });

                // Redirect to the appropriate dashboard based on role
                if (userRole == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Caller");
                }
                else if (userRole == "Manager")
                {
                    return RedirectToAction("ManagerDashboard", "Caller");
                }
                else if (userRole == "Agent")
                {
                    return RedirectToAction("AgentDashboard", "Caller");
                }
            }
        }

        ViewBag.Error = "Invalid role assigned to this account, contact support.";
        return View();
    }


    // POST: Handle User Info Completion after Registration
    [HttpPost]
    public async Task<IActionResult> CompleteRegistration(string firstName, string surname, string phoneNumber, string email)
    {
        // Ensure the email matches the currently authenticated user
        if (email != User.Identity.Name)
        {
            ViewBag.Error = "You can only update your own account.";
            return RedirectToAction("Login", "Users");
        }

        // Update user information
        var (success, reason) = await _authenticationService.UpdateRegisterAccountInfo(email, firstName, surname, phoneNumber, HttpContext);
        if (!success)
        {
            ViewBag.Error = reason;
            return  RedirectToAction("Login", "Users"); ;
        }

        // Redirect to the Dashboard after successful update
        return RedirectToAction("Login", "Users");
    }
}
