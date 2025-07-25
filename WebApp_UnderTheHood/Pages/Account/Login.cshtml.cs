using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebApp_UnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToPage("/Index"); // If already authenticated, redirect to the home page.

            return Page(); // Ensure all code paths return a value.
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Credential.UserName == "admin" && Credential.Password == "password")
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Credential.UserName),
                        new Claim(ClaimTypes.Role, "Administrator"),
                        new Claim("Department","HR"),
                        new Claim("Manager","true"),
                        new Claim("EmployeeDate", "2025-03-18")
                    };
                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authenticateProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };
                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authenticateProperties);

                return RedirectToPage("/Index");
            }

            return Page(); // If authentication fails, return to the login page with validation errors.
        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
