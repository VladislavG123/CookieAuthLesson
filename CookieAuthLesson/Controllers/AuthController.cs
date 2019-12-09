using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookieAuthLesson.Services;
using CookieAuthLesson.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CookieAuthLesson.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService authService;

        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Auth([FromForm]AuthViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }

            if (await authService.AuthenticateUser(model.Email, model.Password))
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Index");
        }

        public ActionResult Registrate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrate([FromForm]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }

            if (await authService.RegistrateUser(model.Email, model.Password))
            {
                return RedirectToAction("Index", "Home");
            }

            return View("Index");
        }

    }
}