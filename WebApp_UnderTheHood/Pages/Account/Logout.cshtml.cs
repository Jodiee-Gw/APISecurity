using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderTheHood.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            // Sign out the user by clearing the authentication cookie
            await HttpContext.SignOutAsync("MyCookieAuth");
            // Redirect to the login page after logout
            return RedirectToPage("/Account/Login");
        }
    }
}
