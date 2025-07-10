using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = "Cookies";
//    options.DefaultChallengeScheme = "github";
//})
//    .AddOAuth("github", o =>
//    {
//        o.ClientId = "Ov23liQbWKhACGwsF1Qe";
//        o.ClientSecret = "3e7c934c40111dc15914b1d5ee2b8cdd8643cdc1";

//        o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";

//        o.TokenEndpoint = "";
//        o.CallbackPath = "/oauth/";
//        o.UserInformationEndpoint = "";
//    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "github";
})
.AddCookie()
.AddOAuth("github", options =>
{
    //options.SignInScheme = "Cookie";
    options.ClientId = "Ov23liQbWKhACGwsF1Qe";
    options.ClientSecret = "3e7c934c40111dc15914b1d5ee2b8cdd8643cdc1";

    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
    options.UserInformationEndpoint = "https://api.github.com/user";

    options.CallbackPath = "/oauth/github-cb";

    //options.SaveTokens = true;
    //options.Scope.Add("user:email");

    //options.ClaimActions.MapJsonKey("urn:github:login", "login");
    //options.ClaimActions.MapJsonKey("urn:github:id", "id");
    //options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");
    //options.ClaimActions.MapJsonKey("urn:github:url", "html_url");

    //options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
    //{
    //    OnCreatingTicket = async context =>
    //    {
    //        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
    //        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
    //        request.Headers.Add("User-Agent", "ASP.NET Core OAuth");

    //        var response = await context.Backchannel.SendAsync(request);
    //        response.EnsureSuccessStatusCode();

    //        using var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    //        context.RunClaimActions(user.RootElement);
    //    }
    //};
    options.SaveTokens = true;
    options.Scope.Add("user:email");

    options.ClaimActions.MapJsonKey("urn:github:login", "login");
    options.ClaimActions.MapJsonKey("urn:github:id", "id");
    options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");
    options.ClaimActions.MapJsonKey("urn:github:url", "html_url");

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
            request.Headers.Add("User-Agent", "ASP.NET Core OAuth");

            var response = await context.Backchannel.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            context.RunClaimActions(user.RootElement);
        }
    };
});

var app = builder.Build();

app.UseAuthentication();


app.MapGet("/", (HttpContext httpContext) =>
{
    return httpContext.User.Claims.Select(x => new { x.Type, x.Value }).ToList();
});

app.MapGet("/login", (HttpContext httpContext) =>
{

    return Results.Challenge(
       new AuthenticationProperties()
       {
           RedirectUri = "https://localhost:5005/"
       },
       authenticationSchemes: new List<string>() { "github" }
   );
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
