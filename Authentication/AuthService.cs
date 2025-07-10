using Microsoft.AspNetCore.DataProtection;

namespace Authentication
{
    public class AuthService
    {
        private readonly IDataProtectionProvider idp;
        private readonly IHttpContextAccessor accessor;

        public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor)
        {
            this.idp = idp;
            this.accessor = accessor;
        }

        public void SignIn()
        {
            var protector = idp.CreateProtector("Authentication.Program.Login");
            accessor.HttpContext.Response.Headers["Set-Cookie"] =
            $"auth={protector.Protect("token")}; Path=/; HttpOnly; Secure; SameSite=Strict";
        }
    }
}
