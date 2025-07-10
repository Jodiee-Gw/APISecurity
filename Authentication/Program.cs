using Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataProtection();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication("Cookie").AddCookie("Cookie", options =>
    {
        options.Cookie.Name = "auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
        options.LoginPath = "/login";
    });
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
//app.Use((ctx, next) =>
//{
//    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
//    var protector = idp.CreateProtector("Authentication.Program.Login");
//    var cookie = protector.Unprotect(ctx.Request.Headers.Cookie.
//        FirstOrDefault(x => x.StartsWith("auth=")).
//        Split("=").Last());

//    var claims= new List<Claim>
//    {
//        new Claim(ClaimTypes.Name, cookie)
//    };
//    var identity = new ClaimsIdentity(claims);
//    ctx.User = new ClaimsPrincipal(identity);

//    return next();
//});

//app.MapGet("/username", (HttpContext context, IDataProtectionProvider idp) =>
//{
//    var protector = idp.CreateProtector("Authentication.Program.Login");
//    var cookie = protector.Unprotect(context.Request.Headers.Cookie.
//        FirstOrDefault(x => x.StartsWith("auth=")).
//        Split("=").Last());

//    return context.User.FindFirst(ClaimTypes.Name).Value;
//    //return cookie;
//    //return "anton";
//});
app.MapGet("/username", (HttpContext context) =>
{
    if (context.User.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
    {
        var token = context.User.FindFirst("auth")?.Value;
        return Results.Ok($"Logged In, token: {token}");
    }

    return Results.Unauthorized();
    //return cookie;
    //return "anton";
});

//app.MapGet("/login", (AuthService authService) =>
//{
//    authService.SignIn();
//    //var protector = idp.CreateProtector("Authentication.Program.Login");
//    //context.Response.Headers["Set-Cookie"] = 
//    //$"auth={protector.Protect("token")}; Path=/; HttpOnly; Secure; SameSite=Strict";
//    return "ok";
//});

app.MapGet("/login", async (HttpContext httpcontext) =>
{
    //var idp = httpcontext.RequestServices.GetRequiredService<IDataProtectionProvider>();
    //var protector = idp.CreateProtector("Authentication.Program.Login");
    //var cookie = protector.Unprotect(httpcontext.Request.Headers.Cookie.
    //    FirstOrDefault(x => x.StartsWith("auth=")).
    //    Split("=").Last());

    var claims = new List<Claim>();
    claims.Add(new Claim("auth", "token"));
    var identity = new ClaimsIdentity(claims, "Cookie");
    await httpcontext.SignInAsync("Cookie", new ClaimsPrincipal(identity));

    //var protector = idp.CreateProtector("Authentication.Program.Login");
    //context.Response.Headers["Set-Cookie"] = 
    //$"auth={protector.Protect("token")}; Path=/; HttpOnly; Secure; SameSite=Strict";
    return "ok";
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
