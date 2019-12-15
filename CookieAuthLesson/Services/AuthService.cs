using CookieAuthLesson.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CookieAuthLesson.Services
{
    public class AuthService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserContext context;

        public AuthService(IHttpContextAccessor httpContext, UserContext context)
        {
            this.httpContextAccessor = httpContext;
            this.context = context;
        }

        public async Task<bool> AuthenticateUser(string email, string password)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Email == email && x.Password == password);

            if (user is null) return false;
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                RedirectUri = "/Home/Index"
            };

            await httpContextAccessor.HttpContext.SignInAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            return true;
        }

        public async Task<bool> RegistrateUser(string email, string password)
        {
            await context.Users.AddAsync(new Models.User { Email = email, Password = password });
            await context.SaveChangesAsync();

            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                RedirectUri = "/Home/Index"
            };

            await httpContextAccessor.HttpContext.SignInAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            return true;
        }

        public async void SignOutUser()
        {
            await httpContextAccessor.HttpContext.SignOutAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public string DecryptClaim()
        {
            // Get the encrypted cookie value
            var opt = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions>>();
            var cookie = opt.CurrentValue.CookieManager.GetRequestCookie(httpContextAccessor.HttpContext, ".AspNetCore.Cookies");

            // Decrypt if found
            if (!string.IsNullOrEmpty(cookie))
            {
                var dataProtector = opt.CurrentValue.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, "v2");

                var ticketDataFormat = new TicketDataFormat(dataProtector);
                var ticket = ticketDataFormat.Unprotect(cookie);
                var claims = ticket.Principal.Claims;
                var list = claims.ToList();

                return list[0].Value;
            }
            return null;
        }
    }
}
