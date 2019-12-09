using CookieAuthLesson.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                RedirectUri = "/Home/Index"
            };

            await httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
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
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                RedirectUri = "/Home/Index"
            };

            await httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            return true;
        }

        public async void SignOutUser()
        {
            await httpContextAccessor.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
